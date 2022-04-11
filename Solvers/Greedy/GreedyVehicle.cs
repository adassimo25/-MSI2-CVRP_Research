
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace CVRP_Research.Solvers.Greedy
{
    public class GreedyVehicle
    {
        public List<int> Cities { get; } = new();
        public int Resources { get; set; } = 0;
        public float ShortestRoute { get; private set; } = float.MaxValue;
        private Stopwatch Stopwatch { get; } = new();

        public void FindShortestRoute(TriangularMatrix distances)
        {
            var depot = 0;
            var currentRoute = 0.0f;

            ShortestRoute = float.MaxValue;

            Stopwatch.Reset();
            Stopwatch.Start();

            VisitCity(depot, Cities, currentRoute, distances);

            Stopwatch.Stop();
        }

        private void VisitCity(int city, List<int> remainingCities, float currentRoute, TriangularMatrix distances)
        {
            if (Stopwatch.ElapsedMilliseconds > Settings.Greedy.VehicleTourTimeout)
            {
                return;
            }

            if (remainingCities.Count == 0)
            {
                if (currentRoute + distances[city, 0] < ShortestRoute)
                {
                    ShortestRoute = currentRoute + distances[city, 0];
                }

                return;
            }

            foreach (var rC in remainingCities)
            {
                if (!(currentRoute + distances[city, rC] >= ShortestRoute))
                {
                    VisitCity(rC, remainingCities.Where(x => x != rC).ToList(), currentRoute + distances[city, rC], distances);
                }
            }
        }
    }
}
