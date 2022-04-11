
using System.IO;
using System.Diagnostics;
using System;
using CVRP_Research.Generators;
using CVRP_Research.Solvers.Greedy;
using CVRP_Research.Solvers.ACO.Base;
using CVRP_Research.Solvers.ACO.Swap;
using CVRP_Research.Solvers.ACO.SubtourReversal;
using System.Collections.Generic;
using CVRP_Research.Solvers;
using IronXL;

namespace CVRP_Research
{
    class Program
    {
        static void Main(string[] args)
        {
            var problems = new List<CVRPProblem>()
                {
                    CVRPRandomInstanceCreator.CreateRandomInstance(15, 30, 30, 3, 1),
                    CVRPRandomInstanceCreator.CreateRandomInstance(20, 40, 40, 4, 2),
                    CVRPRandomInstanceCreator.CreateRandomInstance(25, 50, 50, 5, 3),
                    CVRPFromFileReader.ReadFromFile("./Datasets/Modified/vrpnc1.txt"),
                    CVRPFromFileReader.ReadFromFile("./Datasets/Modified/vrpnc2.txt"),
                    CVRPFromFileReader.ReadFromFile("./Datasets/Modified/vrpnc3.txt")
                };

            Directory.CreateDirectory(Settings.Results.Directory);

            WorkBook results = new WorkBook();

            WorkSheet ht123 = results.CreateWorkSheet(Settings.Results.WorksheetHypothesis123);
            RunBaseSolversManyTimes(problems, ht123);

            WorkSheet ht4 = results.CreateWorkSheet(Settings.Results.WorksheetHypothesis4);
            RunACOSolversWithDifferentPriorities(problems, ht4);

            WorkSheet ht5 = results.CreateWorkSheet(Settings.Results.WorksheetHypothesis5);
            RunACOSolversWithDifferentAntsFactors(problems, ht5);

            var file = $"{Settings.Results.Directory}/{Settings.Results.FileName}"
                + "_" + $"{DateTime.Now.ToString(Settings.Results.FileDateFormat)}.{Settings.Results.FileExtension}";
            results.SaveAs(file);
        }

        private static void RunBaseSolversManyTimes(List<CVRPProblem> problems, WorkSheet ws)
        {
            var solvers = new List<ISolver>()
            {
                new GreedySolver(),
                new ACOBaseSolver(),
                new ACOSubtourReversalSolver(),
                new ACOSwapSolver()
            };

            var row = 1;

            ws[$"A{row}"].Value = "PROBLEM";
            ws[$"A{row}"].Style.Font.Bold = true;
            ws[$"B{row}"].Value = "SOLVER";
            ws[$"B{row}"].Style.Font.Bold = true;
            ws[$"C{row}"].Value = "RESULT";
            ws[$"C{row}"].Style.Font.Bold = true;
            ws[$"D{row}"].Value = "TIME";
            ws[$"D{row}"].Style.Font.Bold = true;

            row++;

            var stopwatch = new Stopwatch();

            foreach (var problem in problems)
            {
                foreach (var solver in solvers)
                {
                    for (int i = 0; i < Settings.Computations.Repeats; i++)
                    {
                        ws[$"A{row}"].Value = problem.Name;
                        ws[$"B{row}"].Value = solver.Name;

                        stopwatch.Reset();
                        stopwatch.Start();

                        var result = problem.Solve(solver);

                        stopwatch.Stop();

                        ws[$"C{row}"].Value = Math.Round(result, 2);
                        ws[$"D{row}"].Value = stopwatch.ElapsedMilliseconds;

                        row++;
                    }
                }
            }
        }

        private static void RunACOSolversWithDifferentPriorities(List<CVRPProblem> problems, WorkSheet ws)
        {
            var priorities = new List<int>() { 2, 3, 5, 7 };

            var row = 1;

            ws[$"A{row}"].Value = "PROBLEM";
            ws[$"A{row}"].Style.Font.Bold = true;
            ws[$"B{row}"].Value = "SOLVER";
            ws[$"B{row}"].Style.Font.Bold = true;
            ws[$"C{row}"].Value = "PRIORITIES";
            ws[$"C{row}"].Style.Font.Bold = true;
            ws[$"D{row}"].Value = "RESULT";
            ws[$"D{row}"].Style.Font.Bold = true;
            ws[$"E{row}"].Value = "TIME";
            ws[$"E{row}"].Style.Font.Bold = true;

            row++;

            var stopwatch = new Stopwatch();

            foreach (var problem in problems)
            {
                foreach (var alpha in priorities)
                {
                    foreach (var beta in priorities)
                    {
                        var solvers = new List<ACOBaseSolver>()
                            {
                                new ACOBaseSolver() { Alpha = alpha, Beta = beta },
                                new ACOSubtourReversalSolver() { Alpha = alpha, Beta = beta },
                                new ACOSwapSolver() { Alpha = alpha, Beta = beta }
                            };

                        foreach (var solver in solvers)
                        {
                            ws[$"A{row}"].Value = problem.Name;
                            ws[$"B{row}"].Value = solver.Name;
                            ws[$"C{row}"].Value = $"Alpha={alpha},Beta={beta}";

                            stopwatch.Reset();
                            stopwatch.Start();

                            var result = problem.Solve(solver);

                            stopwatch.Stop();

                            ws[$"D{row}"].Value = Math.Round(result, 2);
                            ws[$"E{row}"].Value = stopwatch.ElapsedMilliseconds;

                            row++;
                        }
                    }
                }
            }
        }

        private static void RunACOSolversWithDifferentAntsFactors(List<CVRPProblem> problems, WorkSheet ws)
        {
            var row = 1;

            ws[$"A{row}"].Value = "PROBLEM";
            ws[$"A{row}"].Style.Font.Bold = true;
            ws[$"B{row}"].Value = "SOLVER";
            ws[$"B{row}"].Style.Font.Bold = true;
            ws[$"C{row}"].Value = "ANTS";
            ws[$"C{row}"].Style.Font.Bold = true;
            ws[$"D{row}"].Value = "RESULT";
            ws[$"D{row}"].Style.Font.Bold = true;
            ws[$"E{row}"].Value = "TIME";
            ws[$"E{row}"].Style.Font.Bold = true;

            row++;

            var stopwatch = new Stopwatch();

            foreach (var problem in problems)
            {
                for (int i = 1; i <= Settings.Computations.MaxAntsFactor; i++)
                {
                    var solvers = new List<ACOBaseSolver>()
                        {
                            new ACOBaseSolver() { AntsFactor = i },
                            new ACOSubtourReversalSolver() { AntsFactor = i },
                            new ACOSwapSolver() { AntsFactor = i }
                        };

                    foreach (var solver in solvers)
                    {
                        for (int j = 0; j < Settings.Computations.Repeats; j++)
                        {
                            ws[$"A{row}"].Value = problem.Name;
                            ws[$"B{row}"].Value = solver.Name;
                            ws[$"C{row}"].Value = problem.Cities * solver.AntsFactor;

                            stopwatch.Reset();
                            stopwatch.Start();

                            var result = problem.Solve(solver);

                            stopwatch.Stop();

                            ws[$"D{row}"].Value = Math.Round(result, 2);
                            ws[$"E{row}"].Value = stopwatch.ElapsedMilliseconds;

                            row++;
                        }
                    }
                }
            }
        }
    }
}
