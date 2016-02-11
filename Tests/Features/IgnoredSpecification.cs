using NUnit.Framework;
using Penfold;
using Should;
using System;
using System.IO;
using System.Linq;

namespace Tests.Features
{
    public class IgnoredSpecification : Specification
    {
        public IgnoredSpecification()
        {
            describe["Executing a specification"] = () =>
            {
                context["with an ignored assertion"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };
                    var executedA = false;
                    var executedB = false;
                    AggregateException exception = null;

                    before = () =>
                    {
                        specification.@ignore = 
                            specification.it["has an ignored assertion"] = () => executedA = true;
                        specification.it["has another assertion"] = () => executedB = true;

                        exception = Catch<AggregateException>(() => specification.Execute());
                    };

                    it["doesn't execute the code in the ignored assertion"] = () =>
                    {
                        executedA.ShouldBeFalse();
                    };

                    it["throws an ignored exception to the test runner"] = () =>
                    {
                        exception.InnerExceptions.Single().ShouldBeType<IgnoreException>();
                    };

                    it["executes the code in subsequent assertions"] = () =>
                    {
                        executedB.ShouldBeTrue();
                    };

                    it["logs that the assertion was ignored"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("has an ignored assertion [IGNORED]");
                    };
                };

                context["with an ignored activity"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };
                    var executedA = false;
                    var executedB = false;

                    before = () =>
                    {
                        specification.@ignore =
                            specification.When["I ignore an activity"] = () => executedA = true;
                        specification.it["does something"] = () => executedB = true;

                        specification.Execute();
                    };

                    it["doesn't execute the code in the ignored activity"] = () =>
                    {
                        executedA.ShouldBeFalse();
                    };

                    it["executes the code in subsequent assertions"] = () =>
                    {
                        executedB.ShouldBeTrue();
                    };

                    it["logs that the activity was ignored"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("When I ignore an activity [IGNORED]");
                    };
                };

                context["with an ignored context"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };
                    var executed = false;
                    AggregateException exception = null;

                    before = () =>
                    {
                        specification.@ignore =
                        specification.describe["something"] = () =>
                        {
                            specification.When["I do an activity"] = () => executed = true;
                            specification.it["makes an assertion"] = () => executed = true;
                        };

                        exception = Catch<AggregateException>(() => specification.Execute());
                    };

                    it["doesn't execute any steps inside the context"] = () =>
                    {
                        executed.ShouldBeFalse();
                    };

                    it["logs that the context was ignored"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("something [IGNORED]");
                    };

                    it["logs the activities in the context were ignored"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("When I do an activity [IGNORED]");
                    };

                    it["logs the assertions in the context were ignored"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("makes an assertion [IGNORED]");
                    };

                    it["throws an ignored exception to the test runner"] = () =>
                    {
                        exception.InnerExceptions.Single().ShouldBeType<IgnoreException>();
                    };
                };
            };
        }
    }
}
