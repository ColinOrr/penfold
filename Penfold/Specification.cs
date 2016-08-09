using System;
using System.Linq;

namespace Penfold
{
    /// <summary>
    /// Defines the standard Penfold specification language.
    /// </summary>
    public abstract class Specification : SpecificationBase
    {
        // RSpec Keywords
        public StepBuilder<Context> describe { get; private set; }
        public StepBuilder<Context> context { get; private set; }
        public StepBuilder<Assertion> it { get; private set; }

        public Action before { set { Context.Add<Activity>(value); } }
        public Action before_each { set { Context.Add<Setup>(value); } }

        // Gherkin Keywords
        public StepBuilder<Context> Scenario { get; private set; }
        public StepBuilder<Activity> Given { get; private set; }
        public StepBuilder<Activity> When { get; private set; }
        public StepBuilder<Assertion> Then { get; private set; }

        public Action Background { set { Context.Add<Setup>(value); } }

        // Ignores, Categories & Comments
        public Action x { set { Context.Steps.Last().Ignored = true; } }
        public Action @ignore { set { Context.Steps.Last().Ignored = true; } }
        public Action @current { set { Context.Steps.Last().Categories.Add(".current"); } }
        public CategoryBuilder @_ { get; private set; }
        public string comment { set { Context.AddComment(value); } }

        protected Specification()
        {
            describe = new StepBuilder<Context>(this);
            context = new StepBuilder<Context>(this);
            it = new StepBuilder<Assertion>(this);

            Scenario = new StepBuilder<Context>(this, "Scenario: {0}");
            Given = new StepBuilder<Activity>(this, "Given {0}");
            When = new StepBuilder<Activity>(this, "When {0}");
            Then = new StepBuilder<Assertion>(this, "Then {0}");

            @_ = new CategoryBuilder(this);
        }
    }
}
