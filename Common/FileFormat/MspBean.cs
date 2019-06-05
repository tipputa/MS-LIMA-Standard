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
        [Key(1)]
        public int BinId { get; set; }
        [Key(2)]
        public string CompoundClass { get; set; }
        [Key(3)]
        public string Name { get; set; }
        [Key(4)]
        public double PrecursorMz { get; set; }
        [Key(5)]
        public float RetentionTime { get; set; }
        [Key(6)]
        public float RetentionIndex { get; set; }
        [Key(7)]
        public string Formula { get; set; }
        [Key(8)]
        public IonMode IonMode { get; set; }
        [Key(9)]
        public string Smiles { get; set; }
        [Key(10)]
        public string InchiKey { get; set; }
        [Key(11)]
        public string InChI { get; set; }
        [Key(12)]
        public bool Target { get; set; }
        [Key(13)]
        public string Authors { get; set; }
        [Key(14)]
        public string SpectrumType { get; set; }
        [Key(15)]
        public string Instrument { get; set; }
        [Key(16)]
        public string InstrumentType { get; set; }
        [Key(17)]
        public string License { get; set; }
        [Key(18)]
        public List<float> IsotopeRatioList { get; set; }


        [Key(19)]
        public string Links { get; set; }

        [Key(20)]
        public AdductIon AdductIon { get; set; }

        [Key(21)]
        public string Comment { get; set; }

        [Key(22)]
        public float CollisionEnergy { get; set; }

        [Key(23)]
        public string Ontology { get; set; }

        [Key(24)]
        public float Intensity { get; set; }

        [Key(25)]
        public double TheoreticalMass { get; set; }

        [Key(26)]
        public float DiffPpm { get; set; }
        [Key(27)]
        public int Order { get; set; }
        [Key(28)]
        public int PeakNumber { get; set; }
        [Key(29)]
        public List<Peak> Spectrum { get; set; }

    }
}
