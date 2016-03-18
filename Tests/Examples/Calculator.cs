
namespace Tests.Examples
{
    /// <summary>
    /// A simple calculator used to exercise the tests.
    /// </summary>
    public class Calculator
    {
        public string History { get; set; }
        public decimal Total { get; set; }

        public Calculator Key(decimal value)      { Total = value;  History = value.ToString(); return this; }
        public Calculator Add(decimal value)      { Total += value; History += " + " + value;   return this; }
        public Calculator Subtract(decimal value) { Total -= value; History += " - " + value;   return this; }
        public Calculator Multiply(decimal value) { Total *= value; History += " x " + value;   return this; }
        public Calculator Divide(decimal value)   { Total /= value; History += " / " + value;   return this; }
        public Calculator Clear()                 { Total = 0;      History = "";               return this; }
    }
}
