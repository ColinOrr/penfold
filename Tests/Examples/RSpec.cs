using NUnit.Framework;
using Penfold;
using Should;

namespace Tests.Examples
{
    [TestFixture]
    public class CalculatorSpecification : Specification
    {
        public CalculatorSpecification()
        {
            describe["Addition"] = () =>
            {
                it["adds two and three to get five"] = () =>
                {
                    new Calculator().Key(2).Add(3).Total.ShouldEqual(5);
                };

                it["adds two, three and four to get nine"] = () =>
                {
                    new Calculator().Key(2).Add(3).Add(4).Total.ShouldEqual(9);
                };
            };

            describe["Subtraction"] = () =>
            {
                it["subtracts four from six to get two"] = () =>
                {
                    new Calculator().Key(6).Subtract(4).Total.ShouldEqual(2);
                };
            };
        }
    }
}
