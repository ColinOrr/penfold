using System;

namespace Penfold
{
    /// <summary>
    /// Defines the standard Penfold specification language.
    /// </summary>
    public class Specification : SpecificationBase
    {
        // RSpec Keywords
        public StepBuilder<Context> describe { get; private set; }
        public StepBuilder<Context> context { get; private set; }
        public StepBuilder<Assertion> it { get; private set; }

        public Action before { set { Context.Add<Activity>(value); } }
        public Action after { set { Context.Add<Activity>(value); } }

        // Gherkin Keywords
        public StepBuilder<Context> Scenario { get; private set; }
        public StepBuilder<Activity> Given { get; private set; }
        public StepBuilder<Activity> When { get; private set; }
        public StepBuilder<Assertion> Then { get; private set; }

        public Specification()
        {
            describe = new StepBuilder<Context>(this);
            context = new StepBuilder<Context>(this);
            it = new StepBuilder<Assertion>(this);

            Scenario = new StepBuilder<Context>(this, "Scenario: {0}");
            Given = new StepBuilder<Activity>(this, "Given {0}");
            When = new StepBuilder<Activity>(this, "When {0}");
            Then = new StepBuilder<Assertion>(this, "Then {0}");
        }
    }
}
