using System;

namespace Penfold
{
    /// <summary>
    /// Allows steps in a specification to be expressed using the format
    /// keyword["step title"] = () => { ... }
    /// </summary>
    public class StepBuilder<T> where T : Step, new()
    {
        private readonly SpecificationBase specification;

        public StepBuilder(SpecificationBase specification)
        {
            this.specification = specification;
        }

        public Action this[string title]
        {
            set
            {
                var step = new T
                {
                    Context = specification.Context,
                    Title   = title,
                    Action  = value,
                };

                specification.Context.Steps.Add(step);

                // Execute child contexts to gather their steps
                var context = step as Context;
                if (context != null)
                {
                    var original = specification.Context;

                    specification.Context = context;
                    context.Action();
                    specification.Context = original;
                }
            }
        }
    }
}
