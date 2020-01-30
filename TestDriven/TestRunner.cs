using System.Reflection;
using TestDriven.Framework;

namespace Penfold.TestDriven
{
    public class TestRunner : EngineTestRunner, ITestRunner
    {
        public new TestRunState RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Constructor)
            {
                var where  = "(cat == '.current')";
                var result = this.Call<TestRunState>("run", testListener, assembly, where);
                if (result != TestRunState.NoTests) return result;

                return base.RunMember(testListener, assembly, member.DeclaringType);
            }

            return base.RunMember(testListener, assembly, member);
        }
    }
}
