using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows;
using Metabolomics.Core;
using Metabolomics.MsLima.Bean;

namespace Metabolomics.MsLima.Model
{
    public static class ImportUtility
    {
        public static void ImportFile(MsLimaData msLimaData, AutoRepeater exporter)
        {
            var res = MessageBox.Show("Do you open by editing mode?", "Ask", MessageBoxButton.YesNo);

            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "MSP file(*.msp)|*.msp| MGF file (*.mgf)|*.mgf| Text file (*.txt)|*.txt| all files(*)|*;",
                Title = "Import a library file",
                RestoreDirectory = true,
                Multiselect = false
            };

            if (ofd.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (res == MessageBoxResult.Yes)
                {
                    var dt = DateTime.Now;
                    var newFilePath = System.IO.Path.GetDirectoryName(ofd.FileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(ofd.FileName) + "_StartMod_"
                        + dt.ToString("yy_MM_dd_HH_mm_ss") + System.IO.Path.GetExtension(ofd.FileName);
                    System.IO.File.Copy(ofd.FileName, newFilePath, true);
                    msLimaData.DataStorage.SetLibrary(newFilePath, msLimaData.Parameter.CompoundGroupingKey);
                    msLimaData.DataStorage.OriginalFilePath = ofd.FileName;
                    WindowUtility.CheckCompoundGroup(msLimaData.DataStorage.CompoundList);
                    exporter.Start();
                }
                else
                {
                    msLimaData.DataStorage.SetLibrary(ofd.FileName, msLimaData.Parameter.CompoundGroupingKey);
                    WindowUtility.CheckCompoundGroup(msLimaData.DataStorage.CompoundList);
                }
                Mouse.OverrideCursor = null;
            }
        }
    }
}
