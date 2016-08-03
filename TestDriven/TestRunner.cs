using NUnitTDNet.Adapter;
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
                return base.RunMember(testListener, assembly, member.DeclaringType);
            }

            return base.RunMember(testListener, assembly, member);
        }
    }
}
