using Machine.Specifications;
using Penfold;
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

                    it["runs the code in the assertion"] = () =>
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

                    it["runs the code in each assertion"] = () =>
                    {
                        executed.ShouldEqual(2);
                    };

                    it["writes each assertion to the logger"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("does something");
                        specification.Logger.ToString().ShouldContain("does something else");
                    };
                };

                context["with a failing assertion"] = () =>
                {
                    var specification = new Specification { Logger = new StringWriter() };
                    AggregateException exception = null;

                    before = () =>
                    {
                        specification.it["explodes"] = () => { throw new ArithmeticException(); };
                        exception = Catch<AggregateException>(() => specification.Execute());
                    };

                    it["writes the assertion to the logger before bombing out"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("explodes");
                    };

                    it["throws the exception"] = () =>
                    {
                        exception.InnerExceptions[0].ShouldBeOfExactType<ArithmeticException>();
                    };
                };
            };
        }
    }
}
