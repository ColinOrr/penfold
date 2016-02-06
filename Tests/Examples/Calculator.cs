
namespace Tests.Examples
{
    /// <summary>
    /// A simple calculator used to exercise the tests.
    /// </summary>
    public class Calculator
    {
        public decimal Total { get; set; }

        public Calculator Key(decimal value)      { Total = value;  return this; }
        public Calculator Add(decimal value)      { Total += value; return this; }
        public Calculator Subtract(decimal value) { Total -= value; return this; }
        public Calculator Clear()                 { Total = 0;      return this; }
    }
}
