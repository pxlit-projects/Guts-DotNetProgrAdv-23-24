﻿using DartApp.Domain;
using DartApp.Domain.Contracts;
using System.Reflection;

namespace DartApp.Tests
{
    [ExerciseTestFixture("progAdvNet", "H05", "Exercise01",
        @"DartApp.Domain\GameResult.cs")]
    public class GameResultTests : TestBase
    {
        private Type _gameResultType;

        [SetUp]
        public void Setup()
        {
            _gameResultType = typeof(GameResult);
        }

        [MonitoredTest]
        public void ShouldImplementIGameResult()
        {
            Assert.That(typeof(IGameResult).IsAssignableFrom(_gameResultType), Is.True);
        }

        [MonitoredTest]
        public void ShouldNotHaveChangedIGameResult()
        {
            var filePath = @"DartApp.Domain\Contracts\IGameResult.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("A4-EB-A5-1C-9F-03-32-8F-36-9C-1B-FA-11-58-18-17"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest]
        public void ShouldHaveAPrivateParameterLessConstructorAndPrivateSettersForJsonConversionToWork()
        {
            var constructor = _gameResultType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c => c.IsPrivate);

            Assert.That(constructor, Is.Not.Null, "Cannot find a private constructor.");
            Assert.That(constructor!.GetParameters().Length, Is.Zero, "The private constructor should not have parameters.");

            AssertHasPrivateSetter(_gameResultType, nameof(IGameResult.Id));
            AssertHasPrivateSetter(_gameResultType, nameof(IGameResult.AverageThrow));
            AssertHasPrivateSetter(_gameResultType, nameof(IGameResult.BestThrow));
            AssertHasPrivateSetter(_gameResultType, nameof(IGameResult.NumberOf180));
            AssertHasPrivateSetter(_gameResultType, nameof(IGameResult.PlayerId));
        }

        [MonitoredTest]
        public void ShouldHaveAConstructorThatAccepts4Parameters()
        {
            int numberOf180 = 0;
            double averageThrow = 50.5;
            int bestThrow = 60;
            Guid playerId = Guid.NewGuid();
            CreateGameResult(numberOf180, averageThrow, bestThrow, playerId);
        }

        [MonitoredTest]
        public void Constructor_ShouldInitializeProperly()
        {
            int numberOf180 = 0;
            double averageThrow = 50.5;
            int bestThrow = 60;
            Guid playerId = Guid.NewGuid();

            IGameResult gameResult = CreateGameResult(numberOf180, averageThrow, bestThrow, playerId);

            Assert.That(gameResult.BestThrow, Is.EqualTo(bestThrow), "The 'BestThrow' is not initialized correctly.");
            Assert.That(gameResult.AverageThrow, Is.EqualTo(averageThrow), "The 'AverageThrow' is not initialized correctly.");
            Assert.That(gameResult.NumberOf180, Is.EqualTo(numberOf180), "The 'NumberOf180' is not initialized correctly.");
            Assert.That(gameResult.PlayerId, Is.EqualTo(playerId), "The 'PlayerId' is not initialized correctly.");
            Assert.That(gameResult.Id, Is.Not.EqualTo(Guid.Empty), "The constructor should generate and assign a Guid to the 'Id' property.");
        }

        [MonitoredTest]
        [TestCase(-1, 0, 0)]
        [TestCase(0, -1, 0)]
        [TestCase(0, 0, -1)]
        public void Constructor_NegativeScores_ShouldThrowArgumentException(int numberOf180, double averageThrow, int bestThrow)
        {
            Guid playerId = Guid.NewGuid();

            Assert.That(() => CreateGameResult(numberOf180, averageThrow, bestThrow, playerId), Throws.ArgumentException,
                "An 'ArgumentException' should be thrown when a score is negative.");
        }

        [MonitoredTest]
        public void Constructor_AverageScoreIsHigherThanBestScore_ShouldThrowArgumentException()
        {
            int numberOf180 = 0;
            double averageThrow = 50.5;
            int bestThrow = 49;
            Guid playerId = Guid.NewGuid();

            Assert.That(() => CreateGameResult(numberOf180, averageThrow, bestThrow, playerId), Throws.ArgumentException,
                "An 'ArgumentException' should be thrown when a the average is bigger than the best score.");
        }

        [MonitoredTest]
        public void Constructor_NumberOf180IsGreaterThan0WhileBestScoreIsLowerThan180_ShouldThrowArgumentException()
        {
            int numberOf180 = 1;
            double averageThrow = 50.5;
            int bestThrow = 179;
            Guid playerId = Guid.NewGuid();

            Assert.That(() => CreateGameResult(numberOf180, averageThrow, bestThrow, playerId), Throws.ArgumentException,
                "An 'ArgumentException' should be thrown when there was one or more " +
                "180s and the best throw is not 180.");
        }

        [MonitoredTest]
        public void Constructor_BestThrowIsGreaterThan180_ShouldThrowArgumentException()
        {
            int numberOf180 = 0;
            double averageThrow = 50.5;
            int bestThrow = 181;
            Guid playerId = Guid.NewGuid();

            Assert.That(() => CreateGameResult(numberOf180, averageThrow, bestThrow, playerId), Throws.ArgumentException,
                "An 'ArgumentException' should be thrown when the best throw is more than 180. ");
        }

        [MonitoredTest]
        [SetCulture("nl-BE")]
        public void ToString_ShouldReturnInRightFormat_nlBE()
        {
            int numberOf180 = 0;
            double averageThrow = 50.5;
            int bestThrow = 60;
            Guid playerId = Guid.NewGuid();

            IGameResult gameResult = CreateGameResult(numberOf180, averageThrow, bestThrow, playerId);

            Assert.That(gameResult.ToString(), Is.EqualTo("Average: 50,50; 180s: 0; Best: 60"));
        }

        [MonitoredTest]
        [SetCulture("en-US")]
        public void ToString_ShouldReturnInRightFormat_enUS()
        {
            int numberOf180 = 0;
            double averageThrow = 50.5;
            int bestThrow = 60;
            Guid playerId = Guid.NewGuid();

            IGameResult gameResult = CreateGameResult(numberOf180, averageThrow, bestThrow, playerId);

            Assert.That(gameResult.ToString(), Is.EqualTo("Average: 50.50; 180s: 0; Best: 60"));
        }

        private IGameResult CreateGameResult(int numberOf180, double averageThrow, int bestThrow, Guid playerId)
        {
            ConstructorInfo? constructor = _gameResultType
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .FirstOrDefault(c => c.IsAssembly || c.IsPublic);

            Assert.That(constructor, Is.Not.Null, "Cannot find a non-private constructor.");
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(parameters.Length, Is.EqualTo(4), "Cannot find a constructor that accepts 4 parameters");

            Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(Guid)), "The first parameter should be a Guid (playerId).");
            Assert.That(parameters[1].ParameterType, Is.EqualTo(typeof(int)), "The second parameter should be an int (number of 180s).");
            Assert.That(parameters[2].ParameterType, Is.EqualTo(typeof(double)), "The third parameter should be a double (average throw).");
            Assert.That(parameters[3].ParameterType, Is.EqualTo(typeof(int)), "The fourth parameter should be an int (best throw score).");

            try
            {
                return (IGameResult)constructor.Invoke(new object[] { playerId, numberOf180, averageThrow, bestThrow });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException!;
            }
        }
    }
}
