using Penfold;
using Should;
using System;

namespace Tests.Features
{
    public class HookSpecification : Specification
    {
        public HookSpecification()
        {
            describe["Executing a specification"] = () =>
            {
                context["with a setup step"] = () =>
                {
                    var specification = new Specification { Logger = null };
                    var executed = 0;

                    before = () =>
                    {
                        specification.before_each = () => executed++;
                        specification.before = () => { };
                        specification.it["does something"] = () => { };
                        specification.it["does something else"] = () => { };
                        specification.Execute();
                    };

                    it["runs the setup before each assertion"] = () =>
                    {
                        executed.ShouldEqual(2);
                    };
                };

                context["with a setup step before a context"] = () =>
                {
                    var specification = new Specification { Logger = null };
                    var executed = 0;

                    before = () =>
                    {
                        specification.before_each = () => executed++;
                        specification.describe["something"] = () =>
                        {
                            specification.it["does something"] = () => { };
                            specification.it["does something else"] = () => { };
                        };
                        specification.Execute();
                    };

                    it["runs the setup once before the assertions in the context"] = () =>
                    {
                        executed.ShouldEqual(1);
                    };
                };

                context["with a failing setup step"] = () =>
                {
                    var specification = new Specification { Logger = null };
                    var executed = false;
                    AggregateException exception = null;

                    before = () =>
                    {
                        specification.before_each = () => { throw new ArithmeticException(); };
                        specification.it["does something"] = () => executed = true;
                        exception = Catch<AggregateException>(() => specification.Execute());
                    };

                    it["skips the subsequent assertion"] = () =>
                    {
                        executed.ShouldBeFalse();
                    };

                    it["throws the exception"] = () =>
                    {
                        exception.InnerExceptions[0].ShouldBeType<ArithmeticException>();
                    };
                };
            };
        }
    }
}
