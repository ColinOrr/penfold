using Machine.Specifications;
using Penfold;
using System;
using System.IO;

namespace Tests.Features
{
    public class ContextSpecification : Specification
    {
        public ContextSpecification()
        {
            describe["Executing a specificaton"] = () =>
            {
                context["with a single context"] = () =>
                {
                    var specification = new MySpecification { Logger = new StringWriter() };

                    before = () =>
                    {
                        specification.describe["something"] = () =>
                        {
                            specification.it["does something"] = () => { };
                            specification.it["does something else"] = () => { };
                        };
                        specification.Execute();
                    };

                    it["logs each assertion indented within the context"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain(
                            "  something" + Environment.NewLine +
                            "    does something" + Environment.NewLine +
                            "    does something else"
                        );
                    };
                };

                context["with nested contexts"] = () =>
                {
                    var specification = new MySpecification { Logger = new StringWriter() };

                    before = () =>
                    {
                        specification.describe["something"] = () =>
                        {
                            specification.context["in one context"] = () =>
                            {
                                specification.it["does one thing"] = () => { };
                            };

                            specification.context["in another context"] = () =>
                            {
                                specification.it["does another thing"] = () => { };
                            };
                        };
                        specification.Execute();
                    };

                    it["logs each context indented within their parent"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain(
                            "  something" + Environment.NewLine +
                            "    in one context" + Environment.NewLine +
                            "      does one thing" + Environment.NewLine +
                            "    in another context" + Environment.NewLine +
                            "      does another thing"
                        );
                    };
                };
            };

            describe["Executing a single test"] = () =>
            {
                context["with multiple contexts"] = () =>
                {
                    var specification = new MySpecification { Logger = new StringWriter() };
                    var setupCount    = 0;
                    var activityCount = 0;

                    before = () =>
                    {
                        specification.before_each = () => setupCount++;
                        specification.before = () => activityCount++;

                        specification.context["context A"] = () =>
                        {
                            specification.before_each = () => setupCount++;
                            specification.before = () => activityCount++;
                        };

                        specification.context["context B"] = () =>
                        {
                            specification.it["assertion B1"] = () => { };
                        };

                        specification.Execute();
                    };

                    it["doesn't execute setup steps in other contexts"] = () =>
                    {
                        setupCount.ShouldEqual(1);
                    };

                    it["doesn't execute activities in other contexts"] = () =>
                    {
                        activityCount.ShouldEqual(1);
                    };
                };
            };
        }
    }
}
