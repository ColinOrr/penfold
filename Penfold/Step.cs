﻿using System;
using System.Collections.Generic;

namespace Penfold
{
    /// <summary>
    /// The base class for defining steps in a specification.
    /// </summary>
    public abstract class Step
    {
        public Context Context { get; set; }
        public string Title { get; set; }
        public Action Action { get; set; }
        public string Format { get; set; }
        public bool Ignored { get; set; }
        public List<string> Categories { get; private set; }
        
        public abstract bool Executed { get; }

        public Step()
        {
            Categories = new List<string>();
        }

        public override string ToString()
        {
            var format = Format ?? "{0}";
            return string.Format(format, Title);
        }
    }
}
