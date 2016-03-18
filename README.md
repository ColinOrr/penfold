# penfold
Yet another BDD specification library for .NET

[![Build status][1]][2]
[![NuGet version][3]][4]

--------------------------------------------------------------------------------

Penfold can be installed from NuGet. Run the following commands from the Package 
Manager Console:

```bash
PM> Install-Package penfold
```

Penfold runs on NUnit so it should already work with your favourite test runner.
Create a class that inherits from `Specification` then write your specification
in the constructor:

```c#
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

                it["sets the history to:"] = () =>
                {
                    log(calculator.History.ShouldEqual("2 + 3"));
                };
            };
        };
    }
}
```
Penfold supports both RSpec style specifications and Gherkin:
```c#
[TestFixture]
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
            Given["I have pressed clear"] = () => calculator.Clear();
            When["I key in two"]          = () => calculator.Key(2);
            When["I add three"]           = () => calculator.Add(3);
            Then["the total is five"]     = () => calculator.Total.ShouldEqual(5);
            Then["the history is:"]       = () => log(calculator.History.ShouldEqual("2 + 3"));
        };
    }
}
```

Have fun 😀

[1]: https://ci.appveyor.com/api/projects/status/flox08f7kiln80co?svg=true
[2]: https://ci.appveyor.com/project/ColinOrr/penfold
[3]: https://badge.fury.io/nu/penfold.svg
[4]: https://www.nuget.org/packages/penfold
