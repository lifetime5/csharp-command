using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Haruair.Command.Interface;

namespace Haruair.Command
{
	public class Commander
	{
		public IList<Type> Commands
		{
			get;
            protected set;
		}

		public IRequestParser RequestParser
		{
			get;
			set;
		}

		public IPrompter Prompter
		{
			get;
			set;
		}

		public ICommandResolver CommandResolver
		{
			get;
			set;
		}

		public Commander()
		{
			Commands = new List<Type>();
		}

		public Commander Add(Type type)
		{
			Commands.Add(type);
			return this;
		}

		public Commander Add<T>()
		{
			Add(typeof(T));
			return this;
		}

		public string Parse(bool exectue, string[] args)
		{

			if (RequestParser == null)
			{
				RequestParser = new BasicRequestParser();
			}

			if (CommandResolver == null)
			{
				CommandResolver = new BasicCommandResolver();
			}

			if (Prompter == null)
			{
				Prompter = new BasicConsolePrompter();
			}

			var request = RequestParser.Parse(args);

			CommandResolver.Commands = Commands;

			var meta = CommandResolver.Match(request);

            PrintMessage(request);

            if (meta != null)
			{
				var methodParameters = meta.MethodInfo.GetParameters();
				var methodParamAttributes = (Parameter[])Attribute.GetCustomAttributes(meta.MethodInfo, typeof(Parameter));
                methodParamAttributes = methodParamAttributes.OrderBy(param => param.Index).ToArray();

				IList<string> parameters = new List<string>(new string[methodParameters.Length]);

				if (methodParameters.Length > 0)
				{
					foreach (var methodParameter in methodParameters)
					{
						var index = Array.IndexOf(methodParameters, methodParameter);

						var methodParamAttribute = methodParamAttributes.FirstOrDefault(p => p.Attribute.Equals(methodParameter.Name));
						var paramIndex = Array.IndexOf(methodParamAttributes, methodParamAttribute);
						var requestParam = request.Params.ElementAtOrDefault(paramIndex);

						parameters[index] = requestParam;
					}
				}

				foreach (var attr in methodParamAttributes)
				{
					var index = Array.IndexOf(methodParamAttributes, attr);
					if (attr.Required && parameters.ElementAtOrDefault(index) == null)
					{
                        return attr.Attribute;
                    }
				}

                if (exectue || meta.Offline)
                {
                    if (parameters.Count == 0)
                    {
                        meta.MethodInfo.Invoke(Activator.CreateInstance(meta.CommandType), null);
                    }
                    else
                    {
                        meta.MethodInfo.Invoke(Activator.CreateInstance(meta.CommandType), parameters.ToArray());
                    }
                    return string.Empty;
                }
			    return Environment.NewLine;

			}
		    return null;
		}

		protected void PrintMessage(IRequest request)
		{
		    var details = false;
			var list = CommandResolver.Find(request);
			var identity = list.FirstOrDefault();
            if (identity.MethodInfo != null)
            {
                details = list.Any(meta => meta.Method != null && meta.Method == request.Method);
                if (details)
                {
                    Prompter.WriteLine("Example of {0} {1}:", request.Command, request.Method);
			    }
			    else
                {
                    Prompter.WriteLine("Example of {0}:", request.Command);
                }
			}
			else {
				Prompter.WriteLine("Example: ");
            }
		    PrintCommands(list, details ? request.Method : null);
            Prompter.WriteSeparator();
            Prompter.WriteLine("[CMD]\t" + DateTime.Now.ToString(CultureInfo.CurrentCulture));
        }

		protected void PrintCommands(IList<CommandMeta> metaList, string method)
		{
		    var list = method != null ? metaList.Where(p => p.Method == method) : metaList.Where(p => p.Method != null);

            foreach (var meta in list)
			{
				Prompter.Write("  {0}", meta.Method);
			    if (meta.MethodInfo != null)
                {
                    if (method != null)
                    {
                        var methodParamAttributes = (Parameter[])Attribute.GetCustomAttributes(meta.MethodInfo, typeof(Parameter));
                        methodParamAttributes = methodParamAttributes.OrderBy(param => param.Index).ToArray();
                        foreach (var param in methodParamAttributes)
                        {
                            Prompter.Write(param.Required ? " <{0}>" : " [{0}]", param.Attribute);
                        }
                        Prompter.WriteLine();
                        foreach (var param in methodParamAttributes)
                        {
                            Prompter.WriteLine("    {0}\t{1}", param.Attribute, param.Description);
                        }
                        goto End;
                    }
                }
                if (meta.Description != null) Prompter.WriteLine("\t{0}", meta.Description);
                else
                    Prompter.WriteLine();

                End: { }
            }
		}
	}
}
