using Penfold;
using Should;
using System;
using System.IO;

namespace Tests.Features
{
    public class CategorySpecification : Specification
    {
        public CategorySpecification()
        {
            describe["Executing a specificaton"] = () =>
            {
                context["with a single context"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };

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
                            "  something"             + Environment.NewLine +
                            "    does something"      + Environment.NewLine +
                            "    does something else"
                        );
                    };
                };

                context["with nested contexts"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };

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
                            "  something"              + Environment.NewLine +
                            "    in one context"       + Environment.NewLine +
                            "      does one thing"     + Environment.NewLine +
                            "    in another context"   + Environment.NewLine +
                            "      does another thing" 
                        );
                    };
                };
            };
        }
    }
}
