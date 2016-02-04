using System;
using System.Collections.Generic;

namespace Penfold
{
    /// <summary>
    /// Allows steps in a specification to be expressed using indexes and actions.
    /// </summary>
    public class StepBuilder
    {
        private readonly List<Assertion> steps;

        public StepBuilder(List<Assertion> steps)
        {
            this.steps = steps;
        }

        public Action this[string title]
        {
            set
            {
                var step = new Assertion
                {
                    Title  = title,
                    Action = value,
                };

                steps.Add(step);
            }
        }
    }
}
