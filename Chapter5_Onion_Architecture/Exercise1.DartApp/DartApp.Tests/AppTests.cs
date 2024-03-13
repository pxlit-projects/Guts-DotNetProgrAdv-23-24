using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using DartApp.Infrastructure.Storage;
using DartApp.AppLogic;
using DartApp.AppLogic.Contracts;
using DartApp.Presentation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace DartApp.Tests
{
    [Apartment(ApartmentState.STA)]
    [ExerciseTestFixture("progAdvNet", "H05", "Exercise01",
        @"DartApp.Presentation\App.xaml.cs")]
    public class AppTests
    {
        private string _appClassContent = null!;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _appClassContent = Solution.Current.GetFileContent(@"DartApp.Presentation\App.xaml.cs");
        }

        [MonitoredTest]
        public void ShouldUseDependencyInjection()
        {
            var appType = typeof(App);

            FieldInfo? servicesFieldInfo = appType.GetField("_serviceProvider", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(servicesFieldInfo, Is.Not.Null, "The App class should have a private field '_serviceProvider'");

            var app = new App();
            IServiceProvider? serviceProvider = servicesFieldInfo!.GetValue(app) as ServiceProvider;
            Assert.That(serviceProvider, Is.Not.Null,
                "The _serviceProvider field should be of type 'ServiceProvider' and be initialized in the constructor of the App class.");

            PlayerFileRepository? playerRepo = serviceProvider!.GetService<IPlayerRepository>() as PlayerFileRepository;
            Assert.That(playerRepo, Is.Not.Null,
                "The IPlayerRepository should be registered in the service collection. " +
                "The repository returned should be of type 'PlayerFileRepository'.");


            string tip = "\nTip: use the overload of 'AddTransient' that takes a Func<IServiceProvider, TResult> to register the IPlayerRepository in the service collection. " +
                         "You can use a lambda expression that creates the player data folder string and returns a new instance of PlayerFileRepository.";

            Assert.That(_appClassContent, Contains.Substring("Environment.GetFolderPath"),
                "The folder to save players in, should be in the special 'AppData' directory. " +
                "Use the 'Environment' class to retrieve the path of that directory." + tip);

            Assert.That(_appClassContent, Contains.Substring("Environment.SpecialFolder.ApplicationData"),
                "The folder to save players in, should be in the special 'AppData' directory. Use the 'Environment.SpecialFolder' enum." + tip);

            Assert.That(_appClassContent, Contains.Substring("Path.Combine(").And.Contains(@"""DartApp"")"),
                "The folder to save players in, should be a subdirectory 'DartApp' in the special 'AppData' directory. " +
                "Use the static 'Combine' method of the 'System.IO.Path' class to create a string that holds the complete directory path." + tip);

            PlayerService? playerService = serviceProvider!.GetService<IPlayerService>() as PlayerService;
            Assert.That(playerService, Is.Not.Null,
                "The IPlayerService should be registered in the service collection. " +
                "The service returned should be of type 'PlayerService'.");

            MainWindow? mainWindow = serviceProvider!.GetService<MainWindow>();
            Assert.That(mainWindow, Is.Not.Null,
                "The MainWindow should be registered in the service collection.");

            Assert.That(_appClassContent, Does.Not.Contain("new MainWindow"),
                "Do not create a new instance of MainWindow yourself, let the DI container do it behind the scenes.");
            Assert.That(_appClassContent, Does.Not.Contain("new PlayerService"),
                "Do not create a new instance of PlayerService yourself, let the DI container do it behind the scenes.");
        }

        [MonitoredTest]
        public void OnStartup_ShouldRetrieveAnInstanceOfMainWindowFromTheDependencyInjectionContainerAndShowIt()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(_appClassContent);
            var root = syntaxTree.GetRoot();
            MethodDeclarationSyntax? onStartupMethod = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(md => md.Identifier.ValueText.Equals("OnStartup"));

            Assert.That(onStartupMethod, Is.Not.Null, "Cannot find a method 'OnStartup' in App.xaml.cs");

            List<string> methodInvocations = onStartupMethod!.DescendantNodes().OfType<InvocationExpressionSyntax>().Select(i => i.Expression.ToString()).ToList();

            string? getRequiredServiceInvocation = methodInvocations.FirstOrDefault(m => m.Contains(".GetRequiredService<MainWindow>"));
            Assert.That(getRequiredServiceInvocation, Is.Not.Null,
                "Cannot find a statement in 'OnStartup' where the MainWindow is retrieved from the service provider (using the GetRequiredService method).");

            string? showInvocation = methodInvocations.FirstOrDefault(m => m.Contains(".Show"));
            Assert.That(showInvocation, Is.Not.Null,
                "Cannot find a statement in 'OnStartup' where the MainWindow is shown.");
        }
    }
}
