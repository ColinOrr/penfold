using Machine.Specifications;
using NUnit.Framework;
using Penfold;

namespace Tests.Examples
{
    [TestFixture]
    public class CalculatorSpecification : Specification
    {
        public CalculatorSpecification()
        {
            Calculator calculator = null;
            before = () => calculator = new Calculator();

            describe["Addition"] = () =>
            {
                context["adding two and three"] = () =>
                {
                    before = () => calculator.Key(2).Add(3);
                    it["sets the total to five"] = () => calculator.Total.ShouldEqual(5);
                };

                context["adding two, three and four"] = () =>
                {
                    before = () => calculator.Key(2).Add(3).Add(4);
                    it["sets the total to nine"] = () => calculator.Total.ShouldEqual(9);
                };
            };

            describe["Subtraction"] = () =>
            {
                context["subtracting four from six"] = () =>
                {
                    before = () => calculator.Key(6).Subtract(4).Total.ShouldEqual(2);
                    it["sets the total to two"] = () => calculator.Total.ShouldEqual(2);
                };
            };

            after = () => calculator.Clear();
        }
    }
}
