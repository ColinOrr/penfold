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
                foreach (var step in Context.Flatten().OfType<Assertion>())
                {
                    var test = new TestCaseData(step)
                        .SetName(step.Title)
                        .SetCategory(string.Join(" · ", step.Ancestors()));

                    step.Categories
                        .Union(step.Ancestors().SelectMany(c => c.Categories))
                        .ToList().ForEach(c => test.SetCategory(c));

                    yield return test;
                }
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
                Specification = this,
                Title         = this.GetType().Name,
                Action        = () => { },
            };
        }

        /// <summary>
        /// Executes the specification.
        /// </summary>
        public void Execute()
        {
            var errors = new List<Exception>();

            foreach (var test in Tests)
            {
                try { Execute(test.Arguments.Cast<Assertion>().Single()); }
                catch (Exception e) { errors.Add(e); }
            }

            if (errors.Any()) throw new AggregateException(errors.ToArray());
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
                this.logger = test.Ancestors().First().Specification.logger;

                // Run through the previous unexecuted contexts and activities
                foreach (var step in plan.Before(test).Where(s => !s.Executed))
                {
                    if (step is Context)
                    {
                        logStep(step);
                        step.ExecuteSetupSteps();
                        if (step.Ignored) ((Context)step).Steps.ForEach(s => s.Ignored = true);
                    }
                    else if (step is Activity)
                    {
                        logStep(step);
                        if (!step.Ignored && step.Action != null) step.Action();
                    }
                }

                logStep(test);
                if (test.Ignored) Assert.Ignore();
                if (test.Action == null) Assert.Inconclusive();

                // Execute the test
                test.ExecuteSetupSteps();
                test.Action();
                test.Status = TestStatus.Passed;
            }
            catch (IgnoreException)       { test.Status = TestStatus.Skipped;      throw; }
            catch (InconclusiveException) { test.Status = TestStatus.Inconclusive; throw; }
            catch                         { test.Status = TestStatus.Failed;       throw; }
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

        /// <summary>
        /// Logs a message indented appropriately within the current step.
        /// </summary>
        public T log<T>(T message)
        {
            if (message != null)
            {
                logger.Indent++;
                foreach (var line in message.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    logger.WriteLine(line);
                }
                logger.Indent--;
            }

            return message;
        }

        #region Helpers

        /// <summary>
        /// Logs the title of a specified test.
        /// </summary>
        private void logStep(Step step)
        {
            if (step.ToString().IsEmpty()) return;
            logger.Indent = step.Ancestors().Count(a => a.ToString().IsNotEmpty());

            var status = "";
            if (step.Ignored) status = " [IGNORED]";
            else if (step.Action == null) status = " [PENDING]";
            logger.WriteLine(step + status);
        }

        #endregion
    }
}
