using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Metabolomics.Core;
using Metabolomics.MsLima.Model;
using Metabolomics.MsLima.Bean;
using System.Windows.Controls;
using ChartDrawing;

namespace Metabolomics.MsLima
{
    public class MainWindowVM : ViewModelBase
    {
        #region properties

        public MsLimaData MsLimaData { get; set; }
        public MassSpectrumViewHandler MsHandler { get; set; }
        public ControlRefresh ControlRefresh { get; set; }
        public AutoRepeater AutoExporter { get; set; }

        public List<CompoundBean> CompoundTable {
            get => MsLimaData.DataStorage.CompoundList;
        }
        public FilteredTable FilteredCompoundTable { get; set; }

        public FilterSettingsForLibrary FilteredTableSetting { get; set; }
        public TabMassSpectraView TabMassSpectraView { get; set; }
        public TabMassSpectrumTable TabMassSpectrumTable { get; set; } = TabMassSpectrumTable.SinglePeak;

        public ICollectionView FilteredComponudTableView {
            get => FilteredCompoundTable.View;
        }

        public List<MsGroup> ConsensusSpectraTable {
            get {
                if (SelectedCompoundBean == null) return null;
                return MsGrouping.Excute(SelectedCompoundBean);
            }
        }

        private bool ShouldUpdateSingleMassSpectrumVM = true;
        private bool ShouldUpdateMultipleSpectrumVM = true;
        private bool ShouldUpdateConsensusSpectrumVM = true;

        #region Selected Items
        private AnnotatedPeak selectedPeak;
        private MassSpectrum selectedSpectrum;
        private CompoundBean selectedCompoundBean;
        private MsGroup selectedMsGroup;
        public AnnotatedPeak SelectedPeak {
            get => selectedPeak;
            set => OnPropertyChangedIfSet(ref selectedPeak, value, nameof(SelectedPeak));
        }

        public MassSpectrum SelectedSpectrum {
            get => selectedSpectrum;
            set {
                selectedSpectrum = value;
                OnPropertyChanged(nameof(SelectedSpectrum));
                SelectedSpectrumChanged();
            }
        }

        public CompoundBean SelectedCompoundBean {
            get => selectedCompoundBean;
            set {
                OnPropertyChangedIfSet(ref selectedCompoundBean, value, nameof(SelectedCompoundBean));
                SelectedCompoundChanged();
            }
        }

        public MsGroup SelectedMsGroup {
            get => selectedMsGroup;
            set => OnPropertyChangedIfSet(ref selectedMsGroup, value, nameof(SelectedMsGroup));
        }
        #endregion

        #region Label
        private string mainWindowTitle = Properties.Resources.Version;
        public string MainWindowTitle {
            get => mainWindowTitle;
            set => OnPropertyChangedIfSet(ref mainWindowTitle, value, nameof(MainWindowTitle));
        }
        public string LabelNumCompounds {
            get {
                if (CompoundTable == null) return ""; return "Number of compounds: " + CompoundTable.Count;
            }
        }
        public string LabelNumSpectra {
            get {
                if (MsLimaData.DataStorage.RawLibraryFile == null) return "Please import a library"; return "Number of spectra: " + MsLimaData.DataStorage.RawLibraryFile.Count;
            }
        }
        public string LabelSelectedCompound {
            get {
                if (SelectedCompoundBean == null) return ""; return "Selected sompound: " + SelectedCompoundBean.Name;
            }
        }
        public string LabelSelectedSpectra {
            get {
                if (SelectedSpectrum == null) return ""; return "Spectra ID: " + SelectedSpectrum.Id + ", " + SelectedSpectrum.AdductIon.AdductIonName + ", " + SelectedSpectrum.CollisionEnergy + "eV";
            }
        }

        private System.Windows.Media.Imaging.BitmapImage structureImage;

        public System.Windows.Media.Imaging.BitmapImage StructureImage {
            get => structureImage;
            set => OnPropertyChangedIfSet(ref structureImage, value, nameof(StructureImage)); }
        #endregion

