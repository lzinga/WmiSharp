using System;
using System.Collections.Generic;
using System.Text;

namespace WmiSharp.Attributes
{
    public class WmiPropertyAttribute : Attribute
    {
        public WmiPropertyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
