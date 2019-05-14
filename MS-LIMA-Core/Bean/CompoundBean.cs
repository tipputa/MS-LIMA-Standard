using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Metabolomics.Core;

namespace Metabolomics.MsLima.Bean
{
    public class CompoundBean
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RetentionTimes { get => GetRTs(Spectra.Select(x => x.RetentionTime).ToList()); }
        public string RetentionTime { get; set; }
        public double MolecularWeight { get; set; }
        public string InChIKey { get; set; }
        public string InChI { get; set; }
        public string Formula { get; set; }
        public string Smiles { get; set; }
        public int NumSpectra { get; set; }
        public List<MassSpectrum> Spectra { get; set; } = new List<MassSpectrum>();
        public string GetRTs(List<float> rts)
        {
            var rtlist = rts.OrderBy(x => x).Distinct().ToList();
            var res = "";
            for (var i = 0; i < rtlist.Count; i++)
            {
                if (i == 0)
                    res = Math.Round(rtlist[0], 2).ToString();
                else
                    res = res + ", " + Math.Round(rtlist[i], 2);
            }
            return res;
        }

    }
}
