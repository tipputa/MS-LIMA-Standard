using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Input;
using Metabolomics.MsLima.ViewModel;
using Metabolomics.MsLima.Bean;
using Metabolomics.Core;
using Metabolomics.MsLima.Reader;

namespace Metabolomics.MsLima.Model
{
    public static class ComparativeSpectrumViewerModel
    {
        public static List<MassSpectrum> ImportFile(out string fileName)
        {
            List<MassSpectrum> res;
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "MSP file(*.msp)|*.msp; Text file (*.txt)|*.txt; all files(*)|*;",
                Title = "Import a library file",
                RestoreDirectory = true,
                Multiselect = false
            };

            if (ofd.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                fileName = ofd.FileName;
                res = ReadFile.ReadLibraryFiles(ofd.FileName);
                Mouse.OverrideCursor = null;
            }
            else
            {
                fileName = "";
                res = new List<MassSpectrum>();
            }
            return res;

        }

        public static List<MassSpectrum> ConvertCompoundDataToSpectra(MsLimaData data)
        {
            var spectra = new List<MassSpectrum>();
            foreach(var comp in data.DataStorage.CompoundList)
            {
                foreach(var spec in comp.Spectra)
                {
                    spectra.Add(MassSpectrumUtility.ConvertToRelativeIntensity(spec.Copy()));
                }
            }
            return spectra;
        }
    }
}
