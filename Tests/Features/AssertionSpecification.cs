using Penfold;
using Should;
using System;
using System.IO;

namespace Tests.Features
{
    public class AssertionSpecification : Specification
    {
        public AssertionSpecification()
        {
            describe["Executing a specification"] = () =>
            {
                context["with a single assertion"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };
                    var executed = false;

                    before = () =>
                    {
                        specification.it["does something"] = () => { executed = true; };
                        specification.Execute();
                    };

                    it["executes the code in the assertion"] = () =>
                    {
                        executed.ShouldBeTrue();
                    };

                    it["writes the assertion's title to the logger"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("does something");
                    };
                };

                context["with multiple assertions"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };
                    var executed = 0;

                    before = () =>
                    {
                        specification.it["does something"] = () => executed++;
                        specification.it["does something else"] = () => executed++;
                        specification.Execute();
                    };

                    it["executes the code in both assertions"] = () =>
                    {
                        executed.ShouldEqual(2);
                    };

                    it["writes both assertions to the logger"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("does something");
                        specification.Logger.ToString().ShouldContain("does something else");
                    };
                };

                context["with a failing assertion"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };

                    before = () =>
                    {
                        specification.it["explodes"] = () => { throw new ArithmeticException(); };
                        Catch<ArithmeticException>(() => specification.Execute());
                    };

                    it["writes the assertion to the logger before bombing out"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("explodes");
                    };
                };
            };
        }
    }
}
