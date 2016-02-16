using System;
using System.Collections.Generic;
using System.Linq;

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
                if (step is Context)
                {
                    foreach (var child in ((Context)step).Flatten())
                    {
                        yield return child;
                    }
                }
                else yield return step;
            }
        }

        /// <summary>
        /// Retrieves the ancestors of a step from the oldest to the newest.
        /// </summary>
        public static IEnumerable<Context> Ancestors(this Step step)
        {
            if(step.Context != null)
            {
                foreach (var parent in step.Context.Ancestors())
                {
                    yield return parent;
                }

                yield return step.Context;
            }
        }

        /// <summary>
        /// Retrieves the steps in a sequence before the current one.
        /// </summary>
        public static IEnumerable<Step> Before(this IEnumerable<Step> steps, Step current)
        {
            foreach (var step in steps)
            {
                if (step == current) break;
                yield return step;
            }
        }

        /// <summary>
        /// Retrieves the steps in a sequence after the current one.
        /// </summary>
        public static IEnumerable<Step> After(this IEnumerable<Step> steps, Step current)
        {
            var found = false;
            foreach (var step in steps)
            {
                if (found) yield return step;
                else if (step == current) found = true;
            }
        }

        /// <summary>
        /// Executes any setup steps for the specified step.
        /// </summary>
        public static void ExecuteSetupSteps(this Step step)
        {
            if (step.Context == null || step.Ignored || step.Action == null) return;

            foreach (var setup in step.Context.Steps.OfType<Setup>()) 
            {
                if (setup.Ignored || setup.Action == null) continue;

                setup.Action();
            }
        }

        /// <summary>
        /// A short-hand convenience method for string.IsNullOrWhitespace(value)
        /// </summary>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// The opposite of <see cref="IsEmpty"/>.
        /// </summary>
        public static bool IsNotEmpty(this string value)
        {
            return !value.IsEmpty();
        }
    }
}
