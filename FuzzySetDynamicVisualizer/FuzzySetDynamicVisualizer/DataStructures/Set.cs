using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FuzzySetDynamicVisualizer.DataStructures
{
    public class Set
    {
        public readonly string label;
        public readonly List<Member> members = new List<Member>();

        public Set(string label)
        {
            this.label = label;
        }
    }
}
