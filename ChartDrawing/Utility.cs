using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
namespace ChartDrawing
{
    public class Utility
    {
        #region XY axis setting in Draw Visual to set fixed max and min value
        public static void SetDrawingMinAndMaxXYConstValue(DrawVisual drawing, float minX = float.MinValue, float maxX = float.MinValue, float minY = float.MinValue, float maxY = float.MinValue) {
            if (minX > float.MinValue) drawing.SeriesList.MinX = minX;
            if (maxX > float.MinValue) drawing.SeriesList.MaxX = maxX;
            if (minY > float.MinValue) drawing.SeriesList.MinY = minY;
            if (maxY > float.MinValue) drawing.SeriesList.MaxY = maxY;
        }

        public static void SetDrawingMinXRatio(DrawVisual drawing, float ratio) {
            drawing.SeriesList.MinX -= (drawing.SeriesList.MaxX - drawing.SeriesList.MinX) * ratio;
        }

        public static void SetDrawingMinYRatio(DrawVisual drawing, float ratio) {
            drawing.SeriesList.MinY -= (drawing.SeriesList.MaxY - drawing.SeriesList.MinY) * ratio;
        }

        public static void SetDrawingMaxXRatio(DrawVisual drawing, float ratio) {
            drawing.SeriesList.MaxX += (drawing.SeriesList.MaxX - drawing.SeriesList.MinX) * ratio;
        }

        public static void SetDrawingMaxYRatio(DrawVisual drawing, float ratio) {
            drawing.SeriesList.MaxY += (drawing.SeriesList.MaxY - drawing.SeriesList.MinY) * ratio;
        }
        #endregion


        public static Area GetDefaultAreaV1(string xlabel = "Retention time (min)", string ylabel = "RT diff (Sample - Reference) (sec)") {
            var area = new Area() {
                AxisX = new AxisX() { AxisLabel = xlabel, Pen = new Pen(Brushes.Black, 0.5), FontSize = 12 },
                AxisY = new AxisY() { AxisLabel = ylabel, Pen = new Pen(Brushes.Black, 0.5), FontSize = 12 }
            };
            return area;
        }

        public static Area GetDefaultAreaV2(string xlabel = "Retention time (min)") {
            var area = new Area() {
                AxisX = new AxisX() { AxisLabel = xlabel, Pen = new Pen(Brushes.DarkGray, 0.5), FontSize = 12 },
                AxisY = new AxisY() { AxisLabel = "", Pen = new Pen(Brushes.DarkGray, 0.5), FontSize = 12 },
                Margin = new Margin(20, 30, 10, 40)
            };
            return area;
        }

        public static Title GetDefaultTitleV1(int fontsize = 13, string label = "Overview: Retention time correction") {
            return new Title() { FontSize = fontsize, Label = label };
        }

        public static DrawVisual GetLineChartV1(List<List<float[]>> targetListList) {
            var area = GetDefaultAreaV1();
            var title = GetDefaultTitleV1();
            var slist = new SeriesList();
            var brush = Brushes.Blue;
            foreach(var targetList in targetListList) {
                var s = new Series() {
                    ChartType = ChartType.Line,
                    MarkerType = MarkerType.None,
                    MarkerSize = new System.Windows.Size(2, 2),
                    Brush = brush,
                    Pen = new Pen(brush, 1.0)
                };
                foreach(var value in targetList) {
                    s.AddPoint(value[0], value[1]);
                }
                if (s.Points.Count > 0) slist.Series.Add(s);
            }
            return new DrawVisual(area, title, slist);
        }

        public static SolidColorBrush CombineAlphaAndColor(double opacity, SolidColorBrush baseBrush) {
            Color color = baseBrush.Color;
            SolidColorBrush returnSolidColorBrush;

            // Deal with )pacity
            if (opacity > 1.0)
                opacity = 1.0;

            if (opacity < 0.0)
                opacity = 0.0;

            // Get the Hex value of the Alpha Chanel (Opacity)
            byte a = (byte)(Convert.ToInt32(255 * opacity));

            try {
                byte r = color.R;
                byte g = color.G;
                byte b = color.B;

                returnSolidColorBrush = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            }
            catch {
                returnSolidColorBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }
            return returnSolidColorBrush;
        }

        public static double RoundUp(double dValue, int iDigits) {
            double dCoef = System.Math.Pow(10, iDigits);

            return dValue > 0 ? System.Math.Ceiling(dValue * dCoef) / dCoef :
                                System.Math.Floor(dValue * dCoef) / dCoef;
        }

        public static double RoundDown(double dValue, int iDigits) {
            double dCoef = System.Math.Pow(10, iDigits);

            return dValue > 0 ? System.Math.Floor(dValue * dCoef) / dCoef :
                                System.Math.Ceiling(dValue * dCoef) / dCoef;
        }

    }
}
