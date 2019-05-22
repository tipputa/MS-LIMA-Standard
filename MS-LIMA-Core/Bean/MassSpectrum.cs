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
        public int BinId { get; set; }
        public string CompoundClass { get; set; }
        public string Name { get; set; }
        public double PrecursorMz { get; set; }
        public float RetentionTime { get; set; }
        public float RetentionIndex { get; set; }
        public string Formula { get; set; }
        public IonMode IonMode { get; set; }
        public string Smiles { get; set; }
        public string InChiIKey { get; set; }
        public string ShortInChiIKey { get; set; }
        public string InChI { get; set; }
        public bool Target { get; set; }
        public string Authors { get; set; }
        public string SpectrumType { get; set; }
        public string Instrument { get; set; }
        public string InstrumentType { get; set; }
        public string License { get; set; }

        public string Links { get; set; }

        public AdductIon AdductIon { get; set; }

        public string Comment { get; set; }

        public float CollisionEnergy { get; set; }

        public string Ontology { get; set; }

        public float Intensity { get; set; }

        public double TheoreticalMass { get; set; }

        public float DiffPpm { get; set; }
        public int Order { get; set; }
        public int PeakNumber { get => Spectrum.Count; }
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
