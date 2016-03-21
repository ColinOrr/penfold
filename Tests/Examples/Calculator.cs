
namespace Tests.Examples
{
    /// <summary>
    /// A simple calculator used to exercise the tests.
    /// </summary>
    public class Calculator
    {
        public string Equation { get; set; }
        public decimal Total { get; set; }

        public Calculator Key(decimal value)      { Total = value;  Equation = value.ToString(); return this; }
        public Calculator Add(decimal value)      { Total += value; Equation += " + " + value;   return this; }
        public Calculator Subtract(decimal value) { Total -= value; Equation += " - " + value;   return this; }
        public Calculator Multiply(decimal value) { Total *= value; Equation += " x " + value;   return this; }
        public Calculator Divide(decimal value)   { Total /= value; Equation += " / " + value;   return this; }
        public Calculator Clear()                 { Total = 0;      Equation = "";               return this; }
    }
}
