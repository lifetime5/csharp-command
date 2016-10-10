using System;

namespace Haruair.Command
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true)]
	public class Parameter : Attribute
	{
		public string Attribute
		{
			get;
			set;
		}

	    public uint Index
	    {
	        get;
            set;
        }

		public bool Required
		{
			get;
			set;
        }

        public string Description
        {
            get;
            private set;
        }

        public Parameter(string attribute, uint index, bool required)
		{
			Attribute = attribute;
		    Index = index;
            Required = required;
        }

        public Parameter(string attribute, uint index, bool required, string description)
        {
            Attribute = attribute;
            Index = index;
            Required = required;
            Description = description;
        }
    }
}
