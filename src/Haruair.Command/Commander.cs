﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Haruair.Command.Interface;

namespace Haruair.Command
{
	public class Commander
	{
		protected IList<Type> Commands {
			get;
			set;
		}

		public IRequestParser RequestParser {
			get;
			set;
		}

		public IPrompter Prompter {
			get;
			set;
		}

		public ICommandResolver CommandResolver {
			get;
			set;
		}

		public Commander ()
		{
			this.Commands = new List<Type> ();
		}

		public Commander Add (Type type) {
			this.Commands.Add (type);
			return this;
		}

		public Commander Add<T> () {
			this.Add (typeof(T));
			return this;
		}

		public void Parse(string[] args) {

			if (RequestParser == null) {
				RequestParser = new BasicRequestParser ();
			}

			if (CommandResolver == null) {
				CommandResolver = new BasicCommandResolver ();
			}

			if (Prompter == null) {
				Prompter = new BasicConsolePrompter ();
			}

			var request = RequestParser.Parse (args);

			CommandResolver.Commands = this.Commands;

			var meta = CommandResolver.Match (request);

			if (meta != null) {
				meta.CallMethod.Invoke (Activator.CreateInstance (meta.Command), null);
			} else {
				var list = CommandResolver.Find (request);
				var identity = list.FirstOrDefault ();
				if (identity.CallMethod != null) {
					Prompter.WriteLine ("Example of {0}:", request.Command);
				} else {
					Prompter.WriteLine ("Example: ");
				}
				this.PrintCommands (list);
			}
		}

		protected void PrintCommands(IList<CommandMeta> metaList) {
			foreach (var meta in metaList) {
				Prompter.Write ("  {0}", meta.Method);
				if(meta.Alias != null) Prompter.Write (", {0}", meta.Alias);
				if(meta.Description != null) Prompter.WriteLine ("\t{0}", meta.Description);
				else
					Prompter.WriteLine ();
			}
		}
	}
}

