using System;

namespace Haruair.Command
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class Command : Attribute
	{
		public string Method
		{
			get;
			private set;
		}

	    public string Description
	    {
	        get;
            private set;
        }
        
        public Command(string method, string description = "")
		{
			Method = method;
            Description = description;
        }
    }
}
