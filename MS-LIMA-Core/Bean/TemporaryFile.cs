using System;
using System.Collections.Generic;
using System.Text;

namespace Metabolomics.MsLima.Bean
{
    public class TemporaryFile
    {
        public string InChIKey { get; set; }
        public string ShortInChIKey { get { return InChIKey.Split('-')[0]; } }
        public string SMILES { get; set; }
        public string InChI { get; set; }
    }

    public class CommonMetaData
    {
        public string Authors { get; set; }
        public string Instrument { get; set; }
        public string InstrumentType { get; set; }
        public string License { get; set; }
        public string Comment { get; set; }
        public string SpectrumType { get; set; }
        public string MsLevel { get; set; }

    }
}
