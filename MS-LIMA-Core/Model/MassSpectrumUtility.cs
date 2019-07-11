using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Metabolomics.MsLima.Bean;

namespace Metabolomics.MsLima.Model
{
    public static class MassSpectrumUtility
    {
        public static MassSpectrum ConvertToRelativeIntensity(MassSpectrum spectrum)
        {
            double maxIntensityRate = 100.0 / spectrum.Spectrum.Max(x => x.Intensity);
            if (maxIntensityRate == 1) return spectrum;

            foreach(var peaks in spectrum.Spectrum)
            {
                peaks.Intensity *= maxIntensityRate;
            }
            return spectrum;
        }

        public static MassSpectrum DropRetentionTime(MassSpectrum spectrum)
        {
            spectrum.RetentionTime = -1;
            return spectrum;
        }

        public static void ConvertActualMassToTheoreticalMass(MassSpectrum spectrum)
        {
            spectrum.PrecursorMz = spectrum.TheoreticalMass;
        }

        public static void RemoveUnannotatedPeaks(MassSpectrum spectrum)
        {
            var peaks = new List<AnnotatedPeak>();
            foreach(var peak in spectrum.Spectrum)
            {
                if(!string.IsNullOrEmpty(peak.Comment))
                {
                    peaks.Add(peak);
                    Console.Write(peak.Mz + "\t");
                    Console.WriteLine(peak.Comment);
                }
            }
            spectrum.Spectrum = peaks;
        }
    }
}
