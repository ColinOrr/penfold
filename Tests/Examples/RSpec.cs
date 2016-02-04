using NUnit.Framework;
using Penfold;
using Should;
using Tests.Models;

namespace Tests.Examples
{
    [TestFixture]
    public class CalculatorSpecification : Specification
    {
        public CalculatorSpecification()
        {
            it["adds two and three to get five"] = () =>
            {
                new Calculator().Key(2).Add(3).Total.ShouldEqual(5);
            };

            it["adds two, three and four to get nine"] = () =>
            {
                new Calculator().Key(2).Add(3).Add(4).Total.ShouldEqual(9);
            };
        }
    }
}
