using System;
using System.Collections.Generic;
using System.Text;
using Metabolomics.Core;
using MessagePack;

namespace Metabolomics.MsLima.Bean
{
    [MessagePackObject]
    public class MassSpectrum
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
        public string InChiIKey { get; set; }
        [Key(11)]
        public string ShortInChiIKey { get; set; }
        [Key(12)]
        public string InChI { get; set; }
        [Key(13)]
        public bool Target { get; set; }
        [Key(14)]
        public string Authors { get; set; }
        [Key(15)]
        public string SpectrumType { get; set; }
        [Key(16)]
        public string Instrument { get; set; }
        [Key(17)]
        public string InstrumentType { get; set; }
        [Key(18)]
        public string License { get; set; }

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

        public List<string> OtherMetaData { get; set; }
        [Key(29)]

        public int PeakNumber { get => Spectrum.Count; }
        [Key(30)]
        public List<AnnotatedPeak> Spectrum { get; set; }

        public MassSpectrum Copy()
        {
            var spectrum = new MassSpectrum()
            {
                Id = this.Id,
                BinId = this.BinId,
                CompoundClass = this.CompoundClass,
                Name = this.Name,
                PrecursorMz = this.PrecursorMz,
                RetentionTime = this.RetentionTime,
                RetentionIndex = this.RetentionIndex,
                Formula = this.Formula,
                IonMode = this.IonMode,
                Smiles = this.Smiles,
                InChiIKey = this.InChiIKey,
                ShortInChiIKey = this.ShortInChiIKey,
                InChI = this.InChI,
                Target = this.Target,
                Authors = this.Authors,
                SpectrumType = this.SpectrumType,
                Instrument = this.Instrument,
                InstrumentType = this.InstrumentType,
                License = this.License,
                Links = this.Links,
                Comment = this.Comment,
                CollisionEnergy = this.CollisionEnergy,
                Ontology = this.Ontology,
                Intensity = this.Intensity,
                TheoreticalMass = this.TheoreticalMass,
                DiffPpm = this.DiffPpm,
                Order = this.Order,
                AdductIon = Metabolomics.Core.Parser.AdductIonParser.GetAdductIon(this.AdductIon.AdductIonName),
                Spectrum = new List<AnnotatedPeak>()
            };
            foreach (var peak in this.Spectrum) {
                spectrum.Spectrum.Add(peak.Copy());
            }
            return spectrum;
        }
    }
}
