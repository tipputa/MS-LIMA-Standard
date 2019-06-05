using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Metabolomics.MsLima.Bean;


namespace Metabolomics.MsLima.Reader
{
    public static class ReadTemporalFile
    {
        public static List<TemporaryFile> GetTextBasedInChIKeyLibraryFile(string filePath, out List<string> errorMessage)
        {
            var textQueries = new List<TemporaryFile>();
            var textQuery = new TemporaryFile();

            string line, adductString = string.Empty;
            errorMessage = new List<string>();
            string[] lineArray;
            int counter = 1;

            using (StreamReader sr = new StreamReader(filePath, Encoding.ASCII))
            {
                sr.ReadLine();

                while (sr.Peek() > -1)
                {
                    #region
                    line = sr.ReadLine();
                    counter++;

                    if (line == string.Empty) break;

                    lineArray = line.Split('\t');

                    if (lineArray.Length != 3) { errorMessage.Add("Error type 1: line " + counter + " is not suitable."); continue; }

                    textQuery = new TemporaryFile();
                    textQuery.InChIKey = lineArray[0];
                    textQuery.SMILES = lineArray[2];
                    textQuery.InChI = lineArray[1];

                    #endregion

                    textQueries.Add(textQuery);
                }

                if (textQueries.Count == 0)
                {
                    errorMessage.Add("Error type 1: line " + counter + " is not suitable.");
                }
            }

            if (errorMessage.Count > 0)
            {
                return null;
            }

            return textQueries;
        }


        public static CommonMetaData GetCommonMetaDataFromFile(string filePath)
        {
            string errorMessage = string.Empty;
            string wkstr;
            var mspField = new CommonMetaData();
            using (StreamReader sr = new StreamReader(filePath, Encoding.ASCII))
            {
                wkstr = sr.ReadLine();

                while (sr.Peek() > -1)
                {
                    wkstr = sr.ReadLine();
                    if (wkstr == string.Empty || String.IsNullOrWhiteSpace(wkstr)) break;
                    if (Regex.IsMatch(wkstr, "^COMMENT.?:.*", RegexOptions.IgnoreCase))
                    {
                        mspField.Comment = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                        continue;
                    }
                    else if (Regex.IsMatch(wkstr, "^AUTHORS?:.*", RegexOptions.IgnoreCase))
                    {
                        mspField.Authors = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                        continue;
                    }
                    else if (Regex.IsMatch(wkstr, "^INSTRUMENT:.*", RegexOptions.IgnoreCase))
                    {
                        mspField.Instrument = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                        continue;
                    }
                    else if (Regex.IsMatch(wkstr, "^INSTRUMENTTYPE:.*", RegexOptions.IgnoreCase))
                    {
                        mspField.InstrumentType = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                        continue;
                    }
                    else if (Regex.IsMatch(wkstr, "^MSLEVEL:.*", RegexOptions.IgnoreCase))
                    {
                        mspField.MsLevel = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                        continue;
                    }

                    else if (Regex.IsMatch(wkstr, "^LICENSE:.*", RegexOptions.IgnoreCase))
                    {
                        mspField.License = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                        continue;
                    }

                    else if (Regex.IsMatch(wkstr, "^SPECTRUMTYPE:.*", RegexOptions.IgnoreCase))
                    {
                        mspField.SpectrumType = wkstr.Substring(wkstr.Split(':')[0].Length + 2).Trim();
                        continue;

                    }
                }
                return mspField;
            }
        }
    }
}
