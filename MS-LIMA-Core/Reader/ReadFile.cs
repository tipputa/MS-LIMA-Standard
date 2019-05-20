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
                spectra = ReadMspFile.ReadMspFileAsMsSpectrum(filePath);
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
