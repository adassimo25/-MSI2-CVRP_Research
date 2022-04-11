
using System;
using System.Drawing;

namespace CVRP_Research.Generators
{
    public static class Calculator
    {
        public static TriangularMatrix CalculateDistancesBetweenCities(Point[] citiesCoordinates)
        {
            var cities = citiesCoordinates.Length;

            var distances = new TriangularMatrix(cities);

            for (int i = 0; i < cities; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    distances[i, j] = CalculateDistanceBetweenCities(citiesCoordinates[i], citiesCoordinates[j]);
                }
            }

            return distances;
        }

        public static float CalculateDistanceBetweenCities(Point p1, Point p2)
            => (float)Math.Round(Math.Sqrt((double)(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2))), 2);

        public static int CalculateCapacityOnMaxDemandBasis(int cities, int maxDemand, int vehicles)
            => 2 * (int)Math.Ceiling(((double)(cities * maxDemand)) / ((double)(vehicles))); // capacity >> maxDemand;
    }
}
