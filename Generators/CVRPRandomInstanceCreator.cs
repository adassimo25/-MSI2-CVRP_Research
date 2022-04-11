
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CVRP_Research.Generators
{
    public static class CVRPRandomInstanceCreator
    {
        public static CVRPProblem CreateRandomInstance(int cities, int maxDemand, int maxDistance, int vehicles, int seed = 0)
        {
            var random = new Random(seed);

            var citiesCoordinates = GenerateRandomCoordinatesOnMaxDistanceBasis(cities, maxDistance, random);

            var distances = Calculator.CalculateDistancesBetweenCities(citiesCoordinates);
            var capacity = Calculator.CalculateCapacityOnMaxDemandBasis(cities, maxDemand, vehicles);
            var demands = GenerateRandomDemandsOnMaxDemandBasis(cities, maxDemand, random);

            var name = $"RANDOM_{seed}-seed_{maxDemand}-max-demand_{maxDistance}-max-distance";

            return new(cities, distances, vehicles, capacity, demands, name);
        }

        private static Point[] GenerateRandomCoordinatesOnMaxDistanceBasis(int cities, int maxDistance, Random random)
        {
            var possibleCoordinates = new List<Point>();
            for (int i = -maxDistance; i <= maxDistance; i++)
            {
                for (int j = -maxDistance; j <= maxDistance; j++)
                {
                    possibleCoordinates.Add(new(i, j));
                }
            }

            var citiesCoordinates = new List<Point>();
            citiesCoordinates.Add(new(0, 0)); // depot
            possibleCoordinates.Remove(new(0, 0));

            for (int i = 0; i < cities; i++)
            {
                var index = random.Next(0, possibleCoordinates.Count);

                citiesCoordinates.Add(possibleCoordinates[index]);
                possibleCoordinates.RemoveAt(index);
            }

            return citiesCoordinates.ToArray();
        }

        private static int[] GenerateRandomDemandsOnMaxDemandBasis(int cities, int maxDemand, Random random)
        {
            var demands = new int[cities];

            for (int i = 0; i < demands.Length; i++)
            {
                demands[i] = random.Next(1, maxDemand + 1);
            }

            return demands;
        }
    }
}
