using Machine.Specifications;
using Penfold;
using System;
using System.IO;

namespace Tests.Features
{
    public class CommentSpecification : Specification
    {
        public CommentSpecification()
        {
            describe["Executing a specification"] = () =>
            {
                context["with a single line comment"] = () =>
                {
                    var specification = new MySpecification { Logger = new StringWriter() };
                    
                    before = () =>
                    {
                        specification.comment = "I want to write specs";
                        specification.it["does something"] = () => { };
                        specification.Execute();
                    };

                    it["writes the comment into the log with the correct indentation"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain(
                            "Specification"           + Environment.NewLine +
                            "  I want to write specs" + Environment.NewLine +
                            "  does something"
                        );
                    };
                };

                context["with a multi-line comment"] = () =>
                {
                    var specification = new MySpecification { Logger = new StringWriter() };

                    before = () =>
                    {
                        specification.comment = @"
                            as a programmer
                            I want to write specs
                            in order to improve my code quality
                        ";

                        specification.it["does something"] = () => { };
                        specification.Execute();
                    };

                    it["writes the comment into the log with the correct indentation"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain(
                            "Specification" + Environment.NewLine +
                            "  as a programmer" + Environment.NewLine +
                            "  I want to write specs" + Environment.NewLine +
                            "  in order to improve my code quality" + Environment.NewLine +
                            "  does something"
                        );
                    };
                };
            };
        }
    }
}
