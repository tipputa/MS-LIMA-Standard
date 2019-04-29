using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Metabolomics.Core.Utility
{
    public class FormulaUtility
    {
        public static double GetMass(string formula)
        {
            double res = 0.0;
            string elementRegex = "([A-Z][a-z]*)([0-9]*)";
            var dic = new Dictionary<string, int>();
            var Atoms = new List<string>();
            foreach (Match match in Regex.Matches(formula, elementRegex))
            {
                string name = match.Groups[1].Value;
                int count = match.Groups[2].Value != "" ? int.Parse(match.Groups[2].Value) : 1;
                if (Atoms.Contains(name))
                {
                    dic[name] = dic[name] + count;
                }
                else
                {
                    Atoms.Add(name);
                    dic.Add(name, count);
                }
            }

            foreach (string atom in dic.Keys)
            {
                if (StaticStorage.AtomicMassDict.ContainsKey(atom))
                {
                    res += StaticStorage.AtomicMassDict[atom] * (double)dic[atom];
                }
                else
                {
                    Console.WriteLine(atom);
                }
            }
            return res;
        }
    }
}
