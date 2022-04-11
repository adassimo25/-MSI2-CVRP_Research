
using System.Linq;
using System;
using System.Collections.Generic;

namespace CVRP_Research.Solvers.ACO.Base
{
    public class ACOBaseSolver : ISolver
    {
        public int Alpha { get; set; } = Settings.ACO.Alpha; // pheromone priority
        public int Beta { get; set; } = Settings.ACO.Beta; // heuristics priority
        public float Phi { get; set; } = Settings.ACO.Phi; // pheromone evaporation factor
        public float Q { get; set; } = Settings.ACO.Q; // pheromone update factor

        public int AntsFactor { get; set; } = Settings.ACO.AntsFactor;
        public int Iterations { get; set; } = Settings.ACO.Iterations;

        private Random random = new();

        virtual public string Name { get; } = "ACOBase";

        public float Solve(CVRPProblem cvrp)
        {
            var initPheromone = 1.0f;
            var pheromone = new TriangularMatrix(cvrp.Cities + 1, initPheromone);

            var bestSolution = new AntSolution();

            for (int i = 0; i < Iterations; i++)
            {
                var iterationSolutions = new List<AntSolution>();

                for (int ant = 0; ant < (AntsFactor * cvrp.Cities); ant++)
                {
                    var antSolution = CreateAntSolution(ant, cvrp, pheromone);

                    if (antSolution != null)
                    {
                        iterationSolutions.Add(antSolution);

                        if (antSolution.Result < bestSolution.Result)
                        {
                            bestSolution = antSolution;
                        }
                    }
                }

                EvaporatePheromone(pheromone);
                UpdatePheromone(pheromone, iterationSolutions);
            }

            return bestSolution.Result;
        }

        virtual protected AntSolution CreateAntSolution(int ant, CVRPProblem cvrp, TriangularMatrix pheromone)
        {
            var antSolution = new AntSolution();
            antSolution.Result = 0.0f;

            var antRoutes = new List<int[]>();

            var currentResources = 0;
            var currentResult = 0.0f;
            var currentRoute = new List<int>();
            var currentCity = ant % cvrp.Cities + 1; // first city for ant

            var remainingCities = Enumerable.Range(1, cvrp.Cities).ToList();

            currentResult += cvrp.Distances[0, currentCity];
            currentRoute.Add(0);
            currentRoute.Add(currentCity);
            remainingCities.Remove(currentCity);

            while (remainingCities.Count != 0)
            {
                var possibleCities = remainingCities
                    .Where(x => cvrp.Demands[x - 1] + currentResources <= cvrp.Capacity).ToArray();

                var nextCity = ChooseNextCity(currentCity, possibleCities, cvrp.Distances, pheromone);

                if (nextCity != 0)
                {
                    remainingCities.Remove(nextCity);
                    currentResources += cvrp.Demands[nextCity - 1];
                }

                currentResult += cvrp.Distances[currentCity, nextCity];
                currentRoute.Add(nextCity);

                currentCity = nextCity;

                if (nextCity == 0) // not all cities visited
                {
                    currentResources = 0;

                    antSolution.Result += currentResult;
                    currentResult = 0.0f;

                    antRoutes.Add(currentRoute.ToArray());
                    currentRoute = new List<int>() { 0 };

                    if (antRoutes.Count == cvrp.Vehicles && remainingCities.Count != 0) // too many vehicles used
                    {
                        return null;
                    }
                }

                if (nextCity != 0 && remainingCities.Count == 0) // all cities visited
                {
                    currentResult += cvrp.Distances[nextCity, 0];
                    currentRoute.Add(0);

                    antSolution.Result += currentResult;
                    antRoutes.Add(currentRoute.ToArray());
                }
            }

            antSolution.Routes = antRoutes.ToArray();

            return antSolution;
        }

        private int ChooseNextCity(int fromCity, int[] possibleCities,
            TriangularMatrix distances, TriangularMatrix pheromone)
        {
            if (possibleCities.Length == 0)
            {
                return 0;
            }

            var nextCity = 0;

            var total = 0.0f;
            for (int i = 0; i < possibleCities.Length; i++)
            {
                total += (float)(Math.Pow(pheromone[fromCity, possibleCities[i]], Alpha)
                    * Math.Pow(1.0f / distances[fromCity, possibleCities[i]], Beta));
            }

            var randomValue = random.NextDouble() * total;

            var subtotal = 0.0f;
            for (int i = 0; i < possibleCities.Length; i++)
            {
                var cityOdds = (float)(Math.Pow(pheromone[fromCity, possibleCities[i]], Alpha)
                    * Math.Pow(1.0f / distances[fromCity, possibleCities[i]], Beta));
                if (randomValue <= subtotal + cityOdds)
                {
                    nextCity = possibleCities[i];
                    break;
                }

                subtotal += cityOdds;
            }

            return nextCity;
        }

        private void EvaporatePheromone(TriangularMatrix pheromone)
        {
            for (int i = 0; i < pheromone.Size; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    pheromone[i, j] *= Phi;
                }
            }
        }

        private void UpdatePheromone(TriangularMatrix pheromone, List<AntSolution> solutions)
        {
            foreach (var solution in solutions)
            {
                for (int i = 0; i < solution.Routes.Length; i++)
                {
                    for (int j = 1; j < solution.Routes[i].Length; j++)
                    {
                        pheromone[solution.Routes[i][j - 1], solution.Routes[i][j]] += Q / solution.Result;
                    }
                }
            }
        }
    }
}
