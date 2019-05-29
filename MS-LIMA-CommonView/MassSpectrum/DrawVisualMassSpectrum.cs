using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using Metabolomics.Core;
using ChartDrawing;

namespace Metabolomics.Core
{
    public class DrawVisualMassSpectrum : DrawVisual
    {
        public double Ms2Tol { get; set; } = 0.01;
        public int MinimumNumberOfSampleForVisualization { get; set; }
        public double SelectedPeakMz { get; set; }
        public int NumDecimalPlaces { get; set; } = 3;
        public ActionContainer ActionContainer { get; set; }

        private Pen highlightedPen = new Pen(Brushes.Red, 2.5);

        private Pen consensusPenLow = new Pen(Brushes.Black, 1);
        private Pen consensusPenMedium = new Pen(Brushes.Blue, 1);
        private Pen consensusPenHigh = new Pen(Brushes.Red, 1);

        public DrawVisualMassSpectrum() { }

        public DrawVisualMassSpectrum(Area area, Title title, SeriesList seriesList, ActionContainer ac, double ms2tol, int numDecimalPlaces = 3, int numSamples = 1, bool isArticleFormat = false) : base(area, title, seriesList, isArticleFormat) {
            this.Ms2Tol = ms2tol;
            this.NumDecimalPlaces = numDecimalPlaces;
            this.MinimumNumberOfSampleForVisualization = numSamples;
            this.ActionContainer = ac;
        }
        public DrawVisualMassSpectrum(Area area, Title title, SeriesList seriesList, bool isArticleFormat = false) : base(area, title, seriesList, isArticleFormat) { }

        #region default drawing visual         

        public DrawingVisual SetDefaultMassSpectrumDrawingVisual(DrawingVisual drawingVisual)
        {
            MaxX = 100; MaxY = 100; MinY = 0; MinX = 0;
            if (string.IsNullOrWhiteSpace(Title.Label))
            {
                Title.Label = "Mass spectrum";
                Title.FontSize = 15;
            }
            yPacket = (Area.ActualGraphHeight - Area.LabelSpace.Top - Area.LabelSpace.Bottom) / (MaxY - MinY);
            xPacket = Area.ActualGraphWidth / (MaxX - MinX);

            Area.AxisX.AxisLabel = "m/z";
            Area.AxisX.IsItalicLabel = true;
            Area.AxisY.AxisLabel = "Ion intensity";


            drawBackground();
            drawGraphTitle();
            drawCaptionOnAxis();
            drawScaleOnYAxis();
            drawScaleOnXAxis();
            this.drawingContext.Close();// Close DrawingContext
            return drawingVisual;
        }
        #endregion

        public override DrawingVisual GetChart()
        {
            var drawingVisual = new DrawingVisual();

            // return null
            if ((MaxY == MinY && MaxY == 0) || (MaxX == MinX && MaxX == 0)) return drawingVisual;
            if (Area.Width < 2 * (Area.Margin.Left + Area.Margin.Right) || Area.Height < 1.5 * (Area.Margin.Bottom + Area.Margin.Top)) return drawingVisual;

            this.drawingContext = drawingVisual.RenderOpen();
            if (SeriesList.Series.Count == 0) return SetDefaultMassSpectrumDrawingVisual(drawingVisual);

            InitializeGetChart(drawingVisual);
            halfDrawHeight = (Area.ActualGraphHeight - Area.LabelSpace.Top - Area.LabelSpace.Bottom) / 2;

            #region mass spectrum 
            if (SeriesList.Series[0].ChartType == ChartType.MS)
            {
                if(SeriesList.Series[0].Points[0] != null && SeriesList.Series[0].Points[0].Accessory != null && SeriesList.Series[0].Points[0].Accessory.PeakAnnotation.IsMsGroup)
                {
                    DrawMS2PeaksForMsGroup();
                    DrawLabel_MS(SeriesList.Series[0]);
                }
                else
                {
                    DrawMS2Peaks();
                    DrawLabel_MS(SeriesList.Series[0]);
                }
            }
            #endregion

            this.drawingContext.Close();
            return drawingVisual;
        }

