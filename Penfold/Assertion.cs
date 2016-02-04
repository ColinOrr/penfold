using System;

namespace Penfold
{
    /// <summary>
    /// Represents a test that is run against the current context.
    /// </summary>
    public class Assertion
    {
        public string Title { get; set; }
        public Action Action { get; set; }
    }
}
