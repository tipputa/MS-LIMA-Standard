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
    public static class ReadMgfFile
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
                    if (Regex.IsMatch(wkstr, "^BEGIN IONS", RegexOptions.IgnoreCase))
                    {
                        spectrum.Id = counter;
                        while (sr.Peek() > -1)
                        {
                            wkstr = sr.ReadLine();
                            if (Regex.IsMatch(wkstr, "END IONS")) break;
                            if (Regex.IsMatch(wkstr, "NAME=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Name = MetadataParser.GetAfterChar(wkstr, '=');
                            }
                            else if (Regex.IsMatch(wkstr, "^COMMENT.*=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Comment = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "AUTHORS.*=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Authors = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INSTRUMENT.*=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Instrument = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INSTRUMENT.?TYPE.*=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.InstrumentType = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "LICENSE=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.License = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }

                            else if (Regex.IsMatch(wkstr, "SPECTRUM.?TYPE=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.SpectrumType = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "FORMULA=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Formula = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "ION.?MODE=.*", RegexOptions.IgnoreCase))
                            {
                                if (MetadataParser.GetAfterChar(wkstr, '=') == "Negative") spectrum.IonMode = IonMode.Negative;
                                else spectrum.IonMode = IonMode.Positive;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "SMILES=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Smiles = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "COLLISION.?ENERGY=", RegexOptions.IgnoreCase))
                            {
                                spectrum.CollisionEnergy = MspParser.GetCollisionEnergy(MetadataParser.GetAfterChar(wkstr, '='));
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INCHIKEY=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.InChiIKey = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "INCHI=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.InChI = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "COMPOUNDCLASS=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.CompoundClass = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RETENTIONTIME=.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(MetadataParser.GetAfterChar(wkstr, '='), out rt)) spectrum.RetentionTime = rt; else spectrum.RetentionTime = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RT=.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(MetadataParser.GetAfterChar(wkstr, '='), out rt)) spectrum.RetentionTime = rt; else spectrum.RetentionTime = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RETENTIONINDEX=.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(MetadataParser.GetAfterChar(wkstr, '='), out ri)) spectrum.RetentionIndex = ri; else spectrum.RetentionIndex = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "RI=.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(MetadataParser.GetAfterChar(wkstr, '='), out ri)) spectrum.RetentionIndex = ri; else spectrum.RetentionIndex = -1;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "PRECURSORMZ=.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(MetadataParser.GetAfterChar(wkstr, '='), out preMz)) spectrum.PrecursorMz = preMz;
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "PEPMASS=.*", RegexOptions.IgnoreCase))
                            {
                                if (float.TryParse(MetadataParser.GetAfterChar(wkstr, '='), out preMz)) spectrum.PrecursorMz = preMz;
                                continue;
                            }

                            else if (Regex.IsMatch(wkstr, "PRECURSOR.?TYPE=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.AdductIon = AdductIonParser.GetAdductIon(MetadataParser.GetAfterChar(wkstr, '='));
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "COMPOUNDCLASS=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.CompoundClass = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "Links=.*", RegexOptions.IgnoreCase))
                            {
                                spectrum.Links = MetadataParser.GetAfterChar(wkstr, '=');
                                continue;
                            }
                            else if (Regex.IsMatch(wkstr, "SCANS=", RegexOptions.IgnoreCase))
                            {
                                var test = ReadFile.ReadSpectrum(sr);
                                spectrum.Spectrum = test;
                                
                                break;
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

    }
}
