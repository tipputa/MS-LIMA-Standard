using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using Metabolomics.Core;
using Metabolomics.Core.Utility;
using Metabolomics.Core.Parser;
using Metabolomics.MsLima.Bean;

namespace Metabolomics.MsLima.Reader
{
    public sealed class ReadFile
    {
        public static List<MassSpectrum> ReadLibraryFiles(string filePath)
        {
            List<MassSpectrum> spectra;
            var extention = Path.GetExtension(filePath);
            if(extention == ".mgf")
            {
                spectra = ReadMgfFileAsMsSpectrum(filePath);
                return spectra;
            }
            else if(extention == ".msp")
            {
                spectra = ReadMspFileAsMsSpectrum(filePath);
                return spectra;
            }
            else if(extention == ".massbank")
            {
                spectra = ReadMassBankFileAsMsSpectrum(filePath);
                return spectra;
            }
            else if(extention == ".txt")
            {
                spectra = ReadTextFileAsMsSpectrum(filePath);
                return spectra;
            }
            else
            {
                spectra = ReadTextFileAsMsSpectrum(filePath);
                return spectra;
            }
        }

        public static List<MassSpectrum> ReadMspFileAsMsSpectrum(string filePath)
        {
            var spectra = new List<MassSpectrum>();
            var spectrum = new MassSpectrum();
            string wkstr;
            int counter = 0;

            using (StreamReader sr = new StreamReader(filePath, Encoding.ASCII))
            {
                float rt = 0, preMz = 0, ri = 0;

                while (sr.Peek() > -1)
                {
                    wkstr = sr.ReadLine();
                    if (Regex.IsMatch(wkstr, "^NAME:.*", RegexOptions.IgnoreCase))
                    {
                        spectrum.Id = counter;
                        spectrum.Name = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();

                        while (sr.Peek() > -1)
                        {
                            wkstr = sr.ReadLine();
                            if (wkstr == string.Empty || String.IsNullOrWhiteSpace(wkstr)) break;
                            if (Regex.IsMatch(wkstr, "^COMMENT.*:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Comment = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "AUTHORS.*:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Authors = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INSTRUMENT.*:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Instrument = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INSTRUMENT.?TYPE.*:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.InstrumentType = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "LICENSE:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.License = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }

                            else if (Regex.IsMatch(wkstr, "SPECTRUM.?TYPE:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.SpectrumType = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "FORMULA:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Formula = wkstr.Split(':')[1].Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "ION.?MODE:.*", RegexOptions.IgnoreCase))
                            {
                                if (wkstr.Split(':')[1].Trim() == "Negative") spectrum.IonMode = IonMode.Negative;
                                else spectrum.IonMode = IonMode.Positive;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "SMILES:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Smiles = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "COLLISION.?ENERGY:", RegexOptions.IgnoreCase))
                            {
                                spectrum.CollisionEnergy = MspParser.GetCollisionEnergy(wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim()); ;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INCHIKEY:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.InChiIKey = wkstr.Split(':')[1].Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INCHI:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.InChI = wkstr.Split(':')[1].Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "COMPOUNDCLASS:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.CompoundClass = wkstr.Split(':')[1].Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RETENTIONTIME:.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(wkstr.Split(':')[1].Trim(), out rt)) spectrum.RetentionTime = rt; else spectrum.RetentionTime = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RT:.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(wkstr.Split(':')[1].Trim(), out rt)) spectrum.RetentionTime = rt; else spectrum.RetentionTime = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RETENTIONINDEX:.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(wkstr.Split(':')[1].Trim(), out ri)) spectrum.RetentionIndex = ri; else spectrum.RetentionIndex = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RI:.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(wkstr.Split(':')[1].Trim(), out ri)) spectrum.RetentionIndex = ri; else spectrum.RetentionIndex = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "PRECURSORMZ:.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(wkstr.Split(':')[1].Trim(), out preMz)) spectrum.PrecursorMz = preMz;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "PRECURSOR.?TYPE:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.AdductIon = AdductIonParser.GetAdductIon(wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim());
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "COMPOUNDCLASS:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.CompoundClass = wkstr.Split(':')[1].Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "Links:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Links = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "Num Peaks:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Spectrum = ReadSpectrum(sr, wkstr, out int peakNum);
                                spectrum.PeakNumber = spectrum.Spectrum.Count;
                                continue;
                            }
                        }
                        try
                        {
                            spectrum.TheoreticalMass = MspParser.ConvertFormulaToAdductMass(spectrum.AdductIon, spectrum.Formula, spectrum.IonMode);
                            spectrum.DiffPpm = (float)CommonUtility.PpmCalculator(spectrum.TheoreticalMass, spectrum.PrecursorMz);
                        }
                        catch
                        {
                            spectrum.TheoreticalMass = -1;
                            spectrum.DiffPpm = 0;
                        }
                        spectra.Add(spectrum);
                        spectrum = new MassSpectrum();
                        counter++;
                    }
                }
            }
            return spectra;
        }


        public static List<AnnotatedPeak> ReadSpectrum(StreamReader sr, string numPeakField, out int peaknum)
        {
            peaknum = 0;
            var mspPeaks = new List<AnnotatedPeak>();

            if (int.TryParse(numPeakField.Split(':')[1].Trim(), out peaknum))
            {
                if (peaknum == 0) { return mspPeaks; }

                var pairCount = 0;
                var mspPeak = new AnnotatedPeak();

                while (pairCount < peaknum)
                {
                    var wkstr = sr.ReadLine();
                    if (wkstr == string.Empty) break;
                    var numChar = string.Empty;
                    var mzFill = false;

                    for (int i = 0; i < wkstr.Length; i++)
                    {
                        if (char.IsNumber(wkstr[i]) || wkstr[i] == '.')
                        {
                            numChar += wkstr[i];

                            if (i == wkstr.Length - 1 && wkstr[i] != '"')
                            {
                                if (mzFill == false)
                                {
                                    if (numChar != string.Empty)
                                    {
                                        mspPeak.Mz = float.Parse(numChar);
                                        mzFill = true;
                                        numChar = string.Empty;
                                    }
                                }
                                else if (mzFill == true)
                                {
                                    if (numChar != string.Empty)
                                    {
                                        mspPeak.Intensity = float.Parse(numChar);
                                        mzFill = false;
                                        numChar = string.Empty;

                                        if (mspPeak.Comment == null)
                                            mspPeak.Comment = mspPeak.Mz.ToString();
                                        mspPeaks.Add(mspPeak);
                                        mspPeak = new AnnotatedPeak();
                                        pairCount++;
                                    }
                                }
                            }
                        }
                        else if (wkstr[i] == '"')
                        {
                            i++;
                            var letterChar = string.Empty;

                            while (wkstr[i] != '"')
                            {
                                letterChar += wkstr[i];
                                i++;
                            }
                            if (!letterChar.Contains("_f_"))
                                mspPeaks[mspPeaks.Count - 1].Comment = letterChar;
                            else
                            {
                                mspPeaks[mspPeaks.Count - 1].Comment = letterChar.Split(new string[] { "_f_" }, StringSplitOptions.None)[0];
                                mspPeaks[mspPeaks.Count - 1].Frag = letterChar.Split(new string[] { "_f_" }, StringSplitOptions.None)[1];
                            }

                        }
                        else
                        {
                            if (mzFill == false)
                            {
                                if (numChar != string.Empty)
                                {
                                    mspPeak.Mz = float.Parse(numChar);
                                    mzFill = true;
                                    numChar = string.Empty;
                                }
                            }
                            else if (mzFill == true)
                            {
                                if (numChar != string.Empty)
                                {
                                    mspPeak.Intensity = float.Parse(numChar);
                                    mzFill = false;
                                    numChar = string.Empty;

                                    if (mspPeak.Comment == null)
                                        mspPeak.Comment = mspPeak.Mz.ToString();

                                    mspPeaks.Add(mspPeak);
                                    mspPeak = new AnnotatedPeak();
                                    pairCount++;
                                }
                            }
                        }
                    }
                }

                mspPeaks = mspPeaks.OrderBy(n => n.Mz).ToList();
            }

            return mspPeaks;
        }
        public static List<MassSpectrum> ReadMgfFileAsMsSpectrum(string filePath)
        {
            var spectra = new List<MassSpectrum>();
            return spectra;
        }
        public static List<MassSpectrum> ReadMassBankFileAsMsSpectrum(string filePath)
        {
            var spectra = new List<MassSpectrum>();
            return spectra;
        }
        public static List<MassSpectrum> ReadTextFileAsMsSpectrum(string filePath)
        {
            var spectra = new List<MassSpectrum>();
            return spectra;
        }

    }
}
