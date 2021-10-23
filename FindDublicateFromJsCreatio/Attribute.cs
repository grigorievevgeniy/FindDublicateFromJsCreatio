using System.Collections.Generic;

namespace FindDublicateFromJsCreatio
{
    public class Attribute
    {
        public string Name { get; set; }

        public List<string> Childs { get; set; } = new List<string>();
    }
}
