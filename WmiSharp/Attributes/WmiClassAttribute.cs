using System;
using System.Collections.Generic;
using System.Text;

namespace WmiSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WmiClassAttribute : Attribute
    {
        public WmiClassAttribute(string name, string @namespace)
        {
            Name = name;
            Namespace = @namespace;
        }

        public WmiClassAttribute(string name) : this(name, "root/CimV2")
        {

        }

        public string Name { get; }
        public string Namespace { get; }
    }
}