        #region ViewModel
        private DrawVisualMassSpectrum singleMassSpectrumVM = new DrawVisualMassSpectrum();
        public DrawVisualMassSpectrum SingleMassSpectrumVM {
            get => singleMassSpectrumVM;
            set {
                singleMassSpectrumVM = value;
                OnPropertyChanged(nameof(SingleMassSpectrumVM));
                SingleMassSpectrumUI.UpdateFE(SingleMassSpectrumVM);
            }
        }

        private DrawVisualMassSpectrum consensusSpectrumVM = new DrawVisualMassSpectrum();
        public DrawVisualMassSpectrum ConsensusSpectrumVM {
            get => consensusSpectrumVM;
            set {
                consensusSpectrumVM = value;
                OnPropertyChanged(nameof(ConsensusSpectrumVM));
                ConsensusSpectrumUI.UpdateFE(ConsensusSpectrumVM);
            }
        }
        #endregion

        #region UI
        private MassSpectrumUI singleMassSpectrumUI;
        public MassSpectrumUI SingleMassSpectrumUI {
            get => singleMassSpectrumUI;
            set => OnPropertyChangedIfSet(ref singleMassSpectrumUI, value, nameof(SingleMassSpectrumUI));
        }

        private MassSpectrumUI consensusSpectrumUI;
        public MassSpectrumUI ConsensusSpectrumUI {
            get => consensusSpectrumUI;
            set => OnPropertyChangedIfSet(ref consensusSpectrumUI, value, nameof(ConsensusSpectrumUI));
        }

        private StackPanel multipleSpectra;
        public StackPanel MultipleSpectra {
            get => multipleSpectra;
            set => OnPropertyChangedIfSet(ref multipleSpectra, value, nameof(MultipleSpectra));
        }

        #endregion

        #region Filter

        private string filterNameText;
        private string filterMzText;
        private string filterRtText;
        private string filterInChIKeyText;

        public string FilterNameText {
            get => filterNameText;
            set {
                OnPropertyChangedIfSet(ref filterNameText, value, nameof(FilterNameText));
                this.FilteredTableSetting.MetaboliteNameFilter = value;
            }
        }


        public string FilterMzText {
            get => filterMzText;
            set {
                OnPropertyChangedIfSet(ref filterMzText, value, nameof(FilterMzText));
                this.FilteredTableSetting.MzFilter = value;
            }
        }
        public string FilterRtText {
            get => filterRtText;
            set {
                OnPropertyChangedIfSet(ref filterRtText, value, nameof(FilterRtText));
                this.FilteredTableSetting.RetentionTimeFilter = value;
            }
        }

        public string FilterInChIKeyText {
            get => filterInChIKeyText;
            set {
                OnPropertyChangedIfSet(ref filterInChIKeyText, value, nameof(FilterInChIKeyText));
                this.FilteredTableSetting.InChIKeyFilter = value;
            }
        }
        #endregion


        #region Command

        #region Selection Changed Commands
        public DelegateCommand SelectionChangedSingleSpectrumTableCommand { get; set; }
        public DelegateCommand SelectionChangedConsensusTableCommand { get; set; }

        //public DelegateCommand SelectionChangedTabControlMsViewCommand { get; set; }
        public DelegateCommand SelectionChangedTabControlMsTableCommand { get; set; }


        #endregion
        #region Menu Commands
        public DelegateCommand ImportFileCommand { get; set; }
        public DelegateCommand ImportMassBankFileCommand { get; set; }

        public DelegateCommand SaveAsMspCommand { get; set; }
        public DelegateCommand SaveAsMspWithoutRTCommand { get; set; }
        public DelegateCommand SaveAsMzMineCommand { get; set; }

        public DelegateCommand WindowLoaded { get; set; }

        public DelegateCommand StartUpSettingWindow { get; set; }

        public DelegateCommand StartUpWindowAllSpectra { get; set; }

        public DelegateCommand StartUpWindowComparativeViewer { get; set; }

