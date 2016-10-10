using System;
using System.Collections.Generic;
using Haruair.Command.Interface;

namespace Haruair.Command
{
	public class Request : IRequest
	{
	    private string command;
		public string Command
        {
            get { return command; }
            set { command = value == null ? null : value.ToLower(); }
        }
        private string method;
        public string Method
        {
            get { return method; }
            set { method = value == null ? null : value.ToLower(); }
        }
        private IList<string> @params = new List<string>();
        public IList<string> Params
		{
			get { return @params; }
			set { @params = value; }
        }

        private IDictionary<string, string> options = new Dictionary<string, string>();
        public IDictionary<string, string> Options
        {
            get { return options; }
            set { options = value; }
        }

    }
}
