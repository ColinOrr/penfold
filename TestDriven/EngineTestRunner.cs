using NUnit.Engine;
using System;
using System.Reflection;
using System.Xml;
using TDF = TestDriven.Framework;
using NUnit.Engine.Services;

namespace Penfold.TestDriven
{
    public class EngineTestRunner : TDF.ITestRunner
    {
        TestEngine engine;

        public EngineTestRunner()
        {
            engine = new TestEngine();
            engine.Services.Add(new SettingsService(false)); // Might not be required.
            engine.Services.Add(new ExtensionService());

            engine.Services.Add(new InProcessTestRunnerFactory());
            engine.Services.Add(new DriverService());

            engine.Services.Add(new TestFilterService());
            engine.Services.Add(new ProjectService());

            engine.Services.ServiceManager.StartServices();
        }

        public TDF.TestRunState RunAssembly(TDF.ITestListener testListener, Assembly assembly)
        {
            return run(testListener, assembly, null);
        }

        public TDF.TestRunState RunMember(TDF.ITestListener testListener, Assembly assembly, MemberInfo member)
        {
            var where = Utilities.GetWhereForTarget(assembly, member);
            if (string.IsNullOrEmpty(where))
            {
                return TDF.TestRunState.NoTests;
            }

            return run(testListener, assembly, where);
        }

        public TDF.TestRunState RunNamespace(TDF.ITestListener testListener, Assembly assembly, string ns)
        {
            var where = Utilities.GetWhereForTarget(assembly, ns);
            return run(testListener, assembly, where);
        }

        TDF.TestRunState run(TDF.ITestListener testListener, Assembly testAssembly, string where)
        {
            string assemblyFile = new Uri(testAssembly.EscapedCodeBase).LocalPath;
            TestPackage package = new TestPackage(assemblyFile);

            package.AddSetting("ProcessModel", "InProcess");
            package.AddSetting("DomainUsage", "None");

            ITestRunner runner = engine.GetRunner(package);

            var filterService = engine.Services.GetService<ITestFilterService>();
            ITestFilterBuilder builder = filterService.GetTestFilterBuilder();
            if (!string.IsNullOrEmpty(where))
            {
                builder.SelectWhere(where);
            }

            var filter = builder.GetFilter();

            var testRunnerName = getTestRunnerName(testAssembly);
            var eventHandler = new TestEventListener(testListener, testRunnerName);

            XmlNode result = runner.Run(eventHandler, filter);
            return eventHandler.TestRunState;
        }

        static string getTestRunnerName(Assembly testRunnerName)
        {
            foreach (var assemblyName in testRunnerName.GetReferencedAssemblies())
            {
                if (assemblyName.Name == "nunit.framework")
                {
                    return getTestRunnerName(assemblyName);
                }
            }

            return "NUnit - Unknown Version";
        }

        static string getTestRunnerName(AssemblyName assemblyName)
        {
            var version = assemblyName.Version;
            return string.Format("NUnit {0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }


        public class TestEventListener : ITestEventListener
        {
            TDF.ITestListener testListener;
            string testRunnerName;

            internal TDF.TestRunState TestRunState
            {
                get; private set;
            }

            internal int TotalTests
            {
                get; private set;
            }

            public TestEventListener(TDF.ITestListener testListener, string testRunnerName)
            {
                this.testListener = testListener;
                this.testRunnerName = testRunnerName;
                TestRunState = TDF.TestRunState.Success;
            }

            public void OnTestEvent(string report)
            {
                var doc = new XmlDocument();
                doc.LoadXml(report);
                var element = doc.DocumentElement;
                if (element == null)
                {
                    return;
                }

                // Don't output results at assembly level.
                var type = element.GetAttribute("type");
                if (type == "Assembly")
                {
                    return;
                }

                switch (element.Name)
                {
                    case "start-run":
                        processStartRun(element);
                        break;
                    case "test-case":
                        processOutput(element);
                        processTest(element, report, true);
                        break;
                    case "test-suite":
                        processOutput(element);
                        processTest(element, report, false);
                        break;
                    case "test-output":
                        processTestOutput(element);
                        break;
                    case "test-run":
                        // Don't process output.
                        break;
                }
            }

            void processStartRun(XmlElement element)
            {
                var countValue = element.GetAttribute("count");
                if (countValue == null)
                {
                    return;
                }

                int count;
                if (int.TryParse(countValue, out count))
                {
                    TotalTests = count;
                    if (count == 0)
                    {
                        TestRunState = TDF.TestRunState.NoTests;
                    }
                }
            }

            void processTest(XmlElement element, string report, bool isTestCase)
            {
                var testResult = new TDF.TestResult();
                testResult.TotalTests = TotalTests;
                testResult.TestRunnerName = testRunnerName;

                testResult.Name = element.GetAttribute("fullname");

                var messageElement = element.SelectSingleNode("//message");
                if (messageElement != null)
                {
                    var text = trimNewLine(messageElement.InnerText);
                    testResult.Message = text;
                }

                var stackTraceElement = element.SelectSingleNode("//stack-trace");
                if (stackTraceElement != null)
                {
                    testResult.StackTrace = stackTraceElement.InnerText;
                }

                var result = element.GetAttribute("result");

                TDF.TestState state;
                switch (result)
                {
                    case "Failed":
                        state = TDF.TestState.Failed;
                        TestRunState = TDF.TestRunState.Failure;
                        break;
                    case "Passed":
                        state = TDF.TestState.Passed;
                        break;
                    case "Skipped":
                    case "Inconclusive":
                        state = TDF.TestState.Ignored;
                        break;
                    default:
                        state = TDF.TestState.Failed;
                        testListener.WriteLine("Unknown 'result': " + result + "\n" + report, TDF.Category.Error);
                        break;
                }

                testResult.State = state;

                if (isTestCase)
                {
                    testListener.TestFinished(testResult);
                    return;
                }

                // Fake failed test when fixture contains stack trace info.
                if (!isTestCase && testResult.StackTrace != null)
                {
                    testListener.TestFinished(testResult);
                    return;
                }
            }

            void processOutput(XmlElement element)
            {
                var output = element.SelectSingleNode("//output");
                if (output != null)
                {
                    var text = trimNewLine(output.InnerText);
                    testListener.WriteLine(text, TDF.Category.Output);
                }
            }

            void processTestOutput(XmlElement element)
            {
                var text = trimNewLine(element.InnerText);
                testListener.WriteLine(text, TDF.Category.Output);
            }

            static string trimNewLine(string text)
            {
                var newLine = Environment.NewLine;
                if (text.EndsWith(newLine))
                {
                    text = text.Substring(0, text.Length - newLine.Length);
                }

                return text;
            }
        }
    }
}
