using NUnit.Framework;

namespace Penfold
{
    /// <summary>
    /// The primary step that Penfold uses, an assesrtion maps directly to an
    /// NUnit test that can be executed.  Every specification needs at least one
    /// assertion to be executable.
    /// </summary>
    public class Assertion : Step
    {
        public TestStatus? Status { get; set; }
    }
}
