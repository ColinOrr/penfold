using Penfold;
using System.IO;
using Should;
using System;

namespace Tests.Features
{
    public class LogSpecification : Specification
    {
        public LogSpecification()
        {
            describe["Executing a specification"] = () =>
            {
                context["with a single line log message"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };
                    var result = "";

                    before = () =>
                    {
                        specification.it["logs a message"] = () => 
                        {
                            result = specification.log("Hello Mum");
                        };
                        specification.Execute();
                    };

                    it["logs the message indented within the step"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain(
                            "  logs a message" + Environment.NewLine +
                            "    Hello Mum"
                        );
                    };

                    it["returns the message"] = () =>
                    {
                        result.ShouldEqual("Hello Mum");
                    };
                };

                context["with a multi-line log message"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };
                    var result = "";

                    before = () =>
                    {
                        specification.it["logs a message"] = () =>
                        {
                            result = specification.log("Hello\r\nMum");
                        };
                        specification.Execute();
                    };

                    it["applies the correct indentation to each line of the message"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain(
                            "  logs a message" + Environment.NewLine +
                            "    Hello" + Environment.NewLine +
                            "    Mum"
                        );
                    };
                };
            };
        }
    }
}
