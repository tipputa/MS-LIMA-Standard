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
    public static class ExportUtility
    {
        public static void SaveAsMsp(List<CompoundBean> compounds)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "MSP file(.msp)|*.msp|All Files (*.*)|*.*";

            if (sfd.ShowDialog() == true)
            {
                var filePath = sfd.FileName;
                Mouse.OverrideCursor = Cursors.Wait;
                ExportCompoundTable.ExportCompoundTableAsMsp(filePath, compounds);
                Mouse.OverrideCursor = null;
            }
        }

    }
}
