
using System;
using System.Linq;

namespace CVRP_Research.Solvers.Greedy
{
    public class GreedySolver : ISolver
    {
        public string Name { get; } = "Greedy";

        public float Solve(CVRPProblem cvrp)
        {
            var greedyCities = CreateGreedyCities(cvrp.Demands);
            Array.Sort(greedyCities, (x, y) => y.Demand.CompareTo(x.Demand));

            var greedyVehicles = CreateGreedyVehicles(greedyCities, cvrp.Vehicles, cvrp.Capacity);

            if (greedyVehicles == null)
            {
                return -1;
            }

            var result = 0.0f;
            foreach (var gV in greedyVehicles)
            {
                gV.FindShortestRoute(cvrp.Distances);
                result += gV.ShortestRoute;
            }

            return result;
        }

        private GreedyCity[] CreateGreedyCities(int[] demands)
        {
            return demands.Select((d, i) => new GreedyCity { Index = i + 1, Demand = d }).ToArray();
        }

        private GreedyVehicle[] CreateGreedyVehicles(GreedyCity[] cities, int vehicles, int capacity)
        {
            var greedyVehicles = new GreedyVehicle[vehicles].Select(x => new GreedyVehicle()).ToArray();

            for (int i = 0; i < cities.Length; i++)
            {
                var index = GetLeastStackedVehicleIndex(greedyVehicles, cities[i].Demand, capacity);
                if (index == -1)
                {
                    return null;
                }

                UpdateVehicle(greedyVehicles[index], cities[i]);
                SortVehiclesFromLastModifiedIndex(greedyVehicles, index);
            }

            return greedyVehicles;
        }

        private int GetLeastStackedVehicleIndex(GreedyVehicle[] vehicles, int demand, int capacity)
        {
            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i].Resources + demand <= capacity)
                {
                    return i;
                }
            }

            return -1;
        }

        private void UpdateVehicle(GreedyVehicle vehicle, GreedyCity city)
        {
            vehicle.Cities.Add(city.Index);
            vehicle.Resources += city.Demand;
        }

        private void SortVehiclesFromLastModifiedIndex(GreedyVehicle[] vehicles, int index)
        {
            for (int i = index; i < (vehicles.Length - 1); i++)
            {
                var citiesLeft = vehicles[i].Cities.Count;
                var citiesRight = vehicles[i + 1].Cities.Count;

                var demandLeft = vehicles[i].Resources;
                var demandRight = vehicles[i + 1].Resources;

                if (citiesRight < citiesLeft || (citiesRight == citiesLeft && demandRight < demandLeft))
                {
                    var tmpVehicle = vehicles[i];
                    vehicles[i] = vehicles[i + 1];
                    vehicles[i + 1] = tmpVehicle;
                }
                else
                {
                    return;
                }
            }
        }
    }
}
