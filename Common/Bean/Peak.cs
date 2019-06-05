using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Metabolomics.Core
{
    [MessagePackObject]
    public class Peak
    {
        [Key(0)]
        public double Mz { get; set; }
        [Key(1)]
        public double Intensity { get; set; }
        [Key(2)]
        public string Comment { get; set; }
        [Key(3)]
        public string Frag { get; set; }

        /*        public string Smiles { get; set; }
                public string Formulat { get; set; }
                public string 
          */
    }
}
