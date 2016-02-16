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

                    it["runs the setup before each step"] = () =>
                    {
                        executed.ShouldEqual(3);
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
                    var executed = 0;
                    AggregateException exception = null;

                    before = () =>
                    {
                        specification.Background = () => { throw new ArithmeticException(); };
                        specification.When["I do something"] = () => executed++;
                        specification.Then["it says done"] = () => executed++;
                        exception = Catch<AggregateException>(() => specification.Execute());
                    };

                    it["skips the subsequent activities and assertions"] = () =>
                    {
                        executed.ShouldEqual(0);
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
