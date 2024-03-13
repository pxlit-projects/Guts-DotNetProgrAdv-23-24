using DartApp.Domain.Contracts;
using DartApp.AppLogic;
using System.Reflection;
using DartApp.Domain;

namespace DartApp.Tests
{
    [ExerciseTestFixture("progAdvNet", "H05", "Exercise01",
        @"DartApp.Domain\PlayerStats.cs")]
    public class PlayerStatsTests : TestBase
    {
        private Type _playerStatsType = null!;

        [SetUp]
        public void Setup()
        {
            _playerStatsType = typeof(PlayerStats);
        }

        [MonitoredTest]
        public void ShouldImplementIPlayerStats()
        {
            Assert.That(typeof(IPlayerStats).IsAssignableFrom(_playerStatsType), Is.True);
        }

        [MonitoredTest]
        public void ShouldNotHaveChangedIPlayerStats()
        {
            var filePath = @"DartApp.Domain\Contracts\IPlayerStats.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("36-63-3A-C3-96-6A-C5-59-D7-78-03-15-8B-8E-A1-69"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }


        [MonitoredTest]
        public void ShouldOnlyBeVisibleToTheDomainLayer()
        {
            Assert.That(_playerStatsType.IsNotPublic,
                "Only IPlayerStats should be visible to the other layers. The PlayerStats class itself can be encapsulated in the domain layer.");
        }

        [MonitoredTest]
        public void ShouldHaveAConstructorThatAccepts4Parameters()
        {
            int numberOf180 = 0;
            double averageThrow = 50.5;
            int bestThrow = 60;
            double averageBestThrow = 59.6;

            CreatePlayerStats(numberOf180, averageThrow, bestThrow, averageBestThrow);
        }

        [MonitoredTest]
        [SetCulture("nl-BE")]
        public void ToString_ShouldReturnInRightFormat_nlBE()
        {
            int numberOf180 = 0;
            double averageThrow = 50.5;
            int bestThrow = 60;
            double averageBestThrow = 59.6;
            IPlayerStats playerStats = CreatePlayerStats(numberOf180, averageThrow, bestThrow, averageBestThrow);

            Assert.That(playerStats.ToString(), Is.EqualTo("Average: 50,50; 180s: 0; Best throw: 60 (Average best throw: 59,60)"));
        }

        [MonitoredTest]
        [SetCulture("en-US")]
        public void ToString_ShouldReturnInRightFormat_enUS()
        {
            int numberOf180 = 0;
            double averageThrow = 50.5;
            int bestThrow = 60;
            double averageBestThrow = 59.6;
            IPlayerStats playerStats = CreatePlayerStats(numberOf180, averageThrow, bestThrow, averageBestThrow);

            Assert.That(playerStats.ToString(), Is.EqualTo("Average: 50.50; 180s: 0; Best throw: 60 (Average best throw: 59.60)"));
        }

        private IPlayerStats CreatePlayerStats(int numberOf180, double averageThrow, int bestThrow, double averageBestThrow)
        {
            ConstructorInfo? constructor = _playerStatsType
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .FirstOrDefault(c => c.IsAssembly || c.IsPublic);

            Assert.That(constructor, Is.Not.Null, "Cannot find a non-private constructor.");
            ParameterInfo[] parameters = constructor!.GetParameters();
            Assert.That(parameters.Length, Is.EqualTo(4), "Cannot find a constructor that accepts 4 parameters");

            Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(int)), "The first parameter should be an int (number of 180s).");
            Assert.That(parameters[1].ParameterType, Is.EqualTo(typeof(double)), "The second parameter should be a double (average throw).");
            Assert.That(parameters[2].ParameterType, Is.EqualTo(typeof(int)), "The third parameter should be an int (best throw score).");
            Assert.That(parameters[3].ParameterType, Is.EqualTo(typeof(double)), "The fourth parameter should be an double (average best throw).");

            try
            {
                return (IPlayerStats)constructor.Invoke(new object[] { numberOf180, averageThrow, bestThrow, averageBestThrow });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException!;
            }
        }
    }
}
