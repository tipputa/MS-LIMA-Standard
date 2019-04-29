using System;
using System.Collections.Generic;
using System.Text;

namespace Metabolomics.Core
{
    public static class StaticStorage
    {
        public static Dictionary<string, double> AtomicMassDict = new Dictionary<string, double>(){
            {"C", 12.00000},
            {"H", 1.007825},
            {"N", 14.003074},
            {"O", 15.994915},
            {"Na", 22.989770},
            {"Cl", 34.968853f},
            {"S", 31.972072f},
            {"P", 30.973763f},
            {"Br", 78.91834f},
            {"Si", 27.97693f},
            {"I", 126.9045f},
            {"F", 18.99840f},
            {"Fe", 55.93494f},
            {"Se", 79.91652f},
            {"Pt", 194.9648f},
            {"D", 2.014102f},
            {"Co", 58.93320f},
            {"As", 74.92160f},
            {"B", 11.00931f},
            {"K", 38.96371f},
            {"Mg", 23.98504f},
            {"Cu", 62.92960f},
            {"Sn", 119.9022f},
            {"Ag", 106.9051f}
        };

    }
}