        private void DrawMS2Peaks()
        {
            foreach (var xy in SeriesList.Series[0].Points)
            {
                DrawPeak(xy);
            }
        }

        private void DrawMS2PeaksForMsGroup()
        {
            foreach (var xy in SeriesList.Series[0].Points)
            {
                DrawPeakForMsGroup(xy);
            }
        }

        private void DrawPeakForMsGroup(XY xy)
        {
            if (xy.Accessory.PeakAnnotation.Count < MinimumNumberOfSampleForVisualization) return;
            if (xy.Accessory.PeakAnnotation.Count == 1)
            {
                this.drawingContext.DrawLine(consensusPenLow, new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom), new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom + (xy.Y - MinY) * yPacket));
            }
            else if (xy.Accessory.PeakAnnotation.Frequency > 50)
            {
                this.drawingContext.DrawLine(consensusPenHigh, new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom), new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom + (xy.Y - MinY) * yPacket));
            }
            else
            {
                this.drawingContext.DrawLine(consensusPenMedium, new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom), new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom + (xy.Y - MinY) * yPacket));
            }
        }


        private void DrawPeak(XY xy)
        {
            this.drawingContext.DrawLine(SeriesList.Series[0].Pen, new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom), new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom + (xy.Y - MinY) * yPacket));
        }

        public DrawingVisual CheckMousePositionAndHighlight()
        {
            if (SeriesList == null || SeriesList.Series.Count == 0) return null;
            foreach (var xy in SeriesList.Series[0].Points)
            {
                if (Math.Abs(ActionContainer.CurrentMousePoint.X - Area.Margin.Left - (xy.X - MinX) * xPacket) < 5)
                {
                    return DrawHighlightedMs2Peaks(xy);
                }
            }
            return null;
        }

        public DrawingVisual DrawSelectedPeaks()
        {
            if (SeriesList == null || SeriesList.Series.Count == 0) return null;
            foreach (var xy in SeriesList.Series[0].Points)
            {
                if (Math.Abs(SelectedPeakMz - xy.X) < Ms2Tol)
                {
                    return DrawHighlightedMs2Peaks(xy);
                }
            }
            return null;
        }

        public DrawingVisual DrawHighlightedMs2Peaks(XY xy)
        {
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            drawingContext.PushTransform(new TranslateTransform(0, Area.Height)); // 最終的な出力をずらすかどうか。0, drawHeightだと移動なし
            drawingContext.PushTransform(new ScaleTransform(1, -1)); // スケールの変更。-1がついているのでY軸で反転
            if (!isArticleFormat)
                drawingContext.PushClip(new RectangleGeometry(new Rect(Area.Margin.Left, Area.Margin.Bottom, Area.ActualGraphWidth, Area.ActualGraphHeight))); //指定した領域にだけ描画できる。

            drawingContext.DrawLine(highlightedPen, new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom), new Point(this.Area.Margin.Left + (xy.X - MinX) * xPacket, Area.LabelSpace.Bottom + Area.Margin.Bottom + (xy.Y - MinY) * yPacket));

            if (xy.Accessory == null || xy.Accessory.PeakAnnotation == null)
            {
                DrawLabel(xy, drawingContext);
            }else if (xy.Accessory.PeakAnnotation.IsMsGroup)
            {
                DrawConsensusLabelWithAnnotation(xy, drawingContext);
            }
            else
            {
                DrawLabelWithAnnotation(xy, drawingContext);
            }
            drawingContext.Close();
            return drawingVisual;
        }


        public void DrawConsensusLabelWithAnnotation(XY xy, DrawingContext drawingContext)
        {
            drawingContext.PushTransform(new ScaleTransform(1, -1)); // スケールの変更。-1がついているのでY軸で反転
            drawingContext.PushTransform(new TranslateTransform(0, -Area.Height)); // 最終的な出力をずらすかどうか。0, drawHeightだと移動なし

            var xmargine = 12;
            var ymargine = 3;
            var ymargine2 = 5;
            var ymargine3 = 10;
            var fontsize = 30;
            var fontsize2 = 20;
            var fontsize3 = 15;
            var space = 5;
            var labelMargin = 10;

            if (halfDrawHeight - 5 < fontsize * 2 + ymargine * 2 + ymargine2 * 3 + fontsize3 * 2)
            {
                xmargine = 5;
                ymargine = 3;
                ymargine2 = 3;
                ymargine3 = 5;
                fontsize = 15;
                fontsize2 = 10;
                fontsize3 = 7;
                space = 3;
            }

            var back = ChartDrawing.Utility.CombineAlphaAndColor(0.9, Brushes.White);

            var labelMz = System.Math.Round(xy.X, 3).ToString();
            var labelInt = xy.Accessory.PeakAnnotation.RelInt+"%";
            //double count = (msGroup.Counter);
            //double num = (msGroup.IntList.Length);
            //int ratio = count / num > 1 ? 100 : (int)((count * 100) / num);

            var thirdLine = "Count: " + xy.Accessory.PeakAnnotation.Count + " (" + xy.Accessory.PeakAnnotation.Frequency + "%)";
            var thirdLine2 = "m/z: " + xy.Accessory.PeakAnnotation.MzRange +  ", Int: " + xy.Accessory.PeakAnnotation.IntRange;
            var comment = "No annotation";
            if (!string.IsNullOrEmpty(xy.Accessory.PeakAnnotation.Formula))
                if (!string.IsNullOrEmpty(xy.Accessory.PeakAnnotation.Smiles))
                    comment = xy.Accessory.PeakAnnotation.Formula + ", " + xy.Accessory.PeakAnnotation.Smiles;
                else
                    comment = xy.Accessory.PeakAnnotation.Formula;
            else if (!string.IsNullOrEmpty(xy.Accessory.PeakAnnotation.Smiles))
                comment = xy.Accessory.PeakAnnotation.Smiles;
            var fourthLine = comment;
            var mz = new FormattedText(labelMz, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize, Brushes.Black);
            var Int = new FormattedText(labelInt, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize, Brushes.Black);
            var ThirdLine = new FormattedText(thirdLine, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize3, Brushes.Black);
            var ThirdLine2 = new FormattedText(thirdLine2, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize3, Brushes.Black);
            var FourthLine = new FormattedText(fourthLine, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize3, Brushes.Black);
            var mzFormatted = new FormattedText("m/z:  ", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize2, Brushes.Black);
            var intFormatted = new FormattedText("Intensity:  ", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize2, Brushes.Black);
            var mzWidth = mzFormatted.Width;
            var intWidth = intFormatted.Width;


            double tmp1 = Math.Max(Int.Width + intWidth, mz.Width + mzWidth);
            double tmp2 = Math.Max(ThirdLine2.Width, FourthLine.Width);
            double textWidth = Math.Max(tmp1, tmp2);
            var ystart = halfDrawHeight + Area.Margin.Top + space;
            var xstart = 0.0;
            var xp = (xy.X - MinX) * xPacket + Area.Margin.Left;
            if (xp + labelMargin + textWidth + xmargine * 2 < Area.ActualGraphWidth)
                xstart = xp + labelMargin - xmargine;
            else
                xstart = xp - labelMargin - textWidth - xmargine;

            drawingContext.DrawRectangle(back, new Pen(Brushes.Red, 1.0), new Rect(xstart, ystart, textWidth + xmargine * 2, fontsize * 2 + fontsize3 * 3 + ymargine * 2 + ymargine2 * 6));
            drawingContext.DrawText(mzFormatted, new Point(xstart + xmargine, ystart + ymargine + ymargine3));
            drawingContext.DrawText(intFormatted, new Point(xstart + xmargine, ystart + fontsize + ymargine + ymargine2 + ymargine3));
            drawingContext.DrawText(mz, new Point(xstart + xmargine + mzWidth + space, ystart + ymargine));
            drawingContext.DrawText(Int, new Point(xstart + xmargine + intWidth + space, ystart + fontsize + ymargine + ymargine2));
            drawingContext.DrawText(ThirdLine, new Point(xstart + xmargine, ystart + fontsize * 2 + ymargine + ymargine2 * 2));
            drawingContext.DrawText(ThirdLine2, new Point(xstart + xmargine, ystart + fontsize * 2 + fontsize3 + ymargine + ymargine2 * 3));
            drawingContext.DrawText(FourthLine, new Point(xstart + xmargine, ystart + fontsize * 2 + fontsize3 * 2 + ymargine + ymargine2 * 4));
            DrawStructure(xy, drawingContext);
        }

        public void DrawLabelWithAnnotation(XY xy, DrawingContext drawingContext)
        {
            drawingContext.PushTransform(new ScaleTransform(1, -1)); // スケールの変更。-1がついているのでY軸で反転
            drawingContext.PushTransform(new TranslateTransform(0, -Area.Height)); // 最終的な出力をずらすかどうか。0, drawHeightだと移動なし

            var xmargine = 10;
            var ymargine = 3;
            var ymargine2 = 5;
            var ymargine3 = 10;
            var fontsize = 30;
            var fontsize2 = 20;
            var space = 5;
            var labelMargin = 20;

            if (halfDrawHeight - 5 < fontsize * 2 + ymargine * 2 + ymargine2)
            {
                xmargine = 5;
                ymargine = 3;
                ymargine2 = 3;
                ymargine3 = 5;
                fontsize = 15;
                fontsize2 = 10;
                space = 3;
            }

            var back = ChartDrawing.Utility.CombineAlphaAndColor(0.9, Brushes.White);

            var labelMz = System.Math.Round(xy.X, NumDecimalPlaces).ToString();
            var labelInt = System.Math.Round(xy.Accessory.PeakAnnotation.RelInt, 1) + "%";
            var mz = new FormattedText(labelMz, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize, SeriesList.Series[0].Brush);
            var Int = new FormattedText(labelInt, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize, SeriesList.Series[0].Brush);

            var mzFormatted = new FormattedText("m/z:  ", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize2, Brushes.Black);
            var intFormatted = new FormattedText("Intensity:  ", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize2, Brushes.Black);
            var mzWidth = mzFormatted.Width;
            var intWidth = intFormatted.Width;


            double textWidth = System.Math.Max(Int.Width + intWidth, mz.Width + mzWidth);

            var ystart = halfDrawHeight + Area.Margin.Top + space;
            var xstart = 0.0;
            var xp = (xy.X - MinX) * xPacket + Area.Margin.Left;
            if (xp + labelMargin + textWidth + xmargine * 2 < Area.ActualGraphWidth)
                xstart = xp + labelMargin - xmargine;
            else
                xstart = xp - labelMargin - textWidth - xmargine;

            drawingContext.DrawRectangle(back, new Pen(Brushes.Red, 1.0), new Rect(xstart, ystart, textWidth + xmargine * 2, fontsize * 2 + ymargine * 2 + ymargine2 * 2));
            drawingContext.DrawText(mzFormatted, new Point(xstart + xmargine, ystart + ymargine + ymargine3));
            drawingContext.DrawText(intFormatted, new Point(xstart + xmargine, ystart + fontsize + ymargine + ymargine2 + ymargine3));
            drawingContext.DrawText(mz, new Point(xstart + xmargine + mzWidth + space, ystart + ymargine));
            drawingContext.DrawText(Int, new Point(xstart + xmargine + intWidth + space, ystart + fontsize + ymargine + ymargine2));
            DrawStructure(xy, drawingContext);
        }

        public void DrawLabel(XY xy, DrawingContext drawingContext)
        {
            drawingContext.PushTransform(new ScaleTransform(1, -1)); // スケールの変更。-1がついているのでY軸で反転
            drawingContext.PushTransform(new TranslateTransform(0, -Area.Height)); // 最終的な出力をずらすかどうか。0, drawHeightだと移動なし

            var xmargine = 10;
            var ymargine = 3;
            var ymargine2 = 5;
            var ymargine3 = 10;
            var fontsize = 30;
            var fontsize2 = 20;
            var space = 5;
            var labelMargin = 20;
            
            if (halfDrawHeight - 5 < fontsize * 2 + ymargine * 2 + ymargine2)
            {
                xmargine = 5;
                ymargine = 3;
                ymargine2 = 3;
                ymargine3 = 5;
                fontsize = 15;
                fontsize2 = 10;
                space = 3;
            }

            var back = ChartDrawing.Utility.CombineAlphaAndColor(0.9, Brushes.White);

            var labelMz = System.Math.Round(xy.X, NumDecimalPlaces).ToString();
            var labelInt = System.Math.Round(xy.Y,1);
            var mz = new FormattedText(labelMz, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize, SeriesList.Series[0].Brush);
            var Int = new FormattedText(labelInt.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize, SeriesList.Series[0].Brush);

            var mzFormatted = new FormattedText("m/z:  ", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize2, Brushes.Black);
            var intFormatted = new FormattedText("Intensity:  ", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Calibri"), fontsize2, Brushes.Black);
            var mzWidth = mzFormatted.Width;
            var intWidth = intFormatted.Width;


            double textWidth = System.Math.Max(Int.Width + intWidth, mz.Width + mzWidth);

            var ystart = halfDrawHeight + Area.Margin.Top + Area.LabelSpace.Top + space;
            var xstart = 0.0;
            var xp = (xy.X - MinX) * xPacket + Area.Margin.Left;
            if (xp + labelMargin + Area.Margin.Right + textWidth + xmargine * 2 < Area.ActualGraphWidth)
                xstart = xp + labelMargin - xmargine;
            else
                xstart = xp - labelMargin - textWidth - xmargine;

            drawingContext.DrawRectangle(back, new Pen(Brushes.Red, 1.0), new Rect(xstart, ystart, textWidth + xmargine * 2, fontsize * 2 + ymargine * 2 + ymargine2 * 2));
            drawingContext.DrawText(mzFormatted, new Point(xstart + xmargine, ystart + ymargine + ymargine3));
            drawingContext.DrawText(intFormatted, new Point(xstart + xmargine, ystart + fontsize + ymargine + ymargine2 + ymargine3));
            drawingContext.DrawText(mz, new Point(xstart + xmargine + mzWidth + space, ystart + ymargine));
            drawingContext.DrawText(Int, new Point(xstart + xmargine + intWidth + space, ystart + fontsize + ymargine + ymargine2));
            DrawStructure(xy, drawingContext);
        }

        public void DrawStructure(XY xy, DrawingContext drawingContext)
        {
            if (xy.Accessory == null || xy.Accessory.PeakAnnotation == null || string.IsNullOrEmpty(xy.Accessory.PeakAnnotation.Smiles)) return;
            var image = SmilesUtility.SmilesToMediaImageSource(xy.Accessory.PeakAnnotation.Smiles, (int)halfDrawHeight, (int)halfDrawHeight);

            double xStart;
            var xp = (xy.X - MinX) * xPacket + Area.Margin.Left;
            if (xp + 10 < Area.ActualGraphWidth * 0.5) xStart = xp + 10;
            else xStart = xp - image.Width - 10;
            drawingContext.DrawImage(image, new Rect(xStart, Area.Margin.Top, image.Width, image.Height));
        }
    }
}
