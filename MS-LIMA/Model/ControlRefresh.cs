using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using Metabolomics.MsLima;
using Metabolomics.MsLima.Properties;
using Metabolomics.Core;
using Metabolomics.Core.Utility;
using ChartDrawing;

namespace Metabolomics.MsLima.Model
{
    public class ControlRefresh
    {
        private readonly MainWindowVM mainWindowVM;

        public ControlRefresh (MainWindowVM vm) { mainWindowVM = vm; }

        #region SelectionChanged
        public void SelectedPeakChanged(TabMassSpectraView tab)
        {
            if (this.mainWindowVM.SelectedPeak == null) return;
            if (tab == TabMassSpectraView.SingleMS)
            {
                mainWindowVM.SingleMassSpectrumUI.UpdateSelectedPeak(mainWindowVM.SelectedPeak.Mz);
            }
            else if(tab == TabMassSpectraView.MultipleMS)
            {
                if (mainWindowVM.MultipleSpectra == null) return;
                foreach(var g in mainWindowVM.MultipleSpectra.Children)
                {
                    var grid = (Grid)g;
                    var ui = (MassSpectrumUI)grid.Children[0];
                    ui.UpdateSelectedPeak(mainWindowVM.SelectedPeak.Mz);
                }

            }
            else if(tab == TabMassSpectraView.ConsensusMS)
            {

            }

        }

        public void SelectedConsensusPeakChanged(TabMassSpectraView tab)
        {
            if (mainWindowVM.SelectedMsGroup == null) return;
            if (tab == TabMassSpectraView.SingleMS)
            {
                mainWindowVM.SingleMassSpectrumUI.UpdateSelectedPeak(mainWindowVM.SelectedMsGroup.MedianMz);
            }
            else if (tab == TabMassSpectraView.MultipleMS)
            {
                if (mainWindowVM.MultipleSpectra == null) return;
                foreach (var g in mainWindowVM.MultipleSpectra.Children)
                {
                    var grid = (Grid)g;
                    var ui = (MassSpectrumUI)grid.Children[0];
                    ui.UpdateSelectedPeak(mainWindowVM.SelectedMsGroup.MedianMz);
                }

            }
            else if (tab == TabMassSpectraView.ConsensusMS)
            {

            }


        }

        #endregion

        #region Single MS spectrum
        public void SingleSpectrumViewRefresh()
        {
        }

        #endregion


        #region Multiple MS spectra
        public StackPanel MultipleSpectraRefresh()
        {
            if (mainWindowVM.SelectedCompoundBean == null) return null;
            double minX = 1000.0; double maxX = 0;
            double tmpMin, tmpMax;
            foreach(var spectrum in mainWindowVM.SelectedCompoundBean.Spectra)
            {
                tmpMax = spectrum.Spectrum.Max(x => x.Mz);
                tmpMin = spectrum.Spectrum.Min(x => x.Mz);
                if (tmpMax > maxX) maxX = tmpMax;
                if (tmpMin < minX) minX = tmpMin;
            }

            var stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
            MassSpectrumUI msUI;
            foreach (var spectrum in mainWindowVM.SelectedCompoundBean.Spectra)
            {
                var vm = mainWindowVM.MsHandler.GetMassSpectrumDrawVisual(spectrum);
                vm.MaxX = (float)(maxX + (maxX - minX) * 0.1);
                vm.MinX = (float)(minX - (maxX - minX) * 0.1);
                if (vm.MinX < 0) vm.MinX = 0;
                vm.PropertyChanged -= MultipleMassSpectrogramVIew_PropertyChanged;
                vm.PropertyChanged += MultipleMassSpectrogramVIew_PropertyChanged;
                msUI = new MassSpectrumUI(vm);
                var spectrumGrid = new Grid() { Height = mainWindowVM.MsLimaData.Parameter.GraphHeightInMultipleView, HorizontalAlignment = HorizontalAlignment.Stretch };
                spectrumGrid.Children.Add(msUI);
                stackPanel.Children.Add(spectrumGrid);
            }
            return stackPanel;
        }
        public void ScrollViewRefresh(float? max, float? min)
        {
            var i = 0;
            foreach (var spectrum in mainWindowVM.SelectedCompoundBean.Spectra)
            {
                var grid = (Grid)mainWindowVM.MultipleSpectra.Children[i];
                var ui = (MassSpectrumUI)grid.Children[0];
                ui.UpdateMinMax(min, max);
                i++;
            }
        }

        private void MultipleMassSpectrogramVIew_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MaxX" || e.PropertyName == "MinX")
            {
                var max = ((DrawVisualMassSpectrum)sender).MaxX;
                var min = ((DrawVisualMassSpectrum)sender).MinX;
                ScrollViewRefresh(max, min);
            }
        }
        #endregion

    }
}
