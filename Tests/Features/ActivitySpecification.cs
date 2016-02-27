using Machine.Specifications;
using Penfold;
using System;
using System.IO;

namespace Tests.Features
{
    public class ActivitySpecification : Specification
    {
        public ActivitySpecification()
        {
            describe["Executing a specification"] = () =>
            {
                context["with multiple activities"] = () =>
                {
                    var specification = new MySpecification { Logger = new StringWriter() };
                    var executed = 0;

                    before = () =>
                    {
                        specification.Given["I do one activity"] = () => executed++;
                        specification.When["I do another activity"] = () => executed++;
                        specification.Then["I'm very tired"] = () => { };
                        specification.Execute();
                    };

                    it["runs the code in each activity"] = () =>
                    {
                        executed.ShouldEqual(2);
                    };

                    it["writes each activity to the logger"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("Given I do one activity");
                        specification.Logger.ToString().ShouldContain("When I do another activity");
                    };
                };

                context["with a failing activity"] = () => 
                {
                    var specification = new MySpecification { Logger = new StringWriter() };
                    var executed = 0;

                    before = () =>
                    {
                        specification.Given["An explosion"] = () => { throw new ArithmeticException(); };
                        specification.When["I do another activity"] = () => executed++;
                        specification.Then["I'm very tired"] = () => executed++;
                        Catch(() => specification.Execute());
                    };

                    it["writes the activity to the logger before bombing out"] = () =>
                    {
                        specification.Logger.ToString().ShouldContain("An explosion");
                    };

                    it["skips the subsequent activities and assertions"] = () =>
                    {
                        executed.ShouldEqual(0);
                    };
                };
            };
        }
    }
}
