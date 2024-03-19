using System;
using Bank.Domain;
using Guts.Client.Core;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("progAdvNet", "H06", "Exercise02", @"Bank.Domain\City.cs;")]
    public class CityTests
    {
        private static readonly Random Random = new Random();

        [MonitoredTest]
        public void ToString_ShouldReturnStringWithZipCodeAndName()
        {
            //Arrange
            int number = Random.Next(1, int.MaxValue);
            City city = new City
            {
                Name = "DummyCity" + number,
                ZipCode = number
            };
            
            //Act
            string result = city.ToString();

            //Assert
            Assert.That(result, Is.EqualTo($"{number} - DummyCity{number}"),
                $"Invalid result when name is '{city.Name}' and zip code is '{city.ZipCode}'.");
        }
    }
}