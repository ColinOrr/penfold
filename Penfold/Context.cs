using System.Collections.Generic;

namespace Penfold
{
    /// <summary>
    /// A container for a collection of other steps that logically run together,
    /// sharing same setup and scope.
    /// </summary>
    public class Context : Step
    {
        public List<Step> Steps { get; private set; }

        public Context()
        {
            Steps = new List<Step>();
        }
    }
}
