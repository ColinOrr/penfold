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
        private readonly string format;

        public StepBuilder(SpecificationBase specification, string format = null)
        {
            this.specification = specification;
            this.format = format;
        }

        public Action this[string title]
        {
            set
            {
                var step = new T
                {
                    Title  = title,
                    Action = value,
                    Format = format,
                };

                specification.Context.Add(step);

                // Execute child contexts to gather their steps
                var context = step as Context;
                if (context != null && context.Action != null)
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
