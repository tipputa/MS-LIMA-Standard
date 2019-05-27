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
                spectra = ReadMgfFile.ReadAsMsSpectra(filePath);
                return spectra;
            }
            else if(extention == ".msp")
            {
                spectra = ReadMspFile.ReadAsMsSpectra(filePath);
                return spectra;
            }
            else if(extention == ".txt")
            {
                spectra = ReadMassBankFile.ReadAsMsSpectra(filePath);
                return spectra;
            }
            else
            {
                spectra = ReadMassBankFile.ReadAsMsSpectra(filePath);
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
                    double mz = -1;
                    double intensity = -1;
                    bool isCorrectForm = true;
                    var wkstr = sr.ReadLine();
                    if (wkstr == string.Empty) break;
                    string[] peak;
                    var str = wkstr.Trim();
                    if (str.Contains("\t"))
                    {
                        peak = str.Split('\t');
                    }
                    else if (str.Contains(" "))
                    {
                        peak = str.Split(' ');
                    }
                    else
                    {
                        peak = new string[0];
                    }

                    if (peak.Length < 2) break;

                    if (peak.Length >= 2)
                    {
                        if (double.TryParse(peak[0], out mz)) mspPeak.Mz = mz;
                        else isCorrectForm = false;
                        if (double.TryParse(peak[1], out intensity)) mspPeak.Intensity = intensity;
                        else isCorrectForm = false;
                        if (peak.Length == 3)
                        {
                            if (peak[2].Contains("\""))
                            {
                                mspPeak.Comment = peak[2].Split('\"')[1];
                                if (!string.IsNullOrEmpty(mspPeak.Comment))
                                {
                                    if (mspPeak.Comment.Contains("SMILES "))
                                    {
                                        var tmp = mspPeak.Comment.Split(new string[] { "SMILES " }, StringSplitOptions.None)[1];
                                        mspPeak.Smiles = tmp.Split(new string[] { "," }, StringSplitOptions.None)[0];
                                    }
                                    else if (mspPeak.Comment.Contains("SMILES: "))
                                    {
                                        var tmp = mspPeak.Comment.Split(new string[] { "SMILES: " }, StringSplitOptions.None)[1];
                                        mspPeak.Smiles = tmp.Split(new string[] { "," }, StringSplitOptions.None)[0];
                                    }
                                }
                            }
                        }
                    }
                    if (isCorrectForm) mspPeaks.Add(mspPeak);
                    mspPeak = new AnnotatedPeak();
                    pairCount++;
                }

                mspPeaks = mspPeaks.OrderBy(n => n.Mz).ToList();
            }
            return mspPeaks;
        }

        public static List<AnnotatedPeak> ReadSpectrum(StreamReader sr, int peaknum)
        {
            var mspPeaks = new List<AnnotatedPeak>();
            var wkstr = sr.ReadLine();
            if (peaknum == 0) { return mspPeaks; }

            var pairCount = 0;
            var mspPeak = new AnnotatedPeak();

            while (pairCount < peaknum)
            {
                double mz = -1;
                double intensity = -1;
                bool isCorrectForm = true;
                wkstr = sr.ReadLine();
                if (wkstr == string.Empty) break;
                var str = wkstr.Trim();
                string[] peak;
                if(str.Contains("\t"))
                {
                    peak = str.Split('\t');
                }
                else if (str.Contains(" "))
                {
                    peak = str.Split(' ');
                }
                else
                {
                    peak = new string[0];
                }

                if (peak.Length < 2) break;

                if (peak.Length >= 2)
                {
                    if (double.TryParse(peak[0].Trim(), out mz)) mspPeak.Mz = mz;
                    else isCorrectForm = false;
                    if (double.TryParse(peak[1].Trim(), out intensity)) mspPeak.Intensity = intensity;
                    else isCorrectForm = false;
                    if (peak.Length >= 3)
                    {
                        var com = "";
                        if(peak.Length > 3){
                            for(var i = 2; i < peak.Length - 1; i++)
                            {
                                com += peak[i] + " ";
                            }
                        }
                        else
                        {
                            com = peak[2];
                        }

                        if (com.Contains("\""))
                        {
                            mspPeak.Comment = com.Split('\"')[1];
                            if (!string.IsNullOrEmpty(mspPeak.Comment))
                            {
                                if (mspPeak.Comment.Contains("SMILES "))
                                {
                                    var tmp = mspPeak.Comment.Split(new string[] { "SMILES " }, StringSplitOptions.None)[1];
                                    mspPeak.Smiles = tmp.Split(new string[] { "," }, StringSplitOptions.None)[0];
                                }
                                else if (mspPeak.Comment.Contains("SMILES: "))
                                {
                                    var tmp = mspPeak.Comment.Split(new string[] { "SMILES: " }, StringSplitOptions.None)[1];
                                    mspPeak.Smiles = tmp.Split(new string[] { "," }, StringSplitOptions.None)[0];
                                }
                            }
                        }
                    }
                }
                if (isCorrectForm) mspPeaks.Add(mspPeak);
                mspPeak = new AnnotatedPeak();
                pairCount++;
            }

            mspPeaks = mspPeaks.OrderBy(n => n.Mz).ToList();
            
            return mspPeaks;
        }


        public static List<AnnotatedPeak> ReadSpectrum(StreamReader sr)
        {
            var mspPeaks = new List<AnnotatedPeak>();
            var pairCount = 0;
            var mspPeak = new AnnotatedPeak();
            while (true)
            {

                double mz = -1;
                double intensity = -1;
                bool isCorrectForm = true;
                var wkstr = sr.ReadLine();
                if (wkstr == string.Empty) break;
                if (Regex.IsMatch(wkstr, "END IONS")) break;
                var str = wkstr.Trim();
                string[] peak;
                if (str.Contains("\t"))
                {
                    peak = str.Split('\t');
                }
                else if (str.Contains(" "))
                {
                    peak = str.Split(' ');
                }
                else
                {
                    peak = new string[0];
                }

                if (peak.Length < 2) break;

                if (peak.Length >= 2)
                {
                    if (double.TryParse(peak[0].Trim(), out mz)) mspPeak.Mz = mz;
                    else isCorrectForm = false;
                    if (double.TryParse(peak[1].Trim(), out intensity)) mspPeak.Intensity = intensity;
                    else isCorrectForm = false;
                    if (peak.Length == 3)
                    {
                        if (peak[2].Contains("\""))
                        {
                            mspPeak.Comment = peak[2].Split('\"')[1];
                            if (!string.IsNullOrEmpty(mspPeak.Comment))
                            {
                                if (mspPeak.Comment.Contains("SMILES "))
                                {
                                    var tmp = mspPeak.Comment.Split(new string[] { "SMILES " }, StringSplitOptions.None)[1];
                                    mspPeak.Smiles = tmp.Split(new string[] { "," }, StringSplitOptions.None)[0];
                                }
                                else if (mspPeak.Comment.Contains("SMILES: "))
                                {
                                    var tmp = mspPeak.Comment.Split(new string[] { "SMILES: " }, StringSplitOptions.None)[1];
                                    mspPeak.Smiles = tmp.Split(new string[] { "," }, StringSplitOptions.None)[0];
                                }
                            }
                        }
                    }
                }
                if (isCorrectForm) mspPeaks.Add(mspPeak);
                mspPeak = new AnnotatedPeak();
                pairCount++;
            }

            mspPeaks = mspPeaks.OrderBy(n => n.Mz).ToList();
            return mspPeaks;
        }
    }
}
