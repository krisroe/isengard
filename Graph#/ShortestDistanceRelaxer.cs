using QuickGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace GraphSharp
{
    public sealed class ShortestDistanceRelaxer : QuickGraph.Algorithms.IDistanceRelaxer
    {
        public static readonly ShortestDistanceRelaxer Instance;

        double IDistanceRelaxer.InitialDistance => double.MaxValue;

        double IDistanceRelaxer.Combine(double distance, double weight)
        {
            return distance + weight;
        }

        int IComparer<double>.Compare(double x, double y)
        {
            return x.CompareTo(y);
        }
    }
}
