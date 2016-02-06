using System.Collections.Generic;

namespace Penfold
{
    public static class Extensions
    {
        /// <summary>
        /// Flattens nested steps in a hierarchy into a single sequence.
        /// </summary>
        public static IEnumerable<Step> Flatten(this Context context)
        {
            yield return context;

            foreach (var step in context.Steps)
            {
                yield return step;

                if (step is Context)
                {
                    foreach (var child in ((Context)step).Flatten())
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the ancestors of a step from the oldest to the newest.
        /// </summary>
        public static IEnumerable<Context> Parents(this Step step)
        {
            if(step.Context != null)
            {
                foreach (var parent in step.Context.Parents())
                {
                    yield return parent;
                }

                yield return step.Context;
            }
        }
    }
}
