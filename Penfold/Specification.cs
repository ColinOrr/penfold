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

        public Action before { set { Context.Add<Activity>(value); } }
        public Action after { set { Context.Add<Activity>(value); } }

        public Specification()
        {
            describe = new StepBuilder<Context>(this);
            context = new StepBuilder<Context>(this);
            it = new StepBuilder<Assertion>(this);
        }
    }
}
