using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Metabolomics.Core
{
    [MessagePackObject]
    public class AdductIon
    {
        [Key(0)]
        public double AdductIonAccurateMass { get; set; }
        [Key(1)]
        public int AdductIonXmer { get; set; }
        [Key(2)]
        public string AdductIonName { get; set; }
        [Key(3)]
        public int ChargeNumber { get; set; }
        [Key(4)]
        public IonMode IonMode { get; set; }
        [Key(5)]
        public bool FormatCheck { get; set; }
        [Key(6)]
        public double M1Intensity { get; set; }
        [Key(7)]
        public double M2Intensity { get; set; }
        [Key(8)]
        public bool IsRadical { get; set; }
        public AdductIon()
        {
            AdductIonAccurateMass = -1;
            AdductIonName = string.Empty;
            ChargeNumber = -1;
            AdductIonXmer = -1;
            IonMode = IonMode.Positive;
            FormatCheck = false;
            IsRadical = false;
        }
    }
}
