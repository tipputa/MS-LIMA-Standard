using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Metabolomics.Core
{
    [MessagePackObject]
    public class MspBean
    {
        [Key(0)]
        public int Id { get; set; }
        public int BinId { get; set; }
        public string CompoundClass { get; set; }        
        public string Name { get; set; }
        public double PrecursorMz { get; set; }
        public float RetentionTime { get; set; }
        public float RetentionIndex { get; set; }
        public string Formula { get; set; }
        public IonMode IonMode { get; set; }
        public string Smiles { get; set; }
        public string InchiKey { get; set; }
        public string InChI { get; set; }
        public bool Target { get; set; }
        public string Authors { get; set; }
        public string SpectrumType { get; set; }
        public string Instrument { get; set; }
        public string InstrumentType { get; set; }
        public string License { get; set; }
        public List<float> IsotopeRatioList { get; set; }


        public string Links { get; set; }

        public AdductIon AdductIon { get; set; }
  
        public string Comment { get; set; }

        public float CollisionEnergy { get; set; }

        public string Ontology { get; set; }

        public float Intensity { get; set; }

        public double TheoreticalMass { get; set; }

        public float DiffPpm { get; set; }
        public int Order { get; set; }
        public int PeakNumber { get; set; }
        public List<Peak> Spectrum { get; set; }

    }
}
