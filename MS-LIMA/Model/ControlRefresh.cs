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
        private MainWindowVM mainWindowVM;

        public ControlRefresh (MainWindowVM vm) { mainWindowVM = vm; }
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
                if (tmpMin > minX) minX = tmpMin;
            }
            return ScrollViewRefresh((float)maxX, (float)minX);
        }
        public StackPanel ScrollViewRefresh(float? max, float? min)
        {
            var stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
            MassSpectrumUI msUI;
            foreach (var spectrum in mainWindowVM.SelectedCompoundBean.Spectra)
            {
                var vm = MassSpectrumViewHandler.GetMassSpectrumDrawVisual(spectrum);
                //vm.SeriesList.MaxX = max ?? vm.SeriesList.MaxX;
                //vm.SeriesList.MinX = min ?? vm.SeriesList.MinX;
               // vm.PropertyChanged -= MultipleMassSpectrogramVIew_PropertyChanged;
               // vm.PropertyChanged += MultipleMassSpectrogramVIew_PropertyChanged;
                msUI = new MassSpectrumUI(vm);
                var spectrumGrid = new Grid() { Height = mainWindowVM.MsLimaData.Parameter.GraphHeightInMultipleView, HorizontalAlignment = HorizontalAlignment.Stretch };
                spectrumGrid.Children.Add(msUI);
                stackPanel.Children.Add(spectrumGrid);
            }
            return stackPanel;
        }
        private void MultipleMassSpectrogramVIew_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tmpGrid = new Grid();
            var tmpUI = new MassSpectrumUI();
            if (e.PropertyName == "MaxX" || e.PropertyName == "MinX")
            {
                var max = ((DrawVisualMassSpectrum)sender).MaxX;
                var min = ((DrawVisualMassSpectrum)sender).MinX;
                ScrollViewRefresh(max, min);
            }
        }
    }
}