        public DelegateCommand ConvertAccurateMassToTheoreticalMass { get; set; }
        public DelegateCommand DropRetentionTime { get; set; }
        public DelegateCommand RemoveUnannotatedCommand { get; set; }
        public DelegateCommand SaveCommonProductIonCommand { get; set; }
        public DelegateCommand UpdateSmilesAndInChiBasedOnInChIKeyCommand { get; set; }
        public DelegateCommand UpdateCommonMetaDataCommand { get; set; }
        #endregion


        // Change comments to add particular sentence;
        private DelegateCommand temporaryMethod;
        public DelegateCommand TemporaryMethods {
            get {
                return temporaryMethod ?? new DelegateCommand(x =>
                {
                    if (this.CompoundTable == null) return;
                    foreach (var c in CompoundTable)
                    {
                        foreach (var spec in c.Spectra)
                        {
                            if (spec.Comment.Contains("CorrelDec"))
                            {
                                spec.Comment = "MS2 deconvoluted using CorrDec from all ion fragmentation data; MetaboLights identifier MTBLS1040";
                            }
                            else
                            {
                                spec.Comment = "MS2 deconvoluted using MS2Dec from all ion fragmentation data; MetaboLights identifier MTBLS1040; " + spec.Comment;
                            }
                        }
                    }
                });
            }
        }

        // Change MsLevel of CorrDec
        private DelegateCommand temporaryMethod2;
        public DelegateCommand TemporaryMethods2 {
            get {
                return temporaryMethod2 ?? new DelegateCommand(x =>
                {
                    if (this.CompoundTable == null) return;
                    foreach (var c in CompoundTable)
                    {
                        foreach (var spec in c.Spectra)
                        {
                            Console.WriteLine(spec.CollisionEnergy);
                            if (spec.CollisionEnergy == 0)
                            {
                                Console.WriteLine("working");
                                spec.MsLevel = "MS1";
                            }
                        }
                    }
                });
            }
        }

        // CorrDec vs MS2Dec similarity in DIA GIAR library
        private DelegateCommand temporaryMethod3;
        public DelegateCommand TemporaryMethods3 {
            get {
                return temporaryMethod3 ?? new DelegateCommand(x =>
                {
                    if (this.CompoundTable == null) return;
                    using (var sw = new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(MsLimaData.DataStorage.FilePath) + "\\ms2sim.tsv", false, Encoding.UTF8))
                    {
                        foreach (var c in CompoundTable)
                        {
                            c.Name = c.Name.Replace(',', '_');
                            if (c.NumSpectra == 3)
                            {
                                sw.WriteLine(c.InChIKey + "\t" + c.Name + "\t" + c.NumSpectra + "\t");
                                continue;
                            }

                            var dic = new Dictionary<float, MassSpectrum>();
                            foreach (var spec in c.Spectra)
                            {
                                if (dic.ContainsKey(spec.CollisionEnergy))
                                {
                                    var s1 = spec;
                                    var s2 = dic[spec.CollisionEnergy];
                                    if (spec.Comment.Contains("CorrDec"))
                                    {
                                        s1 = dic[spec.CollisionEnergy];
                                        s2 = spec;
                                    }

                                    var ms2tol = (float)MsLimaData.Parameter.MS2Tol;
                                    var dotProductFactor = 1.0;
                                    var reverseDotProdFactor = 1.0;
                                    var presensePercentageFactor = 1.0;

                                    var dotScore = (float)MsSimilarityScoring.GetMassSpectraSimilarity(s1, s2, ms2tol) * 100;
                                    var revScore = (float)MsSimilarityScoring.GetReverseSearchSimilarity(s1, s2, ms2tol) * 100;
                                    var matchScore = (float)MsSimilarityScoring.GetPresenceSimilarityBasedOnReference(s1, s2, ms2tol) * 100;
                                    var totalScore = (float)((dotProductFactor * dotScore + reverseDotProdFactor * revScore + presensePercentageFactor * matchScore) / (dotProductFactor + reverseDotProdFactor + presensePercentageFactor));

                                    var revScore2 = (float)MsSimilarityScoring.GetReverseSearchSimilarity(s2, s1, ms2tol) * 100;
                                    var matchScore2 = (float)MsSimilarityScoring.GetPresenceSimilarityBasedOnReference(s2, s1, ms2tol) * 100;
                                    var totalScore2 = (float)((dotProductFactor * dotScore + reverseDotProdFactor * revScore2 + presensePercentageFactor * matchScore2) / (dotProductFactor + reverseDotProdFactor + presensePercentageFactor));

                                    sw.Write(c.InChIKey + "\t" + c.Name + "\t" + c.NumSpectra + "\t");
                                    sw.WriteLine(spec.CollisionEnergy + "\t" + dotScore + "\t" + revScore + "\t" + matchScore + "\t" + totalScore + "\t" + revScore2 + "\t" + matchScore2 + "\t" + totalScore2);
                                }
                                else
                                {
                                    dic.Add(spec.CollisionEnergy, spec);
                                }
                            }

                        }
                    }
                });
            }
        }

