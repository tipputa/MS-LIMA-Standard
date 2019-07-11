using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Metabolomics.MsLima.Bean;


namespace Metabolomics.MsLima.Model
{
    public class MsSimilarityScoring
    {
        public static double GetPresenceSimilarityBasedOnReference(MassSpectrum spectrum, MassSpectrum referenceSpectra, float bin)
        {
            if (referenceSpectra.Spectrum.Count == 0) return 0;

            double sumM = 0, sumL = 0;
            double minMz = referenceSpectra.Spectrum[0].Mz;
            double maxMz = referenceSpectra.Spectrum[referenceSpectra.Spectrum.Count - 1].Mz;
            double focusedMz = minMz;
            double maxLibIntensity = referenceSpectra.Spectrum.Max(n => n.Intensity);
            int remaindIndexM = 0, remaindIndexL = 0;
            int counter = 0;
            int libCounter = 0;

            List<double[]> measuredMassList = new List<double[]>();
            List<double[]> referenceMassList = new List<double[]>();

            while (focusedMz <= maxMz)
            {
                sumL = 0;
                for (int i = remaindIndexL; i < referenceSpectra.Spectrum.Count; i++)
                {
                    if (referenceSpectra.Spectrum[i].Mz < focusedMz - bin) continue;
                    else if (focusedMz - bin <= referenceSpectra.Spectrum[i].Mz && referenceSpectra.Spectrum[i].Mz < focusedMz + bin)
                        sumL += referenceSpectra.Spectrum[i].Intensity;
                    else { remaindIndexL = i; break; }
                }

                if (sumL >= 0.01 * maxLibIntensity)
                {
                    libCounter++;
                }

                sumM = 0;
                for (int i = remaindIndexM; i < spectrum.Spectrum.Count; i++)
                {
                    if (spectrum.Spectrum[i].Mz < focusedMz - bin) continue;
                    else if (focusedMz - bin <= spectrum.Spectrum[i].Mz && spectrum.Spectrum[i].Mz < focusedMz + bin)
                        sumM += spectrum.Spectrum[i].Intensity;
                    else { remaindIndexM = i; break; }
                }

                if (sumM > 0 && sumL >= 0.01 * maxLibIntensity)
                {
                    counter++;
                }

                if (focusedMz + bin > referenceSpectra.Spectrum[referenceSpectra.Spectrum.Count - 1].Mz) break;
                focusedMz = referenceSpectra.Spectrum[remaindIndexL].Mz;
            }

            if (libCounter == 0) return 0;
            else
                return (double)counter / (double)libCounter;
        }

