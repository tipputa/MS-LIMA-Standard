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
    public static class ReadMspFile
    {

        public static List<MassSpectrum> ReadAsMsSpectra(string filePath)
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
                                spectrum.InChIKey = wkstr.Split(':')[1].Trim();
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
                            else if (Regex.IsMatch(wkstr, "MS.?LEVEL:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.MsLevel = wkstr.Split(':')[1].Trim();
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "COMPOUNDCLASS:.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.CompoundClass = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
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
                            else
                            {
                                spectrum.OtherMetaData.Add(wkstr);
                                continue;
                            }
                        }
                        try
                        {
                            spectrum.TheoreticalMass = MspParser.ConvertFormulaToAdductMass(spectrum.AdductIon, spectrum.Formula, spectrum.IonMode);
                        }
                        catch
                        {
                            spectrum.TheoreticalMass = -1;
                        }
                        spectra.Add(spectrum);
                        spectrum = new MassSpectrum();
                        counter++;
                    }
                }
            }
            return spectra;
        }

    }
}
