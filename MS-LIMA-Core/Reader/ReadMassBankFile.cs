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
    public static class ReadMassBankFile
    {
        public static List<MassSpectrum> ReadMassBankFileAsSpectrum(string filePath)
        {
            var spectra = new List<MassSpectrum>();
            var spectrum = new MassSpectrum();
            spectrum.OtherMetaData = new List<string>();
            string wkstr;
            int counter = 0;

            using (StreamReader sr = new StreamReader(filePath, Encoding.ASCII))
            {
                float rt = 0, preMz = 0, ri = 0;

                while (sr.Peek() > -1)
                {
                    wkstr = sr.ReadLine();
                    if (Regex.IsMatch(wkstr, "^ACCESSION:.*", RegexOptions.IgnoreCase))
                    {
                        spectrum.Id = counter;
                        spectrum.OtherMetaData.Add(wkstr);

                        while (sr.Peek() > -1)
                        {
                            wkstr = sr.ReadLine();
                            if (wkstr == string.Empty || String.IsNullOrWhiteSpace(wkstr) || wkstr == "//") break;
                            if (Regex.IsMatch(wkstr, "^COMMENT.*:.*", RegexOptions.IgnoreCase))
                            {
                                if (string.IsNullOrEmpty(spectrum.Comment))
                                {
                                    spectrum.Comment = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                }
                                else
                                {
                                    spectrum.Comment += ";" + wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                                }
                                continue;
                            }

                            else if (Regex.IsMatch(wkstr, "^CH.?.*:.*", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(wkstr, "CH.?NAME", RegexOptions.IgnoreCase))
                                {
                                    spectrum.Name = MetadataParser.GetAfterChar(wkstr, ':');
                                    continue;
                                }
                                else if (Regex.IsMatch(wkstr, "CH.?FORMULA", RegexOptions.IgnoreCase))
                                {
                                    spectrum.Formula = MetadataParser.GetAfterChar(wkstr, ':');
                                    continue;
                                }
                                else if (Regex.IsMatch(wkstr, "CH.?SMILES", RegexOptions.IgnoreCase))
                                {
                                    spectrum.Smiles = MetadataParser.GetAfterChar(wkstr, ':');
                                    continue;
                                }
                                else if (Regex.IsMatch(wkstr, "CH.?IUPAC", RegexOptions.IgnoreCase))
                                {
                                    spectrum.InChI = MetadataParser.GetAfterChar(wkstr, '=');
                                    continue;
                                }

                                else if (Regex.IsMatch(wkstr, "CH.?LINK", RegexOptions.IgnoreCase))
                                {
                                    var wkstr2 = MetadataParser.GetAfterChar(wkstr, ':');
                                    var links = wkstr2.Split(' ');
                                    if (links[0].Contains("INCHIKEY"))
                                    {
                                        spectrum.InChiIKey = links[1];
                                        continue;
                                    }
                                    else
                                    {
                                        spectrum.OtherMetaData.Add(wkstr);
                                        continue;
                                    }

                                }
                                else
                                {
                                    spectrum.OtherMetaData.Add(wkstr);
                                    continue;
                                }
                            }

                            else if (Regex.IsMatch(wkstr, "^AC.?.*:.*", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(wkstr, "AC.?INSTRUMENT", RegexOptions.IgnoreCase))
                                {
                                    spectrum.Instrument = MetadataParser.GetAfterChar(wkstr, ':');
                                    continue;
                                }
                                else if (Regex.IsMatch(wkstr, "AC.?INSTRUMENT_TYPE", RegexOptions.IgnoreCase))
                                {
                                    spectrum.InstrumentType = MetadataParser.GetAfterChar(wkstr, ':');
                                    continue;
                                }
                                else if (Regex.IsMatch(wkstr, "AC.?MASS_SPECTROMETRY", RegexOptions.IgnoreCase))
                                {
                                    var wkstr2 = MetadataParser.GetAfterChar(wkstr, ':');
                                    var massInfo = wkstr2.Split(' ');
                                    if (Regex.IsMatch(massInfo[0], "ION_MODE", RegexOptions.IgnoreCase))
                                    {
                                        if (massInfo[1].Contains("POSITIVE"))
                                        {
                                            spectrum.IonMode = IonMode.Positive;
                                            continue;
                                        }
                                        else if (massInfo[1].Contains("NEGATIVE"))
                                        {
                                            spectrum.IonMode = IonMode.Negative;
                                            continue;
                                        }
                                        else
                                        {
                                            spectrum.OtherMetaData.Add(wkstr);
                                            continue;
                                        }
                                    }
                                    else if (Regex.IsMatch(massInfo[0], "COLLISION_ENERGY", RegexOptions.IgnoreCase))
                                    {
                                        spectrum.CollisionEnergy = MspParser.GetCollisionEnergy(massInfo[1].Trim());
                                        continue;
                                    }
                                    else
                                    {
                                        spectrum.OtherMetaData.Add(wkstr);
                                        continue;
                                    }
                                }
                                else if (Regex.IsMatch(wkstr, "AC.?CHROMATOGRAPHY", RegexOptions.IgnoreCase))
                                {
                                    var wkstr2 = MetadataParser.GetAfterChar(wkstr, ':');
                                    var chromInfo = wkstr2.Split(' ');
                                    if (Regex.IsMatch(chromInfo[0], "RETENTION_TIME", RegexOptions.IgnoreCase))
                                    {
                                        if (float.TryParse(chromInfo[1].Trim(), out rt))
                                        {
                                            if (chromInfo.Length == 3 && chromInfo[2] == "sec")
                                            {
                                                spectrum.RetentionTime = MetadataParser.ConvertSecToMin(rt);
                                            }
                                            else
                                            {
                                                spectrum.RetentionTime = rt;
                                            }
                                        }
                                        else spectrum.RetentionTime = -1;
                                        continue;
                                    }
                                    else
                                    {
                                        spectrum.OtherMetaData.Add(wkstr);
                                        continue;
                                    }

                                }
                                else
                                {
                                    spectrum.OtherMetaData.Add(wkstr);
                                    continue;
                                }
                            }

                            else if (Regex.IsMatch(wkstr, "^MS.?.*:.*", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(wkstr, "MS.?FOCUSED_ION:", RegexOptions.IgnoreCase))
                                {
                                    var wkstr2 = MetadataParser.GetAfterChar(wkstr, ':');
                                    var precursorInfo = wkstr2.Split(' ');
                                    if(Regex.IsMatch(precursorInfo[0], "PRECURSOR_M/Z", RegexOptions.IgnoreCase)){
                                        if (float.TryParse(precursorInfo[1].Trim(), out preMz))
                                        {
                                            spectrum.PrecursorMz = preMz;
                                            continue;
                                        }
                                    }else if (Regex.IsMatch(precursorInfo[0], "PRECURSOR_TYPE", RegexOptions.IgnoreCase))
                                    {
                                        spectrum.AdductIon = AdductIonParser.GetAdductIon(precursorInfo[1].Trim());
                                        continue;
                                    }
                                    else
                                    {
                                        spectrum.OtherMetaData.Add(wkstr);
                                        continue;
                                    }
                                }
                                else
                                {
                                    spectrum.OtherMetaData.Add(wkstr);
                                    continue;
                                }
                            }

                            else if (Regex.IsMatch(wkstr, "^PK.?.*:.*", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(wkstr, "PK.?NUM_PEAK", RegexOptions.IgnoreCase))
                                {
                                    var numPeaks = int.Parse(MetadataParser.GetAfterChar(wkstr, ':'));
                                    spectrum.Spectrum = ReadFile.ReadSpectrum(sr, numPeaks);
                                    continue;
                                }
                                else
                                {
                                    spectrum.OtherMetaData.Add(wkstr);
                                }
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
                                spectrum.CollisionEnergy = MspParser.GetCollisionEnergy(wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim());
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
                                spectrum.Spectrum = ReadFile.ReadSpectrum(sr, wkstr, out int peakNum);
                                continue;
                            }
                            else if (wkstr.Contains(':'))
                            {
                                spectrum.OtherMetaData.Add(wkstr);
                                continue;
                            }
                        }
                        try
                        {
                            if (spectrum.TheoreticalMass <= 0)
                            {
                                spectrum.TheoreticalMass = MspParser.ConvertFormulaToAdductMass(spectrum.AdductIon, spectrum.Formula, spectrum.IonMode);
                            }
                            spectrum.DiffPpm = (float)CommonUtility.PpmCalculator(spectrum.TheoreticalMass, spectrum.PrecursorMz);
                        }
                        catch
                        {
                            spectrum.TheoreticalMass = -1;
                            spectrum.DiffPpm = 0;
                        }
                        spectra.Add(spectrum);
                        spectrum = new MassSpectrum();
                        spectrum.OtherMetaData = new List<string>();
                        counter++;
                    }
                }

                return spectra;
            }
        }
    }
}
