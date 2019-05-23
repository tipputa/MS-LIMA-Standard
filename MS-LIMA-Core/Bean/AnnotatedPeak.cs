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
        [Key(1)]
        public double Intensity { get; set; }
        [Key(2)]
        public string Comment { get; set; }
        [Key(3)]
        public string Frag { get; set; }
        [Key(4)]
        public string Smiles { get; set; }
        [Key(5)]
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
