using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;


namespace ChartDrawing
{
    public class DrawVisualMassSpectrum : DrawVisual
    {
        public double SelectedMz = -1;
        public double Ms2Tol = 0.01;
        public DrawVisualMassSpectrum() { }
        public DrawVisualMassSpectrum(Area area, Title title, SeriesList seriesList, double selectedMz, double ms2tol, bool isArticleFormat = false) : base(area, title, seriesList, isArticleFormat) {
            this.SelectedMz = selectedMz;
            this.Ms2Tol = ms2tol;
        }
        public DrawVisualMassSpectrum(Area area, Title title, SeriesList seriesList, bool isArticleFormat = false) : base(area, title, seriesList, isArticleFormat) { }

        public override DrawingVisual GetChart()
        {
            var drawingVisual = new DrawingVisual();

            // return null
            if ((MaxY == MinY && MaxY == 0) || (MaxX == MinX && MaxX == 0)) return drawingVisual;
            if (Area.Width < 2 * (Area.Margin.Left + Area.Margin.Right) || Area.Height < 1.5 * (Area.Margin.Bottom + Area.Margin.Top)) return drawingVisual;

            this.drawingContext = drawingVisual.RenderOpen();
            if (SeriesList.Series.Count == 0) return SetDefaultDrawingVisual(drawingVisual);

            InitializeGetChart(drawingVisual);

            #region mass spectrum 
            if (SeriesList.Series[0].ChartType == ChartType.MS)
            {
                DrawMS2Peaks();
                DrawLabel_MS(SeriesList.Series[0]);
            }
            #endregion

            this.drawingContext.Close();
            return drawingVisual;
        }

        private void DrawMS2Peaks()
        {
            if (SelectedMz < 0)
            {
                foreach (var xy in SeriesList.Series[0].Points)
                {
                    this.drawingContext.DrawLine(SeriesList.Series[0].Pen, new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom), new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom + (xy.Y - MinY) * yPacket));
                }
            }
            else
            {
                foreach (var xy in SeriesList.Series[0].Points)
                {
                    if (Math.Abs(xy.X - SelectedMz) < Ms2Tol)
                    {
                        DrawHighlightedMS2Peak();
                    }
                    else
                    {
                        this.drawingContext.DrawLine(SeriesList.Series[0].Pen, new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom), new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom + (xy.Y - MinY) * yPacket));
                    }
                }
            }
        }

        private void DrawHighlightedMS2Peak()
        {

        }

    }
}
