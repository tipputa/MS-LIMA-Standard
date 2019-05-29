using System;
using System.Collections.Generic;
using System.Text;

namespace Metabolomics.Core.Utility
{
    public class CommonUtility
    {
        public static double PpmCalculator(double exactMass, double actualMass)
        {
            if (actualMass <= 0 || exactMass <= 0) return 0;
            double ppm = Math.Round((actualMass - exactMass) / exactMass * 1000000, 4);
            return ppm;
        }



    }
}
