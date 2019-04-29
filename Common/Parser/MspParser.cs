using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using Metabolomics.Core;
using Metabolomics.Core.Utility;

namespace Metabolomics.Core.Parser
{
    public sealed class MspParser
    {
        public static List<MspBean> MspFileReader(string databaseFilepath)
        {
            var mspFields = new List<MspBean>();
            var mspField = new MspBean();
            var mspPeak = new MspBean();
            string wkstr;
            int counter = 0;

            using (StreamReader sr = new StreamReader(databaseFilepath, Encoding.ASCII))
            {
                float rt = 0, preMz = 0, ri = 0;

                while (sr.Peek() > -1)
                {
                    wkstr = sr.ReadLine();
                    if (Regex.IsMatch(wkstr, "^NAME:.*", RegexOptions.IgnoreCase))
                    {
                        mspField.Id = counter;
                        mspField.Name = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();

                        while (sr.Peek() > -1)
                        {
                            wkstr = sr.ReadLine();
                            if (wkstr == string.Empty || String.IsNullOrWhiteSpace(wkstr)) break;
                            if (Regex.IsMatch(wkstr, "^COMMENT.*:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.Comment = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "AUTHORS.*:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.Authors = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INSTRUMENT.*:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.Instrument = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INSTRUMENT.?TYPE.*:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.InstrumentType = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "LICENSE:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.License = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }

                            else if (Regex.IsMatch(wkstr, "SPECTRUM.?TYPE:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.SpectrumType = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "FORMULA:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.Formula = wkstr.Split(':')[1].Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "ION.?MODE:.*", RegexOptions.IgnoreCase))
                            {
                                if (wkstr.Split(':')[1].Trim() == "Negative") mspField.IonMode = IonMode.Negative;
                                else mspField.IonMode = IonMode.Positive;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "SMILES:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.Smiles = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "COLLISION.?ENERGY:", RegexOptions.IgnoreCase))
                            {
                                mspField.CollisionEnergy = getCollisionEnergy(wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim()); ;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INCHIKEY:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.InchiKey = wkstr.Split(':')[1].Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INCHI:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.InChI = wkstr.Split(':')[1].Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "COMPOUNDCLASS:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.CompoundClass = wkstr.Split(':')[1].Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RETENTIONTIME:.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(wkstr.Split(':')[1].Trim(), out rt)) mspField.RetentionTime = rt; else mspField.RetentionTime = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RT:.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(wkstr.Split(':')[1].Trim(), out rt)) mspField.RetentionTime = rt; else mspField.RetentionTime = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RETENTIONINDEX:.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(wkstr.Split(':')[1].Trim(), out ri)) mspField.RetentionIndex = ri; else mspField.RetentionIndex = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RI:.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(wkstr.Split(':')[1].Trim(), out ri)) mspField.RetentionIndex = ri; else mspField.RetentionIndex = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "PRECURSORMZ:.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(wkstr.Split(':')[1].Trim(), out preMz)) mspField.PrecursorMz = preMz;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "PRECURSOR.?TYPE:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.AdductIon = Parser.AdductIonParser.GetAdductIon(wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim());
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "COMPOUNDCLASS:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.CompoundClass = wkstr.Split(':')[1].Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "Links:.*", RegexOptions.IgnoreCase))
                            {
                                mspField.Links = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "Num Peaks:.*", RegexOptions.IgnoreCase))
                            {
                                var peakNum = 0;
                                mspField.Spectrum = ReadSpectrum(sr, wkstr, out peakNum);
                                mspField.PeakNumber = mspField.Spectrum.Count;
                                continue;
                            }
                        }
                        try
                        {
                            mspField.TheoreticalMass = ConvertFormulaToAdductMass(mspField.AdductIon, mspField.Formula, mspField.IonMode);
                            Console.WriteLine(mspField.TheoreticalMass);
                            mspField.DiffPpm = (float)CommonUtility.PpmCalculator(mspField.TheoreticalMass, mspField.PrecursorMz);
                        }
                        catch
                        {
                            mspField.TheoreticalMass = -1;
                            mspField.DiffPpm = 0;
                        }
                        mspFields.Add(mspField);
                        mspField = new MspBean();
                        counter++;
                    }
                }
            }
            return mspFields;
        }

        public static List<Peak> ReadSpectrum(StreamReader sr, string numPeakField, out int peaknum)
        {
            peaknum = 0;
            var mspPeaks = new List<Peak>();

            if (int.TryParse(numPeakField.Split(':')[1].Trim(), out peaknum))
            {
                if (peaknum == 0) { return mspPeaks; }

                var pairCount = 0;
                var mspPeak = new Peak();

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
                                        mspPeak = new Peak();
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
                                    mspPeak = new Peak();
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
        private static float getCollisionEnergy(string ce)
        {
            string figure = string.Empty;
            float ceValue = 0.0f;
            for (int i = 0; i < ce.Length; i++)
            {
                if (Char.IsNumber(ce[i]) || ce[i] == '.')
                {
                    figure += ce[i];
                }
                else
                {
                    float.TryParse(figure, out ceValue);
                    return ceValue;
                }
            }
            float.TryParse(figure, out ceValue);
            return ceValue;
        }

        public static double ConvertFormulaToAdductMass(AdductIon adductIon, string formula, IonMode ion)
        {
            var mass = Utility.FormulaUtility.GetMass(formula);
            Console.WriteLine(mass);
            double adductMass = (mass * (double)adductIon.AdductIonXmer + adductIon.AdductIonAccurateMass) / (double)adductIon.ChargeNumber;
            Console.WriteLine(adductMass);
            if (ion == IonMode.Positive) adductMass -= 0.0005485799 * adductIon.ChargeNumber; else adductMass += 0.0005485799 * adductIon.ChargeNumber;
            Console.WriteLine(adductMass);
            return adductMass;
        }
    }
}
