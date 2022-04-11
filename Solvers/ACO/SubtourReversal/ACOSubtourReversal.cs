
using System;
using CVRP_Research.Solvers.ACO.Base;

namespace CVRP_Research.Solvers.ACO.SubtourReversal
{
    public class ACOSubtourReversalSolver : ACOBaseSolver
    {
        override public string Name { get; } = "ACOSubtourReversal";

        override protected AntSolution CreateAntSolution(int ant, CVRPProblem cvrp, TriangularMatrix pheromone)
        {
            var antSolution = base.CreateAntSolution(ant, cvrp, pheromone);

            if (antSolution != null)
            {
                ApplySubtourReversalHeurictic(cvrp, antSolution);
            }

            return antSolution;
        }

        private void ApplySubtourReversalHeurictic(CVRPProblem cvrp, AntSolution solution)
        {
            for (int i = 0; i < solution.Routes.Length; i++)
            {
                var route = solution.Routes[i];

                for (int diff = 1; diff < (route.Length - 3); diff++)
                {
                    for (int l = 1; l < (route.Length - 1 - diff); l++)
                    {
                        var r = l + diff;

                        var swapResult = solution.Result
                            - cvrp.Distances[route[l - 1], route[l]] - cvrp.Distances[route[r], route[r + 1]]
                            + cvrp.Distances[route[l - 1], route[r]] + cvrp.Distances[route[l], route[r + 1]];

                        if (swapResult < solution.Result
                            || (swapResult == solution.Result && (new Random().Next() % 2 == 1)))
                        {
                            for (int j = 0; j < ((r - l + 1) / 2); j++)
                            {
                                var tmp = route[l + j];
                                route[l + j] = route[r - j];
                                route[r - j] = tmp;
                            }

                            solution.Result = swapResult;

                            return;
                        }
                    }
                }
            }
        }
    }
}
