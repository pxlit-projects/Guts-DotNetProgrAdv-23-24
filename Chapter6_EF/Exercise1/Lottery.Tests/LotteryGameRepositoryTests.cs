﻿using Guts.Client.Core;
using Lottery.Domain;
using Lottery.Infrastructure;

namespace Lottery.Tests
{
    [ExerciseTestFixture("progAdvNet", "H06", "Exercise01", @"Lottery.Infrastructure\LotteryGameRepository.cs")]
    internal class LotteryGameRepositoryTests : DatabaseTests
    {
        [MonitoredTest]
        public void GetAll_ShouldReturnAllGamesFromDb()
        {
            //Arrange
            var someGame = new LotteryGameBuilder().Build();
            var originalAmountOfGames = 0;

            using (var context = CreateDbContext())
            {
                originalAmountOfGames = context.Set<LotteryGame>().Count();

                context.Add(someGame);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repo = new LotteryGameRepository(context);

                //Act
                var allGames = repo.GetAll();

                //Assert
                Assert.That(allGames, Has.Count.EqualTo(originalAmountOfGames + 1), () => "Not all games in the database are returned.");
                var expectedGame = allGames.FirstOrDefault(game => game.Name == someGame.Name);
                Assert.That(expectedGame, Is.Not.Null, () => "Not all games in the database are returned.");
            }
        }
    }
}