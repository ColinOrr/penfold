using NUnit.Framework;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Penfold
{
    /// <summary>
    /// Implements the mechanics of the specification language, builds the test
    /// plan and executes it using NUnit.
    /// </summary>
    [TestFixture]
    public abstract class SpecificationBase
    {
        private readonly IndentedTextWriter logger;

        /// <summary>
        /// Gets or sets the current context containing the steps to be executed.
        /// </summary>
        public Context Context { get; set; }

        /// <summary>
        /// Gets the asssertions to be tested.
        /// Called by NUnit to identify the sequence of tests to be passed to <see cref="Execute"/>.
        /// </summary>
        public IEnumerable<TestCaseData> Tests
        {
            get
            {
                return
                    from step in Context.Flatten().OfType<Assertion>()
                    select new TestCaseData(step)
                        .SetName(step.Title)
                        .SetCategory(string.Join(" · ", step.Parents()));
            }
        }

        /// <summary>
        /// Constructs the initial <see cref="Context"/>, ready to be populated with <see cref="Step"/>s.
        /// </summary>
        protected SpecificationBase()
        {
            logger = new IndentedTextWriter(Console.Out, "  ");

            Context = new Context
            {
                Title = this.GetType().Name,
            };
        }

        /// <summary>
        /// Executes an individual test.
        /// </summary>
        [Test, TestCaseSource("Tests")]
        public void Execute(Assertion test)
        {
            foreach (var context in test.Parents())
            {
                if (context.Flatten().OfType<Assertion>().All(a => a.Status == null))
                {
                    logger.Indent = context.Parents().Count();
                    logger.WriteLine(context);
                }
            }

            logger.Indent = test.Parents().Count();
            logger.WriteLine(test);

            try
            {
                test.Action();
                test.Status = TestStatus.Passed;
            }
            catch
            {
                test.Status = TestStatus.Failed;
                throw;
            }
        }
    }
}
