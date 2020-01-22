using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quarto
{
    /// <summary>
    /// Interaction logic for PlayerSlot.xaml
    /// </summary>
    public partial class PlayerSlot : UserControl
    {
        Piece selectedPiece;
        Player player;
        Bitmap image;

        public PlayerSlot()
        {
            InitializeComponent();
        }

        public void SetPlayer(Player p)
        {
            this.player = p;
            SetPlayerTurn(p.active);
            lblPlayerName.Content = player.name;
        }

        public void SetPlayerTurn(bool turn)
        {
            player.active = turn;
            Bitmap bmp = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                if (turn)
                {
                    using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Green))
                    {
                        g.FillRectangle(brush, 0, 0, 32, 32);
                    }
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Red))
                    {
                        g.FillRectangle(brush, 0, 0, 32, 32);
                    }
                }
            }
            image = bmp;

            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, System.Drawing.Imaging.ImageFormat.Bmp);
                m.Position = 0;
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = m;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                imgPlayersTurn.Source = img;
            }
        }

        public void SetPiece(Piece p)
        {
            selectedPiece = p;
            imgSelectedPiece.Source = selectedPiece.GetImageSource();
        }

        public void RemovePiece()
        {
            if (selectedPiece != null) { selectedPiece = null; imgSelectedPiece.Source = null; }
        }
    }
}
