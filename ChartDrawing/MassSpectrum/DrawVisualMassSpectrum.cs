using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartDrawing
{
    public class DrawVisualMassSpectrum : DrawVisual
    {
        public DrawVisualMassSpectrum() { }
        public DrawVisualMassSpectrum(Area area, Title title, SeriesList seriesList, bool isArticleFormat = false) : base(area, title, seriesList, isArticleFormat)
        {
        }

    }
}