        public static double GetReverseSearchSimilarity(MassSpectrum spectrum, MassSpectrum referenceSpectra, float bin)
        {
            double scalarM = 0, scalarR = 0, covariance = 0;
            double sumM = 0, sumL = 0;

            if (referenceSpectra.Spectrum.Count == 0) return 0;

            double minMz = referenceSpectra.Spectrum[0].Mz;
            double maxMz = referenceSpectra.Spectrum[referenceSpectra.Spectrum.Count - 1].Mz;
            double focusedMz = minMz;
            int remaindIndexM = 0, remaindIndexL = 0;
            int counter = 0;

            List<double[]> measuredMassList = new List<double[]>();
            List<double[]> referenceMassList = new List<double[]>();

            double sumMeasure = 0, sumReference = 0, baseM = double.MinValue, baseR = double.MinValue;

            while (focusedMz <= maxMz)
            {
                sumL = 0;
                for (int i = remaindIndexL; i < referenceSpectra.Spectrum.Count; i++)
                {
                    if (referenceSpectra.Spectrum[i].Mz < focusedMz - bin) continue;
                    else if (focusedMz - bin <= referenceSpectra.Spectrum[i].Mz && referenceSpectra.Spectrum[i].Mz < focusedMz + bin)
                        sumL += referenceSpectra.Spectrum[i].Intensity;
                    else { remaindIndexL = i; break; }
                }

                sumM = 0;
                for (int i = remaindIndexM; i < spectrum.Spectrum.Count; i++)
                {
                    if (spectrum.Spectrum[i].Mz < focusedMz - bin) continue;
                    else if (focusedMz - bin <= spectrum.Spectrum[i].Mz && spectrum.Spectrum[i].Mz < focusedMz + bin)
                        sumM += spectrum.Spectrum[i].Intensity;
                    else { remaindIndexM = i; break; }
                }

                if (sumM <= 0)
                {
                    measuredMassList.Add(new double[] { focusedMz, sumM });
                    if (sumM > baseM) baseM = sumM;

                    referenceMassList.Add(new double[] { focusedMz, sumL });
                    if (sumL > baseR) baseR = sumL;
                }
                else
                {
                    measuredMassList.Add(new double[] { focusedMz, sumM });
                    if (sumM > baseM) baseM = sumM;

                    referenceMassList.Add(new double[] { focusedMz, sumL });
                    if (sumL > baseR) baseR = sumL;

                    counter++;
                }

                if (focusedMz + bin > referenceSpectra.Spectrum[referenceSpectra.Spectrum.Count - 1].Mz) break;
                focusedMz = referenceSpectra.Spectrum[remaindIndexL].Mz;
            }

            if (baseM == 0 || baseR == 0) return 0;

            var eSpectrumCounter = 0;
            var lSpectrumCounter = 0;
            for (int i = 0; i < measuredMassList.Count; i++)
            {
                measuredMassList[i][1] = measuredMassList[i][1] / baseM;
                referenceMassList[i][1] = referenceMassList[i][1] / baseR;
                sumMeasure += measuredMassList[i][1];
                sumReference += referenceMassList[i][1];

                if (measuredMassList[i][1] > 0.1) eSpectrumCounter++;
                if (referenceMassList[i][1] > 0.1) lSpectrumCounter++;
            }

            var cutoff = 0.01;

            for (int i = 0; i < measuredMassList.Count; i++)
            {
                if (referenceMassList[i][1] < cutoff)
                    continue;
                /*
                scalarM += measuredMassList[i][1] * measuredMassList[i][0];
                scalarR += referenceMassList[i][1] * referenceMassList[i][0];
                covariance += Math.Sqrt(measuredMassList[i][1] * referenceMassList[i][1]) * measuredMassList[i][0];
            */
                scalarM += measuredMassList[i][1];
                scalarR += referenceMassList[i][1];
                covariance += Math.Sqrt(measuredMassList[i][1] * referenceMassList[i][1]);

            }

            if (scalarM == 0 || scalarR == 0) { return 0; }
            else { return Math.Pow(covariance, 2) / scalarM / scalarR; }
        }

