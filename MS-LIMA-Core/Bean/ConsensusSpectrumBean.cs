using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Metabolomics.Core;
using Metabolomics.Core.Parser;

namespace Metabolomics.MsLima.Bean
{
    public class SMILES
    {
        public int Counter { get; set; }
        public string Smiles { get; set; }
        public string Formula { get; set; }
        public SMILES(string smiles, string formula) { Counter = 1; Smiles = smiles; Formula = formula; }
    }

    public class FORMULA
    {
        public int Counter { get; set; }
        public string Formula { get; set; }
        public FORMULA(string formula) { Counter = 1; Formula = formula; }
    }

    public class Isotope
    {
        public int Counter { get; set; }
        public double LinkedMz { get; set; }
        public string Label { get; set; }
        public Isotope(string name, double mz) { Counter = 1; Label = name; LinkedMz = mz; }
    }

    public class Adduct
    {
        public int Counter { get; set; }
        public double LinkedMz { get; set; }
        public string AdductName { get; set; }
        public AdductIon AdductIon { get; set; }
        public Adduct(string adduct, double mz) { this.AdductIon = AdductIonParser.GetAdductIon(adduct); Counter = 1; AdductName = adduct; LinkedMz = mz; }
    }

    public class MsGroup
    {
        public int Counter { get; set; }
        public double Mz { get; set; }
        public double MinMz { get; set; }
        public double MaxMz { get; set; }
        public double MedianMz { get; set; }
        public double MinIntensity { get; set; }
        public double MaxIntensity { get; set; }
        public double MedianIntensity { get; set; }
        public SMILES CommonSMILES { get; set; }
        public FORMULA CommonFORMULA { get; set; }
        public List<SMILES> SMILES { get; set; }
        public List<FORMULA> Formula { get; set; }
        public Adduct CommonAdduct { get; set; }
        public Isotope Isotope { get; set; }

        public double[] MzList { get; set; }
        public double[] IntList { get; set; }
        public MsGroup() { }
        public MsGroup(int files)
        {
            this.MzList = new double[files];
            this.IntList = new double[files];
        }
        public MsGroup(int files, double mz, double intensity, int index, string comment)
        {
            this.MzList = new double[files];
            this.IntList = new double[files];
            this.SMILES = new List<SMILES>();
            this.Formula = new List<FORMULA>();
            this.MzList[index] = mz;
            this.IntList[index] = intensity;
            var formula = convertComment2Formulra(comment);
            var smiles = ConvertComment2Smiles(comment);
            this.CommonAdduct = ConvertComment2Adduct(comment);
            this.Isotope = ConvertComment2Isotope(comment);
            if (formula != "") Formula.Add(new FORMULA(formula));
            if (smiles != null && smiles.Count > 0) { SMILES.Add(new SMILES(smiles[0], smiles[1])); }// Formula.Add(new FORMULA(smiles[2])); }
            this.Mz = mz;
            this.Counter = 1;
        }

        public static string convertComment2Formulra(string comment)
        {
            if (comment.Contains("Formula: "))
                return comment.Split(new string[] { "Formula: " }, StringSplitOptions.None)[1];
            else if (comment.Contains("Formula "))
                return comment.Split(new string[] { "Formula " }, StringSplitOptions.None)[1];
            else
                return "";
        }

        public static List<string> ConvertComment2Smiles(string comment)
        {
            var results = new List<string>();
            if (comment.Contains("SMILES: "))
            {
                var tmp = comment.Split(new string[] { "SMILES: " }, StringSplitOptions.None)[1];
                results.Add(tmp.Split(new string[] { "," }, StringSplitOptions.None)[0]);
                tmp = comment.Split(new string[] { "Annotation: " }, StringSplitOptions.None)[1];
                var annotation = tmp.Split(new string[] { "," }, StringSplitOptions.None)[0];
                results.Add(annotation);
                //results.Add(ConvertString2FormulaString(annotation));
                return results;
            }
            else if (comment.Contains("SMILES"))
            {
                var tmp = comment.Split(new string[] { "SMILES " }, StringSplitOptions.None)[1];
                results.Add(tmp.Split(new string[] { "," }, StringSplitOptions.None)[0]);
                tmp = comment.Split(new string[] { "Annotation " }, StringSplitOptions.None)[1];
                var annotation = tmp.Split(new string[] { "," }, StringSplitOptions.None)[0];
                results.Add(annotation);
                //results.Add(ConvertString2FormulaString(annotation));
                return results;

            }

            else return results;
        }

