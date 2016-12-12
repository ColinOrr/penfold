using System.Linq;

namespace Penfold
{
    /// <summary>
    /// Activities are executed in the order defined by the specification.  An 
    /// activity is executed once, then the results are tested by a number of 
    /// <see cref="Assertion"/>s.
    /// </summary>
    public class Activity : Step
    {
        public override bool Executed
        {
            get
            {
                return Context.Steps.After(this).Where(s => !(s is Activity)).Any(s => s.Executed);
            }
        }
    }
}
