using Machine.Specifications;
using NUnit.Framework;
using Penfold;
using System;

namespace Tests.Examples
{
    [TestFixture]
    public class CalculatorSpecification : Specification
    {
        public CalculatorSpecification()
        {
            var calculator = new Calculator();

            before_each = () => calculator.Clear();

            describe["Addition"] = () =>
            {
                context["adding two and three"] = () =>
                {
                    before = () => calculator.Key(2).Add(3);

                    it["sets the total to five"] = () =>
                    {
                        calculator.Total.ShouldEqual(5);
                    };

                    it["sets the equation history to:"] = () =>
                    {
                        log(calculator.Equation.ShouldEqual("2 + 3"));
                    };
                };

                context["adding two, three and four"] = () =>
                {
                    before = () => calculator.Key(2).Add(3).Add(4);

                    it["sets the total to nine"] = () =>
                    {
                        calculator.Total.ShouldEqual(9);
                    };

                    it["sets the equation history to:"] = () =>
                    {
                        log(calculator.Equation.ShouldEqual("2 + 3 + 4"));
                    };
                };
            };

            describe["Division"] = () =>
            {
                context["dividing four from twelve"] = () =>
                {
                    before = () => calculator.Key(12).Divide(4);

                    it["sets the total three"] = () =>
                    {
                        calculator.Total.ShouldEqual(3);
                    };

                    it["sets the equation history to:"] = () =>
                    {
                        log(calculator.Equation.ShouldEqual("12 / 4"));
                    };
                };

                context["dividing by zero"] = () =>
                {
                    it["explodes 💣💥☠️"] = () =>
                    {
                        Catch(() => calculator.Key(2).Divide(0)).ShouldBeOfExactType<DivideByZeroException>();
                    };
                };
            };
        }
    }
}
