
namespace CVRP_Research.Solvers
{
    public interface ISolver
    {
        public string Name { get; }
        public float Solve(CVRPProblem cvrp);
    }
}
