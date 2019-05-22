using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabolomics.MsLima.Bean;

namespace Metabolomics.MsLima.ViewModel
{
    public class AllSpectraMetaInformationTableVM
    {
        public List<MassSpectrum> Table { get; set; } = new List<MassSpectrum>();
        public AllSpectraMetaInformationTableVM() { }

        public AllSpectraMetaInformationTableVM(MsLimaData data)
        {
            foreach(var comp in data.DataStorage.CompoundList)
            {
                foreach(var spec in comp.Spectra)
                {
                    Table.Add(spec);
                }
            }

        }
    }
}
