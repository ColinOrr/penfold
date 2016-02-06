using System.Collections.Generic;
using System.Linq;

namespace Penfold
{
    /// <summary>
    /// A container for a collection of other steps that logically run together,
    /// sharing same setup and scope.
    /// </summary>
    public class Context : Step
    {
        public List<Step> Steps { get; private set; }

        public override bool Executed
        {
            get
            {
                return this.Flatten().Skip(1).Any(s => s.Executed);
            }
        }

        public Context()
        {
            Steps = new List<Step>();
        }
    }
}
