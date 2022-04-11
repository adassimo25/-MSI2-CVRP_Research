
using CVRP_Research.Solvers;

namespace CVRP_Research
{
    public class CVRPProblem
    {
        public int Cities { get; private set; }
        public TriangularMatrix Distances { get; private set; }
        public int Vehicles { get; private set; }
        public int Capacity { get; private set; } // common for all vehicles
        public int[] Demands { get; private set; }

        public string Name { get; set; } = "";

        public CVRPProblem(int cities, TriangularMatrix distances, int vehicles, int capacity, int[] demands, string suffix = "")
        {
            Cities = cities;
            Distances = distances;
            Vehicles = vehicles;
            Capacity = capacity;
            Demands = demands;
            Name = $"{cities}-cities_{vehicles}-vehicles_{capacity}-capacity-{suffix}";
        }

        public float Solve(ISolver solver)
        {
            return solver.Solve(this);
        }
    }
}
