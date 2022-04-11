
namespace CVRP_Research
{
    public static class Settings
    {
        public static class ACO
        {
            public static int Alpha { get; } = 2;
            public static int Beta { get; } = 5;
            public static float Phi { get; } = 0.7f;
            public static float Q { get; } = 10.0f;
            public static int AntsFactor { get; } = 1;
            public static int Iterations { get; } = 250;
        }

        public static class Greedy
        {
            public static int VehicleTourTimeout { get; } = 30000;
        }

        public static class Computations
        {
            public static int MaxAntsFactor { get; } = 4;
            public static int Repeats { get; } = 5;
        }

        public static class Results
        {
            public static string Directory { get; } = "./Results";
            public static string FileName { get; } = "results";
            public static string FileDateFormat { get; } = "yyyy-MM-dd-HHmmss";
            public static string FileExtension { get; } = "xlsx";
            public static string WorksheetHypothesis123 { get; } = "Hypothesis-1_2_3";
            public static string WorksheetHypothesis4 { get; } = "Hypothesis-4";
            public static string WorksheetHypothesis5 { get; } = "Hypothesis-5";
        }
    }
}
