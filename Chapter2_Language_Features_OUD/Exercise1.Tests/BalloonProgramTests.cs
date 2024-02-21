using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Reflection;

namespace Exercise1.Tests
{
    [ExerciseTestFixture("progAdvNet", "H02", "Exercise01", @"Exercise1\BalloonProgram.cs")]
    public class BalloonProgramTests
    {
        private TypeInfo _writeDelegateTypeInfo;
        private ConstructorInfo _constructorTypeInfo;
        private BalloonProgram _program;
        private List<string> _writeLogs;
        private string _balloonProgramClassContent;
        private TypeDelegator _balloonProgramTypeInfo;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _balloonProgramTypeInfo = new TypeDelegator(typeof(BalloonProgram));

            var assembly = Assembly.GetAssembly(typeof(Program));
            _writeDelegateTypeInfo = assembly.DefinedTypes.FirstOrDefault(t =>
            {
                if (!typeof(MulticastDelegate).IsAssignableFrom(t)) return false;

                //check signature (must return void and have a string parameter)
                var methodInfo = t.DeclaredMethods.First(p => p.Name == "Invoke");
                if (methodInfo.ReturnType.Name.ToLower() != "void") return false;
                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length != 1) return false;
                if (parameters[0].ParameterType != typeof(string)) return false;
                return true;
            });

            if (_writeDelegateTypeInfo != null)
            {
                _constructorTypeInfo = _balloonProgramTypeInfo.GetConstructors().FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    if (parameters.Length != 1) return false;
                    return parameters[0].ParameterType == _writeDelegateTypeInfo.AsType();
                });

                if (_constructorTypeInfo != null)
                {

                    MethodInfo handlerMethodInfo = typeof(BalloonProgramTests).GetMethod(nameof(LogWrite), BindingFlags.NonPublic | BindingFlags.Instance);
                    Delegate writeDelegate = Delegate.CreateDelegate(_writeDelegateTypeInfo, this, handlerMethodInfo);
                    _program = (BalloonProgram)_constructorTypeInfo.Invoke(new object[] { writeDelegate });

                }
            }

            _balloonProgramClassContent = Solution.Current.GetFileContent(@"Exercise1\BalloonProgram.cs");
        }

        [SetUp]
        public void BeforeEachTest()
        {
            try
            {
                _writeLogs = new List<string>();
                if (_writeDelegateTypeInfo != null && _constructorTypeInfo != null)
                {
                    MethodInfo handlerMethodInfo = typeof(BalloonProgramTests).GetMethod(nameof(LogWrite), BindingFlags.NonPublic | BindingFlags.Instance);
                    Delegate writeDelegate = Delegate.CreateDelegate(_writeDelegateTypeInfo, this, handlerMethodInfo);
                    _program = (BalloonProgram)_constructorTypeInfo.Invoke(new object[] { writeDelegate });
                }
            }
            catch (Exception e)
            {
                TestContext.WriteLine("Error while setting up the program:");
                TestContext.WriteLine(e.ToString());
            }
        }

        [MonitoredTest("BalloonProgram - There should be a WriteDelegate type defined")]
        public void _01_ThereShouldBeAWriteDelegateTypeDefined()
        {
            AssertWriteDelegateIsDefinedCorrectly();
        }

        [MonitoredTest("BalloonProgram - Should have a constructor that accepts a WriteDelegate")]
        public void _02_ShouldHaveAConstructorThatAcceptsAWriteDelegate()
        {
            AssertConstructorIsDefinedCorrectly();
        }

        [MonitoredTest("BalloonProgram - Run - Should create and write at least 5 random balloons")]
        public void _03_Run_ShouldCreateAndWriteAtLeast5RandomBalloons()
        {
            AssertProgramIsInstantiated();

            _program.Run();

            Assert.That(_writeLogs, Has.Count.GreaterThanOrEqualTo(5), "Did not detect 5 or more writes.");
        }

        [MonitoredTest("BalloonProgram - Run - Should use NextBalloon extension method")]
        public void _04_Run_ShouldUseNextBalloonExtensionMethod()
        {
            AssertProgramIsInstantiated();
            _program.Run(); //check if the program runs without exceptions

            Assert.That(CallsMemberMethod("NextBalloon"), Is.True, "Cannot find an invocation of the 'NextBalloon' method of a 'Random' instance.");
        }

        [MonitoredTest("BalloonProgram - Run - Should use NextBalloonOfArray extension method")]
        public void _05_Run_ShouldUseNextBalloonOfArrayExtensionMethod()
        {
            AssertProgramIsInstantiated();
            _program.Run(); //check if the program runs without exceptions

            Assert.That(CallsMemberMethod("NextBalloonFromArray"), Is.True, "Cannot find an invocation of the 'NextBalloonFromArray' method of a 'Random' instance.");
        }

        [MonitoredTest("BalloonProgram - Run - Should use the WriteDelegate to write about the popped balloon")]
        public void _06_Run_ShouldUseTheWriteDelegateToWriteAboutThePoppedBalloon()
        {
            AssertProgramIsInstantiated();

            _program.Run();

            Assert.That(_writeLogs.Any(log => log.ToLower().Contains("popped")), Is.True, "Did not detect a write with the word 'popped'.");
        }

        [MonitoredTest("BalloonProgram - Should use a readonly Random field")]
        public void _07_ShouldUseAReadOnlyRandomField()
        {
            FieldInfo randomFieldInfo = _balloonProgramTypeInfo
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(f => f.FieldType == typeof(Random));

            Assert.That(randomFieldInfo, Is.Not.Null, "No private field found of type 'Random'.");
            Assert.That(randomFieldInfo.IsInitOnly, Is.True, "The field is not 'readonly'.");
        }

        private void LogWrite(string value)
        {
            _writeLogs.Add(value);
        }

        private void AssertProgramIsInstantiated()
        {
            Assert.That(_program, Is.Not.Null, "Could not create an instance of 'BalloonProgram'.");
        }

        private void AssertConstructorIsDefinedCorrectly()
        {
            AssertWriteDelegateIsDefinedCorrectly();
            Assert.IsNotNull(_constructorTypeInfo,
                "Cannot find a constructor that accepts an instance of the write delegate type");
        }

        private void AssertWriteDelegateIsDefinedCorrectly()
        {
            Assert.IsNotNull(_writeDelegateTypeInfo,
                "Cannot find a delegate type definition that supports methods that return void and accept a string parameter. " +
                "Define the type in 'WriteDelegate.cs'.");
        }

        private bool CallsMemberMethod(string methodName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_balloonProgramClassContent);
            var root = syntaxTree.GetRoot();
            return root
                .DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>()
                .Any(memberAccess => memberAccess.Name.ToString().ToLower() == methodName.ToLower());

        }
    }
}