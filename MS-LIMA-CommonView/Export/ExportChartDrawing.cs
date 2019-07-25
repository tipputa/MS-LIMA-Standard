using System;
using System.Collections.Generic;
using System.Text;
using ChartDrawing;

namespace Metabolomics.MsLima.Exporter
{
    public class ExportDrawVisual
    {
        public static void SaveAsEmf(string filePath, DrawVisual dv, float minX, float maxX, float minY, float maxY, double width, double height, bool isArticleFormat = false)
        {
            var slist = new Series();
            foreach(var s in dv.SeriesList.Series[0].Points)
            {
                if (s.X < minX) continue;
                if (s.X > maxX) break;
                slist.AddPoint(s.X, s.Y, s.Label);
            }
            dv.SeriesList.Series[0].Points = slist.Points;
            dv.ChangeChartArea(width, height);
            dv.MinX = minX;
            dv.MaxX = maxX;
            dv.MinY = minY;
            dv.MaxY = maxY;
            dv.GetChart();
            dv.isArticleFormat = isArticleFormat;
            dv.SaveDrawingAsEmf(dv.GetChart(), filePath);
        }

        public static void SaveAsPng(string filePath, DrawVisual dv, float minX, float maxX, float minY, float maxY, double width, double height, int dpiX, int dpiY, bool isArticleFormat = false)
        {
            var slist = new Series();
            foreach (var s in dv.SeriesList.Series[0].Points)
            {
                if (s.X < minX) continue;
                if (s.X > maxX) break;
                slist.AddPoint(s.X, s.Y, s.Label);
            }
            dv.SeriesList.Series[0].Points = slist.Points;

            dv.ChangeChartArea(width, height);
            dv.MinX = minX;
            dv.MaxX = maxX;
            dv.MinY = minY;
            dv.MaxY = maxY;
            dv.GetChart();
            dv.isArticleFormat = isArticleFormat;
            dv.SaveChart(dv.GetChart(), filePath, (int)width, (int)height, dpiX, dpiY);
        }

    }
}
