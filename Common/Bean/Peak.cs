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
        public double Intensity { get; set; }
        public string Comment { get; set; }
        public string Frag { get; set; }

        /*        public string Smiles { get; set; }
                public string Formulat { get; set; }
                public string 
          */
    }
}
