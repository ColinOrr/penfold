using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using System;
using System.Collections.Generic;

namespace Penfold
{
    /// <summary>
    /// Indicates that the source used to provide test cases is a Penfold 
    /// specification.
    /// </summary>
    public class SpecificationSourceAttribute : TestCaseSourceAttribute, ITestBuilder
    {
        private readonly NUnitTestCaseBuilder builder = new NUnitTestCaseBuilder();
        public SpecificationSourceAttribute() : base("") { }

        /// <summary>
        /// Constructs the specification and builds test cases from the 
        /// <see cref="SpecificationBase.Tests"/>.
        /// </summary>
        IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test suite)
        {
            var tests = new List<TestMethod>();

            try
            {
                var specification = (SpecificationBase)method.TypeInfo.Construct(null);

                foreach (var test in specification.Tests)
                {
                    tests.Add(builder.BuildTestMethod(method, suite, test));
                }
            }
            catch
            {
                var test = new TestCaseData(new Assertion { Action = () => { } });
                test.SetName(method.TypeInfo.Name);
                tests.Add(builder.BuildTestMethod(method, suite, test));
            }

            return tests;
        }
    }
}
