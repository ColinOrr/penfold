using Machine.Specifications;
using Penfold;

namespace Tests.Examples
{
    public class CalculatorFeature : Specification
    {
        public CalculatorFeature()
        {
            var calculator = new Calculator();

            comment = @"
                as a math idiot
                I want to use a calculator
                so I don't make mistakes with simple arithmetic
            ";

            Scenario["Addition"] = () =>
            {
                Given["I have pressed clear"]    = () => calculator.Clear();
                When["I key in two"]             = () => calculator.Key(2);
                When["I add three"]              = () => calculator.Add(3);
                Then["the total is five"]        = () => calculator.Total.ShouldEqual(5);
                Then["the equation history is:"] = () => log(calculator.Equation.ShouldEqual("2 + 3"));
            };

            Scenario["Division"] = () =>
            {
                Given["I have pressed clear"]    = () => calculator.Clear();
                When["I key in twelve"]          = () => calculator.Key(12);
                When["I divide by four"]         = () => calculator.Divide(4);
                Then["the total is three"]       = () => calculator.Total.ShouldEqual(3);
                Then["the equation history is:"] = () => log(calculator.Equation.ShouldEqual("12 / 4"));
            };
        }
    }
}
