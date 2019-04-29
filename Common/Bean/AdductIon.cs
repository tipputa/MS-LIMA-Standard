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
        public int AdductIonXmer { get; set; }
        public string AdductIonName { get; set; }
        public int ChargeNumber { get; set; }
        public IonMode IonMode { get; set; }
        public bool FormatCheck { get; set; }
        public double M1Intensity { get; set; }
        public double M2Intensity { get; set; }
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
