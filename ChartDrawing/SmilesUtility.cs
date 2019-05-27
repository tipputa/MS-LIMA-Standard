using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using Metabolomics.Core;

namespace ChartDrawing
{
    public class SmilesUtility
    {

        public static void TryClassLoad()
        {
            SmilesConverter.TryClassLoad();
        }

        public static System.Windows.Media.Imaging.BitmapImage SmilesToMediaImageSource(string smiles, int width, int height)
        {
            System.Drawing.Image drawingImage = SmilesConverter.SmilesToImage(smiles, width, height);
            return ConvertDrawingImageToBitmap(drawingImage);
        }

        public static System.Windows.Media.Imaging.BitmapImage ConvertDrawingImageToBitmap(System.Drawing.Image image)
        {
            if (image == null) return null;
            var ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            var bi = new System.Windows.Media.Imaging.BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(ms.ToArray());
            bi.EndInit();

            return bi;
        }
    }
}