        public static double GetMassSpectraSimilarity(MassSpectrum spectrum, MassSpectrum referenceSpectra, float bin)
        {
            double scalarM = 0, scalarR = 0, covariance = 0;
            double sumM = 0, sumR = 0;

            if (spectrum.Spectrum.Count == 0) return 0;
            if (referenceSpectra.Spectrum.Count == 0) return 0;

            double minMz = Math.Min(spectrum.Spectrum[0].Mz, referenceSpectra.Spectrum[0].Mz);
            double maxMz = Math.Max(spectrum.Spectrum[spectrum.Spectrum.Count - 1].Mz, referenceSpectra.Spectrum[referenceSpectra.Spectrum.Count - 1].Mz);
            double focusedMz = minMz;
            int remaindIndexM = 0, remaindIndexL = 0;

            List<double[]> measuredMassList = new List<double[]>();
            List<double[]> referenceMassList = new List<double[]>();

            double sumMeasure = 0, sumReference = 0, baseM = double.MinValue, baseR = double.MinValue;

            while (focusedMz <= maxMz)
            {
                sumM = 0;
                for (int i = remaindIndexM; i < spectrum.Spectrum.Count; i++)
                {
                    if (spectrum.Spectrum[i].Mz < focusedMz - bin) { continue; }
                    else if (focusedMz - bin <= spectrum.Spectrum[i].Mz && spectrum.Spectrum[i].Mz < focusedMz + bin) sumM += spectrum.Spectrum[i].Intensity;
                    else { remaindIndexM = i; break; }
                }

                sumR = 0;
                for (int i = remaindIndexL; i < referenceSpectra.Spectrum.Count; i++)
                {
                    if (referenceSpectra.Spectrum[i].Mz < focusedMz - bin) continue;
                    else if (focusedMz - bin <= referenceSpectra.Spectrum[i].Mz && referenceSpectra.Spectrum[i].Mz < focusedMz + bin)
                        sumR += referenceSpectra.Spectrum[i].Intensity;
                    else { remaindIndexL = i; break; }
                }

                if (sumM <= 0 && sumR > 0)
                {
                    measuredMassList.Add(new double[] { focusedMz, sumM });
                    if (sumM > baseM) baseM = sumM;

                    referenceMassList.Add(new double[] { focusedMz, sumR });
                    if (sumR > baseR) baseR = sumR;
                }
                else
                {
                    measuredMassList.Add(new double[] { focusedMz, sumM });
                    if (sumM > baseM) baseM = sumM;

                    referenceMassList.Add(new double[] { focusedMz, sumR });
                    if (sumR > baseR) baseR = sumR;
                }

                if (focusedMz + bin > Math.Max(spectrum.Spectrum[spectrum.Spectrum.Count - 1].Mz, referenceSpectra.Spectrum[referenceSpectra.Spectrum.Count - 1].Mz)) break;
                if (focusedMz + bin > referenceSpectra.Spectrum[remaindIndexL].Mz && focusedMz + bin <= spectrum.Spectrum[remaindIndexM].Mz)
                    focusedMz = spectrum.Spectrum[remaindIndexM].Mz;
                else if (focusedMz + bin <= referenceSpectra.Spectrum[remaindIndexL].Mz && focusedMz + bin > spectrum.Spectrum[remaindIndexM].Mz)
                    focusedMz = referenceSpectra.Spectrum[remaindIndexL].Mz;
                else
                    focusedMz = Math.Min(spectrum.Spectrum[remaindIndexM].Mz, referenceSpectra.Spectrum[remaindIndexL].Mz);
            }

            if (baseM == 0 || baseR == 0) return 0;


            var eSpectrumCounter = 0;
            var lSpectrumCounter = 0;
            for (int i = 0; i < measuredMassList.Count; i++)
            {
                measuredMassList[i][1] = measuredMassList[i][1] / baseM * 999;
                referenceMassList[i][1] = referenceMassList[i][1] / baseR * 999;
                sumMeasure += measuredMassList[i][1];
                sumReference += referenceMassList[i][1];

                if (measuredMassList[i][1] > 0.1) eSpectrumCounter++;
                if (referenceMassList[i][1] > 0.1) lSpectrumCounter++;
            }

            double wM, wR;

            if (sumMeasure - 0.5 == 0) wM = 0;
            else wM = 1 / (sumMeasure - 0.5);

            if (sumReference - 0.5 == 0) wR = 0;
            else wR = 1 / (sumReference - 0.5);

            foreach (var m in measuredMassList) { System.Diagnostics.Debug.WriteLine(m[0] + " " + m[1]); }
            foreach (var m in referenceMassList) { System.Diagnostics.Debug.WriteLine(m[0] + " " + m[1]); }
            for (int i = 0; i < measuredMassList.Count; i++)
            {
                /*scalarM += measuredMassList[i][1] * measuredMassList[i][0];
                scalarR += referenceMassList[i][1] * referenceMassList[i][0];
                covariance += Math.Sqrt(measuredMassList[i][1] * referenceMassList[i][1])*measuredMassList[i][0];
    */
                scalarM += measuredMassList[i][1];
                scalarR += referenceMassList[i][1];
                covariance += Math.Sqrt(measuredMassList[i][1] * referenceMassList[i][1]);

            }

            if (scalarM == 0 || scalarR == 0) { return 0; }
            else
            {
                return Math.Pow(covariance, 2) / scalarM / scalarR;
            }
        }

        public static double GetGaussianSimilarity(float actual, float reference, float tolrance)
        {
            return Math.Exp(-0.5 * Math.Pow((actual - reference) / tolrance, 2));
        }

        public static double GetTotalMsSimilarity(double spectraSimilarity, double reverseSearchSimilarity, double presenceSimilarity)
        { 
            var dotProductFactor = 1.0;
            var revesrseDotProdFactor = 1.0;
            var presensePercentageFactor = 1.0;

            var msmsSimilarity =
                (dotProductFactor * spectraSimilarity + revesrseDotProdFactor * reverseSearchSimilarity + presensePercentageFactor * presenceSimilarity) /
                (dotProductFactor + revesrseDotProdFactor + presensePercentageFactor);
            return msmsSimilarity;
        }
    }
}
