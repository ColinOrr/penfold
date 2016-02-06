using System;

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
        public abstract bool Executed { get; }

        public override string ToString()
        {
            return Title;
        }
    }
}
