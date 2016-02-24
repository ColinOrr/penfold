using System;
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
        public SpecificationBase Specification { get; set; }
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

        /// <summary>
        /// Adds a step to the current context, ensuring that the step's 
        /// <see cref="Step.Context"/> is set.
        /// </summary>
        public void Add(Step step)
        {
            step.Context = this;
            Steps.Add(step);
        }

        /// <summary>
        /// Adds an action as a blank step of type T.
        /// </summary>
        public void Add<T>(Action action) where T : Step, new()
        {
            var step = new T { Action = action };
            this.Add(step);
        }

        /// <summary>
        /// Adds a comment to the context.
        /// </summary>
        public void AddComment(string comment)
        {
            Add<Activity>(() => { Specification.log(comment.Unindent()); });
        }
    }
}
