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
        private IndentedTextWriter logger;

        /// <summary>
        /// Gets or sets the logger used for the output.
        /// </summary>
        public TextWriter Logger
        {
            get { return logger.InnerWriter; }
            set 
            {
                value = value ?? new StringWriter();
                logger = new IndentedTextWriter(value, "  "); 
            }
        }

        /// <summary>
        /// Gets or sets the current context containing the steps to be executed.
        /// </summary>
        public Context Context { get; set; }

        /// <summary>
        /// Gets the asssertions to be tested.
        /// Called by NUnit to identify the sequence of tests to be passed to <see cref="Execute(Assertion)"/>.
        /// </summary>
        public IEnumerable<TestCaseData> Tests
        {
            get
            {
                return
                    from step in Context.Flatten().OfType<Assertion>()
                    select new TestCaseData(step)
                        .SetName(step.Title)
                        .SetCategory(string.Join(" · ", step.Ancestors()));
            }
        }

        /// <summary>
        /// Constructs the initial <see cref="Context"/>, ready to be populated with <see cref="Step"/>s.
        /// </summary>
        protected SpecificationBase()
        {
            Logger = Console.Out;
            
            Context = new Context
            {
                Title = this.GetType().Name,
            };
        }

        /// <summary>
        /// Executes the specification.
        /// </summary>
        public void Execute()
        {
            foreach (var test in Tests)
            {
                try { Execute(test.Arguments.Cast<Assertion>().Single()); }
                catch { }
            }
        }

        /// <summary>
        /// Executes an individual test.
        /// </summary>
        [Test, TestCaseSource("Tests")]
        public void Execute(Assertion test)
        {
            try
            {
                var plan = test.Ancestors().First().Flatten();

                // Run through the previous unexecuted contexts and activities
                foreach (var step in plan.Before(test).Where(s => !s.Executed))
                {
                    if (step is Context) log(step);
                    else if (step is Activity)
                    {
                        log(step);
                        step.Action();
                    }
                }

                // Execute the test
                log(test);
                test.Action();
                test.Status = TestStatus.Passed;
            }
            catch
            {
                test.Status = TestStatus.Failed;
                throw;
            }
        }

        /// <summary>
        /// Logs the title of a specified test.
        /// </summary>
        public void log(Step step)
        {
            if (step.ToString().IsEmpty()) return;
            logger.Indent = step.Ancestors().Count(a => a.ToString().IsNotEmpty());
            logger.WriteLine(step);
        }

        /// <summary>
        /// Logs the specified message to the test output.
        /// </summary>
        public void log(object message)
        {
            logger.WriteLine(message);
        }

        /// <summary>
        /// Executes the code an returns the exception raised, or null if the code succeeds.
        /// </summary>
        public Exception Catch(Action code)
        {
            return Catch<Exception>(code);
        }

        /// <summary>
        /// Executes the code an returns the exception raised, or null if the code succeeds.
        /// </summary>
        public TException Catch<TException>(Action code) where TException : Exception
        {
            try { code(); }
            catch (TException e) { return e; }
            return null;
        }
    }
}
