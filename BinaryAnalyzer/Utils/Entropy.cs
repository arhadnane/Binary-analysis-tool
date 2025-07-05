// Calcul d'entropie de données binaires
using System;

namespace BinaryAnalyzer.Utils
{
    public static class Entropy
    {
        public static double Calculate(byte[] data)
        {
            if (data == null || data.Length == 0)
                return 0.0;
            int[] counts = new int[256];
            foreach (var b in data)
                counts[b]++;
            double entropy = 0.0;
            int len = data.Length;
            for (int i = 0; i < 256; i++)
            {
                if (counts[i] == 0) continue;
                double p = (double)counts[i] / len;
                entropy -= p * Math.Log2(p);
            }
            return entropy;
        }
    }
}
