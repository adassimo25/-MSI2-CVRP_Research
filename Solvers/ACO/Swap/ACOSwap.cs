
using CVRP_Research.Solvers.ACO.Base;

namespace CVRP_Research.Solvers.ACO.Swap
{
    public class ACOSwapSolver : ACOBaseSolver
    {
        override public string Name { get; } = "ACOSwap";

        override protected AntSolution CreateAntSolution(int ant, CVRPProblem cvrp, TriangularMatrix pheromone)
        {
            var antSolution = base.CreateAntSolution(ant, cvrp, pheromone);

            if (antSolution != null)
            {
                ApplySwapHeurictic(cvrp, antSolution);
            }

            return antSolution;
        }

        private void ApplySwapHeurictic(CVRPProblem cvrp, AntSolution solution)
        {
            int[] resources = new int[solution.Routes.Length];
            for (int i = 0; i < solution.Routes.Length; i++)
            {
                for (int j = 1; j < (solution.Routes[i].Length - 1); j++)
                {
                    resources[i] += cvrp.Demands[solution.Routes[i][j] - 1];
                }
            }

            for (int i = 0; i < (solution.Routes.Length - 1); i++)
            {
                var iRoute = solution.Routes[i];

                for (int j = i + 1; j < solution.Routes.Length; j++)
                {
                    var jRoute = solution.Routes[i];

                    for (int k = 1; k < (iRoute.Length - 1); k++)
                    {
                        for (int l = 1; l < (jRoute.Length - 1); l++)
                        {
                            if ((resources[i] - cvrp.Demands[iRoute[k] - 1] + cvrp.Demands[jRoute[l] - 1] <= cvrp.Capacity)
                                && (resources[j] - cvrp.Demands[jRoute[l] - 1] + cvrp.Demands[iRoute[k] - 1] <= cvrp.Capacity))
                            {
                                var swapResult = solution.Result
                                    - cvrp.Distances[iRoute[k - 1], iRoute[k]] - cvrp.Distances[iRoute[k], iRoute[k + 1]]
                                    - cvrp.Distances[jRoute[l - 1], jRoute[l]] - cvrp.Distances[jRoute[l], jRoute[l + 1]]
                                    + cvrp.Distances[iRoute[k - 1], jRoute[l]] + cvrp.Distances[jRoute[l], iRoute[k + 1]]
                                    + cvrp.Distances[jRoute[l - 1], iRoute[k]] + cvrp.Distances[iRoute[k], jRoute[l + 1]];

                                if (swapResult < solution.Result)
                                {
                                    var tmp = iRoute[k];
                                    iRoute[k] = jRoute[l];
                                    jRoute[l] = tmp;

                                    solution.Result = swapResult;

                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