        // CorrDecやMS2Dec(DIA; GIAR) vs. DDA (KI)を比較したときの結果をtxt, pngで保存。
        public DelegateCommand TemporaryMethods4 {
            get {
                return new DelegateCommand(x => {
                    if (this.CompoundTable == null) return;
                    using (var sw = new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(MsLimaData.DataStorage.FilePath) + "\\ms2sim.tsv", false, Encoding.UTF8))
                    {
                        foreach (var c in CompoundTable)
                        {
                            c.Name = c.Name.Replace(',', '_');
                            if (c.NumSpectra != 10)
                            {
                                continue;
                            }

                            var dic1 = new Dictionary<float, MassSpectrum>();
                            var dic2 = new Dictionary<float, MassSpectrum>();
                            foreach (var spec in c.Spectra)
                            {
                                if (dic1.ContainsKey(spec.CollisionEnergy))
                                {
                                    if (dic2.ContainsKey(spec.CollisionEnergy))
                                    {
                                        var s1 = spec;
                                        var s2 = dic1[spec.CollisionEnergy];
                                        var s3 = dic2[spec.CollisionEnergy];

                                        var ms2tol = (float)MsLimaData.Parameter.MS2Tol;

                                        var dotScore1 = (float)MsSimilarityScoring.GetMassSpectraSimilarity(s2, s1, ms2tol) * 100;
                                        var dotScore2 = (float)MsSimilarityScoring.GetMassSpectraSimilarity(s3, s1, ms2tol) * 100;

                                        sw.Write(c.InChIKey + "\t" + c.Name + "\t" + c.NumSpectra + "\t");
                                        sw.WriteLine(spec.CollisionEnergy + "\t" + dotScore1 + "\t" + dotScore2);

                                        var s1rel = MassSpectrumUtility.ConvertToRelativeIntensity(s1);
                                        var s2rel = MassSpectrumUtility.ConvertToRelativeIntensity(s2);
                                        var s3rel = MassSpectrumUtility.ConvertToRelativeIntensity(s3);
                                        var f1 = System.IO.Path.GetDirectoryName(MsLimaData.DataStorage.FilePath) + "\\" + c.Id +"_"+ "CorrDec_" + Math.Round(dotScore1, 1) + "_" + spec.CollisionEnergy + "_" + c.Name;
                                        var f2 = System.IO.Path.GetDirectoryName(MsLimaData.DataStorage.FilePath) + "\\" + c.Id +"_"+ "MS2Dec_" + Math.Round(dotScore2, 1) + "_" + spec.CollisionEnergy + "_" + c.Name;
                                        var dv1 = MsHandler.GetMassSpectrumWithRefDrawVisual(s2rel, s1rel);
                                        var dv2 = MsHandler.GetMassSpectrumWithRefDrawVisual(s3rel, s1rel);
                                        dv1.Title.Label = "CorrDec; " + s1.CollisionEnergy + "eV; " + Math.Round((dotScore1), 1) + "%; " + s2.Name;
                                        dv2.Title.Label = "MS2Dec; " + s2.CollisionEnergy + "eV; " + Math.Round((dotScore2), 1) + "%; " + s3.Name;
                                        Exporter.ExportDrawVisual.SaveAsPng(f1 + ".png", dv1, dv1.MinX, dv1.MaxX, dv1.MinY, dv1.MaxY, 350, 300, 300, 300, true);
                                        Exporter.ExportDrawVisual.SaveAsPng(f2 + ".png", dv2, dv2.MinX, dv2.MaxX, dv2.MinY, dv2.MaxY, 350, 300, 300, 300, true);
                                    }
                                    else
                                    {
                                        dic2.Add(spec.CollisionEnergy, spec);
                                    }
                                }
                                else
                                {
                                    dic1.Add(spec.CollisionEnergy, spec);
                                }
                            }

                        }
                    }

                });
            }
        }

        // 2つのMSPファイルを開き、後に開いた方のprecursor mzを、先に開いた方のコメント欄に追記する。
        // なお、その時にCorrDecとコメントに入っている場合は何もしない。
        public DelegateCommand TemporaryMethods5 {
            get {
                return new DelegateCommand(x =>
                {
                    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog
                    {
                        Filter = "MSP file(*.msp)|*.msp| MGF file (*.mgf)|*.mgf| Text file (*.txt)|*.txt| all files(*)|*;",
                        Title = "Import a library file",
                        RestoreDirectory = true,
                        Multiselect = false
                    };

                    if (ofd.ShowDialog() == true)
                    {
                        var ms1 = Reader.ReadMspFile.ReadAsMsSpectra(ofd.FileName);

                        Microsoft.Win32.OpenFileDialog ofd2 = new Microsoft.Win32.OpenFileDialog
                        {
                            Filter = "MSP file(*.msp)|*.msp| MGF file (*.mgf)|*.mgf| Text file (*.txt)|*.txt| all files(*)|*;",
                            Title = "Import a library file",
                            RestoreDirectory = true,
                            Multiselect = false
                        };

                        if (ofd2.ShowDialog() == true)
                        {
                            var ms2 = Reader.ReadMspFile.ReadAsMsSpectra(ofd2.FileName);


                            for (var i = 0; i < ms1.Count; i++)
                            {
                                if (ms1[i].Comment.Contains("CorrDec")) continue;
                                var s = ms2[i].PrecursorMz;
                                ms1[i].Comment = "Experimental precursorMz " + Math.Round(s,5) + "; " + ms1[i].Comment;
                            }

                        }
                        using (var sw = new System.IO.StreamWriter(ofd.FileName + "mod", false, Encoding.UTF8))
                        {
                            Writer.MassSpectrumWriter.WriteMassSpectraAsMsp(sw, ms1);
                        }
                    }
                });
            }
        }

        // CorrDec revision用
        // Tyrosineの低段階希釈サンプルをCorrDecで全通り計算したので、それらを比較するためのメソッド。
        // 対象とするライブラリを変更することも可。2つめに開いたライブラリが対象ライブラリ。
        public DelegateCommand TemporaryMethods6 {
            get {
                return new DelegateCommand(x =>
                {
                    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog
                    {
                        Filter = "MSP file(*.msp)|*.msp| MGF file (*.mgf)|*.mgf| Text file (*.txt)|*.txt| all files(*)|*;",
                        Title = "Import a library file",
                        RestoreDirectory = true,
                        Multiselect = false
                    };

                    if (ofd.ShowDialog() == true)
                    {
                        var ms1 = Reader.ReadMspFile.ReadAsMsSpectra(ofd.FileName);

                        Microsoft.Win32.OpenFileDialog ofd2 = new Microsoft.Win32.OpenFileDialog
                        {
                            Filter = "MSP file(*.msp)|*.msp| MGF file (*.mgf)|*.mgf| Text file (*.txt)|*.txt| all files(*)|*;",
                            Title = "Import a library file",
                            RestoreDirectory = true,
                            Multiselect = false
                        };

                        if (ofd2.ShowDialog() == true)
                        {
                            using (var sw = new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(MsLimaData.DataStorage.FilePath) + "\\ms2similarity_tyrosine_serires.tsv", false, Encoding.UTF8))
                            {
                                var resMatrix = new double[100,100];
                                // ms2には1つだけmass spectrum入れておくこと。
                                var ms2 = Reader.ReadMspFile.ReadAsMsSpectra(ofd2.FileName);
                                var s2rel = MassSpectrumUtility.ConvertToRelativeIntensity(ms2[0]);
                                var s2relv2 = new MassSpectrum() { Spectrum = new List<AnnotatedPeak>() };
                                foreach (var spec in s2rel.Spectrum)
                                {
                                    if (spec.Intensity < 1) continue;
                                    s2relv2.Spectrum.Add(spec);
                                }

                                for (var i = 0; i < ms1.Count; i++)
                                {
                                    var start = 0;
                                    var end = 0;
                                    foreach (var meta in ms1[i].OtherMetaData)
                                    {
                                        Console.WriteLine(meta);
                                        if (meta.Contains("Start: "))
                                        {
                                            var tmps = meta.Split(':')[1].Trim();
                                            start = int.Parse(tmps);
                                            Console.WriteLine(start);
                                        }
                                        else if (meta.Contains("End: "))
                                        {
                                            var tmps = meta.Split(':')[1].Trim();
                                            end = int.Parse(tmps);
                                        }
                                    }

                                    if (ms1[i].Spectrum.Count == 0)
                                    {
                                        resMatrix[start, end] = 0.0;
                                        continue;
                                    }

                                    var s1rel = MassSpectrumUtility.ConvertToRelativeIntensity(ms1[i]);
                                    var s1relv2 = new MassSpectrum() { Spectrum = new List<AnnotatedPeak>() };
                                    foreach (var spec in s1rel.Spectrum)
                                    {
                                        if (spec.Intensity < 1) continue;
                                        s1relv2.Spectrum.Add(spec);
                                    }
                                    
                                    s1relv2.Name = s1rel.Name;
                                    resMatrix[start,end] = MsSimilarityScoring.GetMassSpectraSimilarity(s1relv2, s2relv2, 0.01f) * 100;
                                    
                                }
                                // header
                                sw.Write("i/j" + "\t");
                                for(var i = 0; i < 100; i++)
                                {
                                    sw.Write(i + "\t");
                                }
                                sw.WriteLine("");

                                for(var i = 0; i < 100; i++)
                                {
                                    // rownames
                                    sw.Write(i + "\t");
                                    for(var j = 0; j < 100; j++)
                                    {
                                        sw.Write(resMatrix[i, j] + "\t");
                                    }
                                    sw.WriteLine("");
                                }
                            }
                        }
                    }
                });
            }
        }

        private DelegateCommand saveChart;

        public DelegateCommand SaveChart{
            get {
                return saveChart ?? new DelegateCommand(x => {
                    var w = new SaveChartDrawing(((DrawVisualMassSpectrum)x));
                    w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    w.Show();
                });
            }
       }

        #endregion
        #endregion

        public MainWindowVM()
        {
            Task.Run(() => SmilesUtility.TryClassLoad());
            MsLimaData = new MsLimaData();
            MsHandler = new MassSpectrumViewHandler(MsLimaData.Parameter);
            AutoExporter = new AutoRepeater(MsLimaData.Parameter.WinParam.AutoExportIntervalMillisecond);
            AutoExporter.OnTimeEventHandler += (o, e) => { AutoExportFunction(); };
            SingleMassSpectrumUI = new MassSpectrumUI(SingleMassSpectrumVM);
            ConsensusSpectrumUI = new MassSpectrumUI(ConsensusSpectrumVM);

            ControlRefresh = new ControlRefresh(this);
            SetCommands();
        }

        private void MainWindowLoad()
        {
            //WindowUtility.StartUpInitializingWindow();
        }


        private void SetCommands()
        {
            WindowLoaded = new DelegateCommand(x => MainWindowLoad());

            #region MenuItems
            ImportFileCommand = new DelegateCommand(
                x =>
                {
                    ImportUtility.ImportFile(MsLimaData, AutoExporter);
                    ImportFile();
                }
            );

            ImportMassBankFileCommand = new DelegateCommand(
                x =>
                {
                    ImportUtility.ImportMassBankFile(MsLimaData, AutoExporter);
                    ImportFile();
                }
            );

            SaveAsMspCommand = new DelegateCommand(
                x => ExportUtility.SaveAsMsp(CompoundTable),
                x => !IsDataLoaded());

            SaveAsMspWithoutRTCommand = new DelegateCommand(
                x => ExportUtility.SaveAsMspWithoutRT(CompoundTable),
                x => !IsDataLoaded());

            SaveAsMzMineCommand = new DelegateCommand(
                x => ExportUtility.SaveCompoundTableAsMzMineFormat(CompoundTable),
                x => !IsDataLoaded());

            ConvertAccurateMassToTheoreticalMass = new DelegateCommand(
                x => CompoundGroupUtility.ConvertActualMassToTheoreticalMass(CompoundTable),
                x => !IsDataLoaded());

            DropRetentionTime = new DelegateCommand(
                x => CompoundGroupUtility.DropRetentionTime(CompoundTable),
                x => !IsDataLoaded());

            RemoveUnannotatedCommand = new DelegateCommand(
                x => CompoundGroupUtility.RemoveUnannotatedPeaks(CompoundTable),
                x => !IsDataLoaded());

            SaveCommonProductIonCommand = new DelegateCommand(
                x => ExportUtility.SaveCommonProductIonTable(CompoundTable),
                x => !IsDataLoaded());

            UpdateSmilesAndInChiBasedOnInChIKeyCommand = new DelegateCommand(
                x => WindowUtility.UpdateMetaData(CompoundTable),
                x => !IsDataLoaded());

            UpdateCommonMetaDataCommand = new DelegateCommand(
              x => WindowUtility.UpdateCommonMetaData(CompoundTable),
              x => !IsDataLoaded());

            #endregion


            #region SelectionChanged
            SelectionChangedSingleSpectrumTableCommand = new DelegateCommand(
                x => SingleSpectrumTableSelectionChanged()
                );

            SelectionChangedConsensusTableCommand = new DelegateCommand(
                x => ConsensusTableSelectionChanged()
                );
            SelectionChangedTabControlMsTableCommand = new DelegateCommand(
                x => SelectionChangedTabConrtrolMsView()
                );
            #endregion


            #region launch window
            StartUpSettingWindow = new DelegateCommand(
                x => WindowUtility.StartUpParameterSettingWindow(MsLimaData));

            StartUpWindowAllSpectra = new DelegateCommand(
                x => WindowUtility.StartUpAllSpectraTableWindow(MsLimaData),
                x => !IsDataLoaded());
            StartUpWindowComparativeViewer = new DelegateCommand(
                x => WindowUtility.StartUpComparativeSpectraViewer(MsLimaData),
                x => !IsDataLoaded());
            #endregion
        }


        #region Methods for SelectionChanged Command

        public void SingleSpectrumTableSelectionChanged()
        {
            ControlRefresh.SelectedPeakChanged(TabMassSpectraView);
        }

        public void ConsensusTableSelectionChanged()
        {
            ControlRefresh.SelectedConsensusPeakChanged(TabMassSpectraView);
        }

        public void SelectionChangedTabConrtrolMsView()
        {
            SingleMassSpectrumRefresh();
            MsSpectraViewRefresh();
        }

        public void SelectedCompoundChanged()
        {

            OnPropertyChanged(nameof(LabelSelectedCompound));
            OnPropertyChanged(nameof(ConsensusSpectraTable));
            ShouldUpdateMultipleSpectrumVM = true;
            ShouldUpdateConsensusSpectrumVM = true;
            SelectedSpectrum = selectedCompoundBean.Spectra[0];
            if (!string.IsNullOrEmpty(SelectedCompoundBean.Smiles))
            {
                StructureImage = SmilesUtility.SmilesToMediaImageSource(SelectedCompoundBean.Smiles, 200, 200);
            }
            MsSpectraViewRefresh();
        }

        public void SelectedSpectrumChanged()
        {
            OnPropertyChanged(nameof(LabelSelectedSpectra));
            ShouldUpdateSingleMassSpectrumVM = true;
            SingleMassSpectrumRefresh();
        }

        public void SingleMassSpectrumRefresh()
        {
            if (TabMassSpectraView == TabMassSpectraView.SingleMS && ShouldUpdateSingleMassSpectrumVM)
            {
                SingleMassSpectrumVM = MsHandler.GetMassSpectrumDrawVisual(SelectedSpectrum);
                ShouldUpdateSingleMassSpectrumVM = false;
            }
        }

        public void MsSpectraViewRefresh()
        {
            if (TabMassSpectraView == TabMassSpectraView.MultipleMS && ShouldUpdateMultipleSpectrumVM)
            {
                MultipleSpectra = ControlRefresh.MultipleSpectraRefresh();
                ShouldUpdateMultipleSpectrumVM = false;
            }
            else if (TabMassSpectraView == TabMassSpectraView.ConsensusMS && ShouldUpdateConsensusSpectrumVM)
            {
                ConsensusSpectrumVM = MsHandler.GetMassSpectrumDrawVisualFromConsensus(ConsensusSpectraTable);
                ShouldUpdateConsensusSpectrumVM = false;
            }
        }
        #endregion


        #region Methods for MenuItem Commands

        public void ImportFile()
        {
            if (MsLimaData.DataStorage.CompoundList == null || MsLimaData.DataStorage.CompoundList.Count == 0) return;
            FilteredCompoundTable = new FilteredTable(this.CompoundTable);
            FilteredTableSetting = new FilterSettingsForLibrary(this.FilteredCompoundTable.View);
            FilteredCompoundTable.View.Filter = this.FilteredTableSetting.CompoundFilter;
            MainWindowTitle = Properties.Resources.Version + " File: " + MsLimaData.DataStorage.FilePath;
            SelectedCompoundBean = CompoundTable[0];
            OnPropertyChangedAfterFileImported();
        }

        #endregion

        #region Utilities
        private void OnPropertyChangedAfterFileImported()
        {
            StartUpWindowComparativeViewer.RaiseCanExecuteChanged();
            StartUpWindowAllSpectra.RaiseCanExecuteChanged();
            SaveAsMspCommand.RaiseCanExecuteChanged();
            SaveAsMspWithoutRTCommand.RaiseCanExecuteChanged();
            SaveAsMzMineCommand.RaiseCanExecuteChanged();
            UpdateSmilesAndInChiBasedOnInChIKeyCommand.RaiseCanExecuteChanged();
            ConvertAccurateMassToTheoreticalMass.RaiseCanExecuteChanged();
            DropRetentionTime.RaiseCanExecuteChanged();
            RemoveUnannotatedCommand.RaiseCanExecuteChanged();
            SaveCommonProductIonCommand.RaiseCanExecuteChanged();
            UpdateCommonMetaDataCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(FilteredComponudTableView));
            OnPropertyChanged(nameof(LabelNumCompounds));
            OnPropertyChanged(nameof(LabelNumSpectra));
        }

        private bool IsDataLoaded()
        {
            return (CompoundTable == null || CompoundTable.Count == 0);
        }

        public void AutoExportFunction()
        {
            Task.Run(() => ExportCompoundTable.ExportCompoundTableAsMsp(MsLimaData.DataStorage.FilePath, MsLimaData.DataStorage.CompoundList));
        }


        #endregion
    }
}
