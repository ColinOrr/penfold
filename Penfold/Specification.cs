using System;

namespace Penfold
{
    /// <summary>
    /// Defines the standard Penfold specification language.
    /// </summary>
    public abstract class Specification : SpecificationBase
    {
        public StepBuilder<Context> describe { get; private set; }
        public StepBuilder<Context> context { get; private set; }
        public StepBuilder<Assertion> it { get; private set; }

        public Action before
        {
            set
            {
                var step = new Activity { Context = Context, Action = value };
                Context.Steps.Add(step);
            }
        }

        public Specification()
        {
            describe = new StepBuilder<Context>(this);
            context = new StepBuilder<Context>(this);
            it = new StepBuilder<Assertion>(this);
        }
    }
}
