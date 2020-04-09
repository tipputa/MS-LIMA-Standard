using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Metabolomics.MsLima.Bean;
namespace Metabolomics.MsLima.Writer
{
    public static class MassSpectrumWriter
    {
        #region Msp
        public static void WriteMassSpectraAsMspWithoutRT(StreamWriter sw, List<MassSpectrum> spectra)
        {
            foreach (var spectrum in spectra)
            {                
                WriteMassSpectrumAsMsp(sw, Model.MassSpectrumUtility.DropRetentionTime(spectrum.Copy()));
            }
        }

        public static void WriteMassSpectraAsMsp(StreamWriter sw, List<MassSpectrum> spectra)
        {
            foreach(var spectrum in spectra)
            {
                WriteMassSpectrumAsMsp(sw, spectrum);
            }            
        }

        public static void WriteMassSpectrumAsMsp(StreamWriter sw, MassSpectrum spec)
        {
            if (spec.PeakNumber == 0) return;
            sw.WriteLine("NAME: " + spec.Name);
            if (spec.RetentionTime >= 0)
                sw.WriteLine("RETENTIONTIME: " + spec.RetentionTime);
            else
                sw.WriteLine("RETENTIONTIME: -1");
            sw.WriteLine("PRECURSORMZ: " + spec.PrecursorMz);
            if(spec.AdductIon != null && !string.IsNullOrEmpty(spec.AdductIon.AdductIonName))
                sw.WriteLine("PRECURSORTYPE: " + spec.AdductIon.AdductIonName);
            sw.WriteLine("IONMODE: " + spec.IonMode);
            sw.WriteLine("COLLISIONENERGY: " + spec.CollisionEnergy);
            sw.WriteLine("FORMULA: " + spec.Formula);
            sw.WriteLine("SMILES: " + spec.Smiles);
            sw.WriteLine("INCHIKEY: " + spec.InChIKey);
            sw.WriteLine("INCHI: " + spec.InChI);
            sw.WriteLine("SPECTRUMTYPE: " + spec.SpectrumType);
            sw.WriteLine("AUTHORS: " + spec.Authors);
            sw.WriteLine("INSTRUMENT: " + spec.Instrument);
            sw.WriteLine("INSTRUMENTTYPE: " + spec.InstrumentType);
            sw.WriteLine("LICENSE: " + spec.License);
            sw.WriteLine("MSLEVEL: " + spec.MsLevel);
            if(spec.OtherMetaData != null && spec.OtherMetaData.Count > 0)
            {
                foreach(var m in spec.OtherMetaData)
                {
                    sw.WriteLine(m);
                }
            }
            sw.WriteLine("COMMENT: " + spec.Comment);
            sw.WriteLine("Num Peaks:" + spec.PeakNumber);

            foreach (var peak in spec.Spectrum)
            {
                if (String.IsNullOrWhiteSpace(peak.Comment))
                {
                    sw.WriteLine(peak.Mz + "\t" + peak.Intensity);
                }
                else
                {
                    sw.WriteLine(peak.Mz + "\t" + peak.Intensity + "\t" + "\"" + peak.Comment + "\"");
                }
            }
            sw.WriteLine("");
        }

        #endregion


        #region MGF
        public static void WriteMassSpectraAsMgf(StreamWriter sw, List<MassSpectrum> spectra)
        {
            foreach (var spectrum in spectra)
            {
                WriteMassSpectrumAsMgf(sw, spectrum);
            }
        }

        public static void WriteMassSpectrumAsMgf(StreamWriter sw, MassSpectrum spec)
        {
            if (spec.PeakNumber == 0) return;
            sw.WriteLine("BEGIN IONS");
            var rt = -1.0;
            if (spec.RetentionTime >= 0)
               rt = Math.Round(spec.RetentionTime * 60, 2);

            var titleString = spec.Name + "; CE=" + spec.CollisionEnergy + "; Num Peaks:" + spec.PeakNumber;
            sw.WriteLine("TITLE=" + titleString);
            sw.WriteLine("RTINMINUTES=" + Math.Round(spec.RetentionTime, 3));
            sw.WriteLine("RTINSECONDS=" + rt);
            sw.WriteLine("PEPMASS=" + Math.Round(spec.PrecursorMz,5));

            foreach (var peak in spec.Spectrum)
            {
                sw.WriteLine(peak.Mz + "\t" + peak.Intensity);
            }
            sw.WriteLine("END IONS");
        }

        #endregion

        public static void WriteMassSpectraAsMzMineFormat(StreamWriter sw, CompoundBean compound)
        {
            var exported = new List<string>();
            foreach(var spec in compound.Spectra){
                if (exported.Contains(spec.AdductIon.AdductIonName) == true) continue;
                sw.WriteLine(Math.Round(spec.TheoreticalMass, 6) + "," + spec.RetentionTime + "," + spec.InChIKey + "," + spec.Name.Replace(',', '_') + "," + spec.AdductIon.AdductIonName);
                exported.Add(spec.AdductIon.AdductIonName);
            }
        }

    }
}
