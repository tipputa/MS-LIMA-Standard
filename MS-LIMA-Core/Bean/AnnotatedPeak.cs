using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Metabolomics.MsLima.Bean
{
    [MessagePackObject]
    public class AnnotatedPeak
    {
        [Key(0)]
        public double Mz { get; set; }
        public double Intensity { get; set; }
        public string Comment { get; set; }
        public string Frag { get; set; }
        public string Smiles { get; set; }
        public string Formula { get; set; }

        public AnnotatedPeak Copy()
        {
            return new AnnotatedPeak()
            {
                Mz = this.Mz,
                Intensity = this.Intensity,
                Comment = this.Comment,
                Frag = this.Frag,
                Smiles = this.Smiles,
                Formula = this.Formula
            };
        }
    }
}
