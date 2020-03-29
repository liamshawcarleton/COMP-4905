using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;

namespace Quarto
{
    public class Piece
    {
        bool solid;
        bool round;
        bool black;
        bool large;
        public Bitmap image { get; private set; }

        public Piece(bool large, bool round, bool solid, bool black, Bitmap image)
        {
            this.large = large;
            this.round = round;
            this.solid = solid;
            this.black = black;
            this.image = image;
        }

        public int[] GetBinary()
        {
            int[] ret = new int[4];
            ret[0] = solid ? 1 : 0;
            ret[1] = round ? 1 : 0;
            ret[2] = black ? 1 : 0;
            ret[3] = large ? 1 : 0;
            return ret;
        }

        public int[] BinaryXOR(Piece p)
        {
            int[] result = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (p.GetBinary()[i] == this.GetBinary()[i]) { result[i] = 1; }
                else { result[i] = 0; }
            }
            return result;
        }

        public int[] BinaryXOR(int[] binary)
        {
            int[] result = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (binary[i] == this.GetBinary()[i]) { result[i] = 1; }
                else { result[i] = 0; }
            }
            return result;
        }

        public BitmapImage GetImageSource()
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, System.Drawing.Imaging.ImageFormat.Png);
                m.Position = 0;
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = m;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                return img;
            }
        }
    }
}
