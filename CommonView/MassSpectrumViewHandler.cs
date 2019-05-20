using System;
using System.Collections.Generic;
using System.Linq;
using ChartDrawing;
using System.Windows.Media;
using Metabolomics.Core;
using Metabolomics.MsLima.Bean;
namespace Metabolomics.MsLima
{
    public class MassSpectrumViewHandler
    {
        //number of decimal places
        public ParameterBean Param { get; set; }
        public int NumDecimalPlaces { get => Param.NumberOfDecimalPlaces; }
        public MassSpectrumViewHandler() { }
        public MassSpectrumViewHandler(ParameterBean param) {
            Param = param;
        }

        #region Single mass spectrum
        public DrawVisualMassSpectrum GetMassSpectrumDrawVisual(MassSpectrum spectrum)
        {
            var height = 200;
            var width = 500;
            var area = new Area()
            {
                Height = height,
                Width = width,
                LabelSpace = new LabelSpace() { Top = 25, Bottom = 0 },
                Margin = new Margin(60, 30, 20, 40)
            };
            area.AxisX.IsItalicLabel = true;
            //area.AxisX.Pen = new System.Windows.Media.Pen(Brushes.Black, 1);
            //area.AxisY.Pen = new System.Windows.Media.Pen(Brushes.Black, 1);
            area.AxisX.FontSize = 13;
            area.AxisY.FontSize = 13;
            area.AxisX.AxisLabel = "m/z";
            area.AxisY.AxisLabel = "Intensity";
            area.AxisY.MinorScaleEnabled = false;
            area.AxisX.MinorScaleEnabled = false;
            var title = Utility.GetDefaultTitleV1(15, "ID: " + spectrum.Id + ", " + spectrum.Name);
            var slist = GetMassSpectrumSeriesList(spectrum);
            var dv = new DrawVisualMassSpectrum(area, title, slist, null, Param.MS2Tol, NumDecimalPlaces);
            dv.SeriesList.MinY = 0;
            if (slist.Series[0].Points.Count > 1)
            {
                Utility.SetDrawingMaxXRatio(dv, 0.1f);
                Utility.SetDrawingMinXRatio(dv, 0.1f);
            }
            else
            {
                dv.SeriesList.MaxX += 10;
                dv.SeriesList.MinX -= 10;
            }
            dv.Initialize();
            return dv;
        }

        public SeriesList GetMassSpectrumSeriesList(MassSpectrum spectrum)
        {
            var slist = new SeriesList();
            var s = new Series() { ChartType = ChartType.MS, MarkerType = MarkerType.None, Pen = new System.Windows.Media.Pen(Brushes.Blue, 1), FontType = new Typeface("Arial") };
            var maxInt = spectrum.Spectrum.Max(x => x.Intensity);
            foreach (var peak in spectrum.Spectrum)
            {
                var peakAnnotation = GetMsPeakAnnotation(peak, maxInt);
                s.AddPoint((float)peak.Mz, (float)peak.Intensity, 
                    Math.Round(peak.Mz, NumDecimalPlaces).ToString(),
                    new Accessory() { PeakAnnotation = peakAnnotation }
                    );

            }
            s.IsLabelVisible = true;
            slist.Series.Add(s);
            return slist;
        }
        #endregion

        #region Consensus Mass Spectrum
        public DrawVisualMassSpectrum GetMassSpectrumDrawVisualFromConsensus(List<MsGroup> spectrum)
        {
            var height = 200;
            var width = 500;
            var area = new Area()
            {
                Height = height,
                Width = width,
                LabelSpace = new LabelSpace() { Top = 25, Bottom = 0 },
                Margin = new Margin(60, 0, 20, 40)
            };
            area.AxisX.IsItalicLabel = true;
            //area.AxisX.Pen = new System.Windows.Media.Pen(Brushes.Black, 1);
            //area.AxisY.Pen = new System.Windows.Media.Pen(Brushes.Black, 1);
            area.AxisX.FontSize = 13;
            area.AxisY.FontSize = 13;
            area.AxisX.AxisLabel = "m/z";
            area.AxisY.AxisLabel = "Intensity";
            area.AxisY.MinorScaleEnabled = false;
            area.AxisX.MinorScaleEnabled = false;
            var title = Utility.GetDefaultTitleV1();
            var slist = GetMassSpectrumSeriesList(spectrum);
            var dv = new DrawVisualMassSpectrum(area, title, slist);
            dv.SeriesList.MinY = 0;
            if (slist.Series[0].Points.Count > 1)
            {
                Utility.SetDrawingMaxXRatio(dv, 0.1f);
                Utility.SetDrawingMinXRatio(dv, 0.1f);
            }
            else
            {
                dv.SeriesList.MaxX += 10;
                dv.SeriesList.MinX -= 10;
            }
            dv.Initialize();
            return dv;
        }
        public SeriesList GetMassSpectrumSeriesList(List<MsGroup> spectrum)
        {
            var slist = new SeriesList();
            var s = new Series() { ChartType = ChartType.MS, MarkerType = MarkerType.None, Pen = new System.Windows.Media.Pen(Brushes.Black, 1), FontType = new Typeface("Arial") };
            foreach (var peak in spectrum)
            {
                s.AddPoint((float)peak.MedianMz, (float)peak.MedianIntensity, Math.Round(peak.MedianMz, NumDecimalPlaces).ToString());

            }
            s.IsLabelVisible = true;
            slist.Series.Add(s);
            return slist;
        }
        #endregion

        #region Utilities
        // Conversion from AnnotatedPeaks to Accessory.MsPeakAnnotation for 
        public static Accessory.MsPeakAnnotation GetMsPeakAnnotation(AnnotatedPeak peak, double maxInt)
        {
            return new Accessory.MsPeakAnnotation()
            {
                RelInt = (peak.Intensity / maxInt) * 100,
                IsMsGroup = false,
                Formula = peak.Formula,
                Smiles = peak.Smiles
            };
        }

        #endregion
    }
}
