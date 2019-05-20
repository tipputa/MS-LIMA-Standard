using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Metabolomics.MsLima.Bean;

namespace Metabolomics.MsLima
{
    public static class ExportCompoundTable
    {
        public static void ExportCompoundTableAsMsp(string filePath, List<CompoundBean> compounds)
        {
            using(var sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                foreach (var comp in compounds) {
                    Writer.MassSpectrumWriter.WriteMassSpectraAsMsp(sw, comp.Spectra);
                }
            }

        }
    }
}
