
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CVRP_Research.Generators
{
    public static class CVRPFromFileReader
    {
        public static CVRPProblem ReadFromFile(string filePath)
        {
            using (var file = new StreamReader(filePath))
            {
                (int cities, int capacity, int vehicles) = ReadCitiesCapacityVehiclesFromFile(file);
                (Point[] citiesCoordinates, int[] demands) = ReadCitiesCoordinatesDemandsFromFile(file, cities);

                var distances = Calculator.CalculateDistancesBetweenCities(citiesCoordinates);

                var name = $"{Path.GetFileNameWithoutExtension(filePath).ToUpper()}";

                file.Close();

                return new(cities, distances, vehicles, capacity, demands, name);
            }
        }

        private static (int, int, int) ReadCitiesCapacityVehiclesFromFile(StreamReader streamReader)
        {
            var ccv = streamReader.ReadSplitDataLine();

            return (int.Parse(ccv[0]), int.Parse(ccv[1]), int.Parse(ccv[2]));
        }

        private static (Point[], int[]) ReadCitiesCoordinatesDemandsFromFile(StreamReader streamReader, int cities)
        {
            var depot = streamReader.ReadSplitDataLine();

            var citiesCoordinates = new List<Point>();
            citiesCoordinates.Add(new(int.Parse(depot[0]), int.Parse(depot[1]))); // depot

            var demands = new int[cities];

            for (int i = 0; i < cities; i++)
            {
                var city = streamReader.ReadSplitDataLine();

                citiesCoordinates.Add(new(int.Parse(city[0]), int.Parse(city[1])));
                demands[i] = int.Parse(city[2]);
            }

            return (citiesCoordinates.ToArray(), demands);
        }

        private static string[] ReadSplitDataLine(this StreamReader streamReader)
            => streamReader.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);
    }
}