        public static Isotope ConvertComment2Isotope(string comment)
        {
            if (comment.Contains("Isotopic ion"))
            {
                try
                {
                    var tmp = comment.Split(new string[] { "Isotopic ion," }, StringSplitOptions.None)[1];
                    var tmp2 = tmp.Split(',');
                    var mz = tmp2[1].Trim().Split(' ')[1].Trim();
                    double d;
                    if (double.TryParse(mz, out d))
                    {
                        return new Isotope(tmp2[0].Trim(), d);
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public static Adduct ConvertComment2Adduct(string comment)
        {
            if (comment.Contains("Adduct ion"))
            {
                try
                {
                    var tmp = comment.Split(new string[] { "Adduct ion, " }, StringSplitOptions.None)[1];
                    var tmp2 = tmp.Split(',');
                    var mz = tmp2[1].Trim().Split(' ')[1].Trim();
                    double d;
                    if (double.TryParse(mz, out d))
                    {
                        return new Adduct(tmp2[0].Trim(), d);
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

    }

    public class MsGrouping
    {
        /*
        public static void Excute_All(MainWindow mainWindow, MainWindowVM mainWindowVM)
        {
            var output = mainWindowVM.Data.ImportFileDir + "New_msp.msp";
            using (var sw = new StreamWriter(output, false, Encoding.ASCII))
            {
                var numComp = mainWindowVM.Data.rawData.Count;
                for (var j = 0; j < numComp; j++)
                {
                    var MsGroupList = new List<MsGroup>();
                    var num = mainWindowVM.Data.rawData[j].DataList.Count;
                    for (var i = 0; i < num; i++)
                    {
                        var rawData = (MspFormatCompoundInformationBean)mainWindowVM.Data.rawData[j].DataList[i];
                        var peaks = rawData.MzIntensityCommentBeanList.OrderBy(n => n.Mz).ToList();
                        var maxInt = peaks.Max(x => x.Intensity);
                        foreach (var peak in peaks)
                        {
                            CheckPeak(MsGroupList, peak, num, i, maxInt);
                        }
                    }
                    Finalizer(MsGroupList);
                    Debug.WriteLine("ID: " + j + ", Num Ms: " + num);
                    WriteAllmsGroup(mainWindowVM.Data.rawData[j], MsGroupList, 1, sw);
                }
            }
        }
        */
        public static void ExcuteAll(StreamWriter sw, List<CompoundBean> compounds)
        {
            var MsGroupList = new List<MsGroup>();
            var num = 0;
            foreach (var comp in compounds)
            {
                foreach (var spec in comp.Spectra)
                {
                    num++;
                }
            }
            var i = 0;
            foreach (var comp in compounds)
            {
                foreach (var spec in comp.Spectra)
                {
                    var peaks = spec.Spectrum.OrderBy(n => n.Mz).ToList();
                    var maxInt = peaks.Max(x => x.Intensity);
                    foreach (var peak in peaks)
                    {
                        CheckPeak(MsGroupList, peak, num, i, maxInt);
                    }
                    i++;
                }
            }
            Finalizer(MsGroupList);
            var counter = num;
            foreach (var g in MsGroupList)
            {
                var comment = "";
                if (g.CommonSMILES != null)
                {
                    comment += "SMILES " + g.CommonSMILES.Smiles + ", NumSmiles " + g.SMILES.Count();
                    if (g.SMILES.Count > 1)
                    {
                        comment += " {";
                        var tmp = new List<string>();
                        foreach (var s in g.SMILES)
                        {
                            tmp.Add(s.Smiles + "(" + s.Counter + ")");
                        }
                        comment += string.Join(", ", tmp);
                        comment += "}; ";
                    }
                    else comment += "; ";
                }
                if (g.CommonFORMULA != null)
                {
                    comment += "Formula " + g.CommonFORMULA.Formula + ", numFormula " + g.Formula.Count();
                    if (g.Formula.Count > 1)
                    {
                        comment += " {";
                        var tmp = new List<string>();
                        foreach (var f in g.Formula)
                        {
                            tmp.Add(f.Formula + "(" + f.Counter + ")");
                        }
                        comment += string.Join(", ", tmp);
                        comment += "}; ";
                    }
                    else comment += "; ";
                }
                if (g.CommonAdduct != null)
                {
                    comment += "Adduct " + g.CommonAdduct.AdductName + ", linkedMz " + g.CommonAdduct.LinkedMz + "; ";
                }
                if (g.Isotope != null)
                {
                    comment += "Isotope " + g.Isotope.Label + ", linkedMz " + g.Isotope.LinkedMz + "; ";
                }

                sw.WriteLine(g.MedianMz + "\t" + g.MedianIntensity + "\t" + g.Counter + "\t" + Math.Round(((double)g.Counter / (double)counter * 100.0), 1) + "\t" + comment);
            }
            
        }
        

        public static List<MsGroup> Excute(CompoundBean comp)
        {
            var MsGroupList = new List<MsGroup>();
            var num = comp.Spectra.Count();
            var i = 0;
            foreach(var spec in comp.Spectra) {
                var peaks = spec.Spectrum.OrderBy(n => n.Mz).ToList();
                var maxInt = peaks.Max(x => x.Intensity);
                foreach (var peak in peaks)
                {
                    CheckPeak(MsGroupList, peak, num, i, maxInt);
                }
                i++;
            }
            Finalizer(MsGroupList);
            MsGroupList = MsGroupList.OrderBy(x => x.MedianMz).ToList();
            return MsGroupList;
        }

    /*
        public static void Excute2(StreamWriter sw, MainWindow mainWindow, MainWindowVM mainWindowVM)
        {
            foreach (var raw in mainWindowVM.Data.rawData)
            {
                Excute3(sw, raw.DataList);
            }
        }
        public static void Excute3(StreamWriter sw, List<MassSpectrum> msps)
        {
            var num = msps.Count;
            var CeList = msps.Select(x => x.CollisionEnergy).Distinct().ToList();
            foreach (var ce in CeList)
            {
                var MsGroupList = new List<MsGroup>();
                var counter = 0;
                for (var i = 0; i < num; i++)
                {
                    var rawData = (MassSpectrum)msps[i];
                    if (ce != rawData.CollisionEnergy) continue;
                    counter++;
                    var peaks = rawData.Spectrum.OrderBy(n => n.Mz).ToList();
                    if (peaks.Count == 0) continue;

                    var maxInt = peaks.Max(x => x.Intensity);
                    foreach (var peak in peaks)
                    {
                        CheckPeak(MsGroupList, peak, num, i, maxInt);
                    }
                }
                Finalizer(MsGroupList);
                MsGroupList = MsGroupList.OrderBy(x => x.MedianMz).ToList();
                var targetMsGroupList = counter == 1 ? MsGroupList : MsGroupList.Where(x => x.Counter >= (float)counter / 2.0).ToList();
                if (targetMsGroupList.Count == 0) continue;

                var rawData2 = (MspFormatCompoundInformationBean)msps[0];
                sw.WriteLine("NAME: " + rawData2.Name);
                sw.WriteLine("RETENTIONTIME: " + rawData2.RetentionTime);
                sw.WriteLine("PRECURSORMZ: " + rawData2.PrecursorMz);
                sw.WriteLine("PRECURSORTYPE: " + rawData2.AdductIonBean.AdductIonName);
                sw.WriteLine("IONMODE: " + rawData2.IonMode);
                sw.WriteLine("COLLISIONENERGY: " + ce);
                sw.WriteLine("FORMULA: " + rawData2.Formula);
                sw.WriteLine("SMILES: " + rawData2.Smiles);
                sw.WriteLine("INCHIKEY: " + rawData2.InchiKey);
                sw.WriteLine("INCHI: " + rawData2.InChI);
                sw.WriteLine("SPECTRUMTYPE: " + rawData2.SpectrumType);
                sw.WriteLine("AUTHORS: " + rawData2.Authors);
                sw.WriteLine("INSTRUMENT: " + rawData2.Instrument);
                sw.WriteLine("INSTRUMENTTYPE: " + rawData2.InstrumentType);
                sw.WriteLine("LICENSE: " + rawData2.License);
                sw.WriteLine("COMMENT: " + counter + " spectra were merged");
                sw.WriteLine("Num Peaks: " + targetMsGroupList.Count);
                foreach (var g in targetMsGroupList)
                {
                    if (g.CommonSMILES != null)
                    {
                        sw.WriteLine(g.MedianMz + "\t" + g.MedianIntensity + "\t\"" + g.Counter + " spectra (" + Math.Round(((double)g.Counter / (double)counter * 100.0), 1) + "%), " + g.CommonSMILES.Smiles + "\"");
                    }
                    else if (g.CommonFORMULA != null)
                    {
                        sw.WriteLine(g.MedianMz + "\t" + g.MedianIntensity + "\t\"" + g.Counter + " spectra (" + Math.Round(((double)g.Counter / (double)counter * 100.0), 1) + "%), " + g.CommonFORMULA.Formula + "\"");
                    }
                    else if (g.CommonAdduct != null)
                    {
                        sw.WriteLine(g.MedianMz + "\t" + g.MedianIntensity + "\t\"" + g.Counter + " spectra (" + Math.Round(((double)g.Counter / (double)counter * 100.0), 1) + "%), " + g.CommonAdduct.AdductName + ", linkedMz " + g.CommonAdduct.LinkedMz + "\"");
                    }
                    else if (g.Isotope != null)
                    {
                        sw.WriteLine(g.MedianMz + "\t" + g.MedianIntensity + "\t\"" + g.Counter + " spectra (" + Math.Round(((double)g.Counter / (double)counter * 100.0), 1) + "%), " + g.Isotope.Label + ", linkedMz " + g.Isotope.LinkedMz + "\"");
                    }
                    else
                    {
                        sw.WriteLine(g.MedianMz + "\t" + g.MedianIntensity + "\t\"" + g.Counter + " spectra (" + Math.Round(((double)g.Counter / (double)counter * 100.0), 1) + "%)\"");
                    }
                }
                sw.WriteLine("");
            }
        }

    */
        public static void CheckPeak(List<MsGroup> msGroupList, AnnotatedPeak peak, int num, int id, double maxIntensity)
        {
            var checker = false;
            var relInt = 100 * peak.Intensity / maxIntensity;
            if (msGroupList == null)
            {
                msGroupList.Add(new MsGroup(num, peak.Mz, relInt, id, peak.Comment));
            }
            foreach (var msGroup in msGroupList)
            {
                if (Math.Abs(msGroup.Mz - peak.Mz) < 0.01)
                {
                    msGroup.MzList[id] = peak.Mz;
                    msGroup.IntList[id] = relInt;
                    msGroup.Counter++;
                    AddComment(msGroup, peak.Comment);
                    checker = true;
                }
            }
            if (checker == false)
                msGroupList.Add(new MsGroup(num, peak.Mz, relInt, id, peak.Comment));
        }

        public static void AddComment(MsGroup msGroup, string comment)
        {
            var formula = MsGroup.convertComment2Formulra(comment);
            var smiles = MsGroup.ConvertComment2Smiles(comment);
            AddFormula(msGroup.Formula, formula);
            AddSmiles(msGroup.SMILES, msGroup.Formula, smiles);
            if (msGroup.Formula.Count == 0 && msGroup.SMILES.Count == 0 && msGroup.Isotope == null && msGroup.CommonAdduct == null)
            {
                msGroup.Isotope = MsGroup.ConvertComment2Isotope(comment);
                msGroup.CommonAdduct = MsGroup.ConvertComment2Adduct(comment);
            }
        }

        public static void AddFormula(List<FORMULA> Formula, string formula)
        {
            var checker = true;
            if (formula != "")
            {
                foreach (var f in Formula)
                {
                    if (f.Formula == formula)
                    {
                        checker = false;
                        f.Counter++;
                        break;
                    }
                }
                if (checker) Formula.Add(new FORMULA(formula));
            }
        }

        public static void AddSmiles(List<SMILES> Smiles, List<FORMULA> Formula, List<string> smiles)
        {
            var checker = true;
            if (smiles != null && smiles.Count > 0)
            {
                foreach (var s in Smiles)
                {
                    if (s.Smiles == smiles[0])
                    {
                        checker = false;
                        s.Counter++;
                        break;
                    }
                }
                if (checker) Smiles.Add(new SMILES(smiles[0], smiles[1]));
                //AddFormula(Formula, smiles[2]);
            }

        }

        public static void Finalizer(List<MsGroup> msGroupList)
        {
            foreach (var msGroup in msGroupList)
            {
                msGroup.MinMz = msGroup.MzList.Where(x => x > 0).Min();
                msGroup.MaxMz = msGroup.MzList.Max();
                msGroup.MinIntensity = msGroup.IntList.Where(x => x > 0).Min();
                msGroup.MaxIntensity = msGroup.IntList.Max();
                msGroup.MedianIntensity = Median(msGroup.IntList);
                msGroup.MedianMz = Median(msGroup.MzList);
                if (msGroup.SMILES != null && msGroup.SMILES.Count > 0)
                    msGroup.CommonSMILES = msGroup.SMILES.First(x => x.Counter.Equals(msGroup.SMILES.Max(y => y.Counter)));
                if (msGroup.Formula != null && msGroup.Formula.Count > 0)
                    msGroup.CommonFORMULA = msGroup.Formula.First(x => x.Counter.Equals(msGroup.Formula.Max(y => y.Counter)));
            }
        }

        public static void CheckAllmsGroup(List<MsGroup> msGroupList, int threshold)
        {
            Debug.WriteLine("Num: " + msGroupList.Count);
            msGroupList = msGroupList.OrderByDescending(x => x.Counter).ToList();
            foreach (var msGroup in msGroupList)
            {
                if (msGroup.Counter >= threshold)
                {
                    var comment = "";
                    if (msGroup.CommonFORMULA != null)
                        if (msGroup.CommonSMILES != null)
                            comment = msGroup.CommonFORMULA.Formula + " (" + msGroup.CommonFORMULA.Counter + ")\t" + msGroup.CommonSMILES.Smiles + "(" + msGroup.CommonSMILES.Counter + ")";
                        else
                            comment = msGroup.CommonFORMULA.Formula + " (" + msGroup.CommonFORMULA.Counter + ")\t";
                    else if (msGroup.CommonSMILES != null)
                        comment = msGroup.CommonSMILES.Smiles + "(" + msGroup.CommonSMILES.Counter + ")";

                    Debug.WriteLine(r(msGroup.MedianMz) + "\t" + r(msGroup.MedianIntensity) + "\t" + r(msGroup.Counter) + "\t" + r(msGroup.MinMz) + "-" + r(msGroup.MaxMz) + "\t" + r(msGroup.MinIntensity) + "-" + r(msGroup.MaxIntensity) + comment);
                }
            }
        }

        public static void WriteAllmsGroup(CompoundBean comp, List<MsGroup> msGroupList, int threshold, StreamWriter sw)
        {
            msGroupList = msGroupList.OrderBy(x => x.MedianMz).ToList();
            var spectrum = new List<AnnotatedPeak>();
            foreach (var msGroup in msGroupList)
            {
                if (msGroup.Counter >= threshold)
                {
                    var com = "\"Count: " + r(msGroup.Counter) + ", mzRange: " + r(msGroup.MinMz) + "-" + r(msGroup.MaxMz) + ", IntRange: " + r(msGroup.MinIntensity) + "-" + r(msGroup.MaxIntensity) + "\"";
                    var peak = new AnnotatedPeak() { Mz = r(msGroup.MedianMz), Intensity = r(msGroup.MedianIntensity), Comment = com };
                    spectrum.Add(peak);
                }
            }
            sw.WriteLine("NAME: " + comp.Name);
            sw.WriteLine("RETENTIONTIME: " + comp.RetentionTime);
            sw.WriteLine("PRECURSORMZ: " + (comp.MolecularWeight + 1.007277));
            sw.WriteLine("PRECURSORTYPE: [M+H]+");
            sw.WriteLine("IONMODE: Positive");
            sw.WriteLine("FORMULA: " + comp.Formula);
            sw.WriteLine("SMILES: " + comp.Smiles);
            sw.WriteLine("INCHIKEY: " + comp.InChIKey);
            sw.WriteLine("Num Peaks:" + spectrum.Count);
            foreach (var mzIntcomment in spectrum)
            {
                sw.WriteLine(mzIntcomment.Mz + "\t" + mzIntcomment.Intensity + "\t" + mzIntcomment.Comment);
            }
            sw.WriteLine();
        }

        private static float r(double val)
        {
            return (float)Math.Round(val, 3);
        }

        private static double Median(double[] value)
        {
            value = value.Where(x => x > 0).OrderBy(x => x).ToArray();
            var num = value.Length; var checker = false; var index = 0;
            if (num == 1) return value[0];
            if (num % 2 == 0) { index = num / 2; } else { checker = true; index = num / 2 - 1; }
            if (checker) return (value[index] + value[index + 1]) / 2;
            else return value[index];
        }
    }
}
