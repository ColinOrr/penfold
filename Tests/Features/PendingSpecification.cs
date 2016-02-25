using Machine.Specifications;
using NUnit.Framework;
using Penfold;
using System;
using System.IO;
using System.Linq;

namespace Tests.Features
{
    public class PendingSpecification : Specification
    {
        public PendingSpecification()
        {
            describe["Executing a specification"] = () =>
            {
                context["with a pending assertion"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };
                    AggregateException exception = null;

                    before = () =>
                    {
                        specification.it["has a pending assertion"] = null;
                        exception = Catch<AggregateException>(() => specification.Execute());
                    };

                    it["throws an inconclusive exception to the test runner"] = () =>
                    {
                        exception.InnerExceptions.Single().ShouldBeOfExactType<InconclusiveException>();
                    };

                    it["logs that the assertion is pending"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("has a pending assertion [PENDING]");
                    };
                };

                context["with a pending activity"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };
                    var executed = false;
                    AggregateException exception = null;

                    before = () =>
                    {
                        specification.When["it has a pending assertion"] = null;
                        specification.it["still does something"] = () => executed = true;
                        exception = Catch<AggregateException>(() => specification.Execute());
                    };

                    it["executes the code in subsequent assertions"] = () =>
                    {
                        executed.ShouldBeTrue();
                    };

                    it["logs that the activity is pending"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("When it has a pending assertion [PENDING]");
                    };
                };
            };

            describe["Writing a specification"] = () =>
            {
                context["with a pending context"] = () =>
                {
                    var specification = new Specification();

                    before = () =>
                    {
                        specification.describe["pending context"] = null;
                    };

                    it["contains no tests"] = () =>
                    {
                        specification.Tests.ShouldBeEmpty();
                    };
                };
            };
        }
    }
}
