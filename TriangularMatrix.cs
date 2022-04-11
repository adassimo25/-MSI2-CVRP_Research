
using System.Linq;

namespace CVRP_Research
{
    public class TriangularMatrix
    {
        private float[][] Values { get; set; }
        public int Size { get { return Values.Length; } }

        public TriangularMatrix(int n, float initValue = 0.0f)
        {
            Values = new float[n][].Select((x, i) => new float[i + 1].Select(y => initValue).ToArray()).ToArray();
        }

        public float this[int i, int j]
        {
            get
            {
                return (i >= j ? Values[i][j] : Values[j][i]);
            }

            set
            {
                if (i >= j)
                {
                    Values[i][j] = value;
                }
                else
                {
                    Values[j][i] = value;
                }
            }
        }
    }
}
