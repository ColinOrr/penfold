using System.Linq;

namespace Penfold
{
    /// <summary>
    /// Setup steps are run multiple times before each of their siblings within 
    /// a context.  For setup steps that run only once, use an <see cref="Activity"/>.
    /// </summary>
    public class Setup : Step
    {
        public override bool Executed
        {
            get
            {
                return Context.Steps.After(this).Any(s => s.Executed);
            }
        }
    }
}
