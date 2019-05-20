using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Input;
using Metabolomics.Core;
using Metabolomics.MsLima.Bean;

namespace Metabolomics.MsLima.Model
{
    public static class ImportUtility
    {
        public static void ImportFile(MsLimaData msLimaData)
        {
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
                msLimaData.DataStorage.SetLibrary(ofd.FileName);
                //var dt = DateTime.Now;
                //var mspFileNewPath = System.IO.Path.GetDirectoryName(mspFilePath) + "\\" + System.IO.Path.GetFileNameWithoutExtension(mspFilePath) + "_StartMod_"
                //    + dt.ToString("yy_MM_dd_HH_mm_ss") + ".msp";
                //System.IO.File.Copy(mspFilePath, mspFileNewPath, true);
                //var mspQueriesOld = MspFileParcer.MspFileReaderMakeCompoundList(mspFileNewPath);
                //var mspQueries = MspFileParcer.GetCompList(mspQueriesOld);
                //this.mainWindowVM.Data = new DataStorageBean(mspFileNewPath, mspQueries);
                //this.mainWindowVM.Refresh_ImportRawData(mspQueries);
                //this.Title = this.MainWindowTitle + " " + mspFileNewPath;
                Mouse.OverrideCursor = null;
            }
        }
    }
}
