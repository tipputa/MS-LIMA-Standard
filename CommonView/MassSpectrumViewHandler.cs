using System;
using System.Collections.Generic;
using System.Linq;
using ChartDrawing;
using System.Windows.Media;
using Metabolomics.Core;
using Metabolomics.MsLima.Bean;
namespace Metabolomics.MsLima
{
    public static class MassSpectrumViewHandler
    {
        public static DrawVisualMassSpectrum GetMassSpectrumDrawVisual(MassSpectrum spectrum)
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
            var slist = GetMassSpectrumSeriesList(ConvertMassSpectrum2List(spectrum)); 
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

        public static List<float[]> ConvertMassSpectrum2List(MassSpectrum spectrum)
        {
            var newList = new List<float[]>();
            foreach(var s in spectrum.Spectrum)
            {
                newList.Add(new float[2] { (float)s.Mz, (float)s.Intensity });
            }
            return newList;
        }

        public static List<float[]> ConvertMsgroup2List(List<MsGroup> spectrum)
        {
            var newList = new List<float[]>();
            foreach (var s in spectrum)
            {
                newList.Add(new float[2] { (float)s.MedianMz, (float)s.MedianIntensity });
            }
            return newList;

        }


        public static SeriesList GetMassSpectrumSeriesList(List<float[]> msList)
        {
            var slist = new SeriesList();
            var s = new Series() { ChartType = ChartType.MS, MarkerType = MarkerType.None, Pen = new System.Windows.Media.Pen(Brushes.Black, 1), FontType = new Typeface("Arial") };
            var x1 = msList.Select(x => (float)(x[0])).ToArray();
            var y1 = msList.Select(x => (float)(x[1])).ToArray();
            for (var i = 0; i < x1.Length; i++)
            {
                s.AddPoint(x1[i], y1[i], x1[i].ToString("0.000"));
            }
            s.IsLabelVisible = true;
            slist.Series.Add(s);
            return slist;
        }
        public static DrawVisualMassSpectrum GetMassSpectrumDrawVisualFromConsensus(List<MsGroup> spectrum)
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
            var slist = GetMassSpectrumSeriesList(ConvertMsgroup2List(spectrum));
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
    }
}
