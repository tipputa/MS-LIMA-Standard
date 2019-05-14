using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Metabolomics.Core;
using Metabolomics.Core.Parser;
using Metabolomics.Core.Utility;
using Metabolomics.MsLima.Reader;

namespace Metabolomics.MsLima.Bean
{
    public class DataStorageBean
    {
        #region Properties
        public List<MassSpectrum> RawLibraryFile { get; set; }
        public List<MassSpectrum> ModifiedRawMspFile { get; set; }
        public List<CompoundBean> CompoundList { get; set; }

        public string FilePath { get; set; }
        public LibraryFileFormat FileFormat { get; set; }
        #endregion

        public DataStorageBean() { }

        public void SetLibrary(string filePath)
        {
            FilePath = filePath;
            ReadLibraryFile();
            CreateCompoundList();
        }

        public void ReadLibraryFile()
        {
            var extention = Path.GetExtension(FilePath).ToLower();
            if (extention == ".mgf")
            {
                this.FileFormat = LibraryFileFormat.Mgf;
                this.RawLibraryFile = ReadFile.ReadMgfFileAsMsSpectrum(FilePath);
            }
            else if (extention == ".msp")
            {
                this.FileFormat = LibraryFileFormat.Msp;
                this.RawLibraryFile = ReadFile.ReadMspFileAsMsSpectrum(FilePath);
            }
            else if (extention == ".massbank")
            {
                this.FileFormat = LibraryFileFormat.MassBank;
                this.RawLibraryFile = ReadFile.ReadMassBankFileAsMsSpectrum(FilePath);
            }
            else if (extention == ".txt")
            {
                this.FileFormat = LibraryFileFormat.Text;
                this.RawLibraryFile = ReadFile.ReadTextFileAsMsSpectrum(FilePath);
            }
            else
            {
                this.FileFormat = LibraryFileFormat.Text;
                this.RawLibraryFile = ReadFile.ReadTextFileAsMsSpectrum(FilePath);
            }
        }

        public void CreateCompoundList()
        {
            CompoundBean comp;
            var dic = new Dictionary<string, CompoundBean>();
            var InchiKeys = new List<string>();

            var counter = 1;
            //            mspFields.Where(msp => msp.InchiKey.Contains())
            foreach (var spectrum in RawLibraryFile)
            {
                var Inchikey = spectrum.InChiIKey.Split('-')[0];
                if (InchiKeys.Contains(Inchikey))
                {
                    dic[Inchikey].Spectra.Add(spectrum);
                    dic[Inchikey].NumSpectra++;
                }
                else
                {
                    comp = new CompoundBean
                    {
                        Id = counter,
                        InChIKey = spectrum.InChiIKey,
                        InChI = spectrum.InChI
                    };
                    comp.NumSpectra++;
                    comp.Spectra.Add(spectrum);
                    comp.MolecularWeight = FormulaUtility.GetMass(spectrum.Formula);
                    comp.Name = spectrum.Name;
                    comp.Formula = spectrum.Formula;
                    dic.Add(Inchikey, comp);
                    InchiKeys.Add(Inchikey);
                    counter++;
                }
            }


            this.CompoundList = new List<CompoundBean>(dic.Values);


            /*                 
                    string str2 = "";
                    if (minRt < 100f && maxRt > -1f && minRt < maxRt && (maxRt - minRt) > minRtDiff)
                                    {
                                        str2 = "ID: " + compound.Id + ", Name: " + compound.Name + ", first RT: " + rts[0] + ", minRT: " + minRt + ", maxRT: " + maxRt + ", diff: " + (maxRt - minRt);
                                        missRts.Add(str2);
                                    }
                      */
        }
        /*
                   compList = new ObservableCollection<CompoundInformationBean>(dic.Values);
        var missFormula = new List<string>();
        var missInChIKeys = new List<string>();
        var missRts = new List<string>();
                    float minRtDiff = 1.0f;
       foreach (var compound in compList)
        {
            var formulas = compound.DataList.Select(x => x.Formula).Distinct().ToList();
            if (formulas.Count > 1)
            {
                var str = "ID: " + compound.Id + ", Name: " + compound.Name + ", Formulas: ";
                foreach (var f in formulas) { str = str + ", " + f; }
                missFormula.Add(str);
            }

            var InChIKey = compound.DataList.Select(x => x.InchiKey).Distinct().ToList();
            if (InChIKey.Count > 1)
            {
                var counter2 = 0;
                foreach (var i in InChIKey)
                {
                    if (!i.Contains("-UHFFFAOYSA -")) counter2++;
                }
                if (counter2 > 1)
                {
                    var str = "ID: " + compound.Id + ", Name: " + compound.Name + ", InChIKeys: ";
                    foreach (var s in InChIKey) { str = str + ", " + s; }
                    missInChIKeys.Add(str);
                }
            }
            var rtOriginal = compound.DataList.Select(x => x.RetentionTime);
            var rts = new List<float>();
            foreach (var item in rtOriginal)
            {
                rts.Add((float)Math.Round(item, 2));
            }
            rts = rts.Distinct().ToList();
            string str2 = "";
            var maxRt = -1f; var minRt = 100f;
            for (var i = 0; i < rts.Count; i++)
            {
                if (rts[i] < 0) continue;
                if (maxRt < rts[i])
                {
                    maxRt = rts[i];
                }
                if (rts[i] < minRt)
                {
                    minRt = rts[i];
                }
            }
            if (minRt < 100f && maxRt > -1f && minRt < maxRt && (maxRt - minRt) > minRtDiff)
            {
                str2 = "ID: " + compound.Id + ", Name: " + compound.Name + ", first RT: " + rts[0] + ", minRT: " + minRt + ", maxRT: " + maxRt + ", diff: " + (maxRt - minRt);
                missRts.Add(str2);
            }
        }
        if (missRts.Count > 0)
        {
            var str = missRts.Count + " InChiKeys have different RTs (> 1min).\n";
            foreach (var f in missRts)
            {
                str = str + f + "\n";
            }
            var w = new ShortMessageWindow();
            w.Label_MessageTitle.Text = str;
            w.Title = "Different RT lists";
            w.Width = 600;
            w.Height = 100 + (missRts.Count * 16);
            w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            w.Show();
        }
        if (missFormula.Count > 0)
        {
            var str = missFormula.Count + " InChiKeys have different formula.\n";
            foreach (var f in missFormula)
            {
                str = str + f + "\n";
            }
            var w = new ShortMessageWindow();
            w.Label_MessageTitle.Text = str;
            w.Title = "Different formula lists";
            w.Width = 600;
            w.Height = 100 + (missFormula.Count * 16);
            w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            w.Show();
        }

        if (missInChIKeys.Count > 0)
        {
            var str = missInChIKeys.Count + " InChiKeys have different stereo information.\n";
            foreach (var f in missInChIKeys)
            {
                str = str + f + "\n";
            }
            var w = new ShortMessageWindow();
            w.Label_MessageTitle.Text = str;
            w.Title = "Different InChIKey lists";
            w.Width = 600;
            w.Height = 100 + (missInChIKeys.Count * 16);
            w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            w.Show();
        }
        return compList;
*/
    
        
    }
}
