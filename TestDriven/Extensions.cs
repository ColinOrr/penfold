using System.Linq;
using System.Reflection;

namespace Penfold.TestDriven
{
    public static class Extensions
    {
        /// <summary>
        /// Invokes a private or protected method by using reflection.
        /// </summary>
        public static T Call<T>(this object obj, string methodName, params object[] parameters)
        {
            var types = parameters.Select(p => p.GetType()).ToArray();

            var method = obj
                .GetType().BaseType
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, null, types, null);

            return (T)method.Invoke(obj, parameters);
        }
    }
}
