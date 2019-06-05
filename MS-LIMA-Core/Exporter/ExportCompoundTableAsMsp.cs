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
            using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                foreach (var comp in compounds) {
                    Writer.MassSpectrumWriter.WriteMassSpectraAsMsp(sw, comp.Spectra);
                }
            }

        }

        public static void ExportCompoundTableAsMspWithoutRT(string filePath, List<CompoundBean> compounds)
        {
            using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                foreach (var comp in compounds)
                {

                    Writer.MassSpectrumWriter.WriteMassSpectraAsMspWithoutRT(sw, comp.Spectra);
                }
            }
        }

        public static void ExportCompoundTableAsMzMineFormat(string filePath, List<CompoundBean> compounds)
        {
            using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                sw.WriteLine("m/z,RT,InChIKey,Name,AdductType");
                foreach (var comp in compounds)
                {
                    Writer.MassSpectrumWriter.WriteMassSpectraAsMzMineFormat(sw, comp);
                }
            }
        }

        public static void CalculateConsensusPeakInLibrary(string filePath, List<CompoundBean> compounds)
        {
            using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
            { 
                MsGrouping.ExcuteAll(sw, compounds);
            }
        }
    }
}
