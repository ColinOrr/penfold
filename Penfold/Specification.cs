using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Penfold
{
    /// <summary>
    /// Defines the Penfold specification language, builds the test plans and executes them using NUnit.
    /// </summary>
    [TestFixture]
    public abstract class Specification
    {
        private readonly List<Assertion> steps;
        protected readonly StepBuilder it;

        public Specification()
        {
            steps = new List<Assertion>();
            it = new StepBuilder(steps);
        }

        public IEnumerable<TestCaseData> Tests
        {
            get { return from s in steps select new TestCaseData(s).SetName(s.Title); }
        }

        [Test, TestCaseSource("Tests")]
        public void Execute(Assertion test)
        {
            Console.WriteLine(test.Title);
            test.Action();
        }
    }
}
