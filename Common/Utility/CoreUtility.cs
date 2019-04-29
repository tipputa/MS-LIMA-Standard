using System;
using System.Collections.Generic;
using System.Text;

namespace Metabolomics.Core.Utility
{
    public class CommonUtility
    {
        public static double PpmCalculator(double exactMass, double actualMass)
        {
            double ppm = Math.Round((actualMass - exactMass) / exactMass * 1000000, 4);
            return ppm;
        }



    }
}
