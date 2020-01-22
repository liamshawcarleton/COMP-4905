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
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : UserControl
    {
        public Board board;
        public EventHandler<ItemPlacedEventArgs> ItemPlaced;

        public GameBoard()
        {
            InitializeComponent();
            Setup();
        }

        private void Setup()
        {
            Bitmap bmp = new Bitmap(64, 64);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Gray))
                {
                    g.FillRectangle(brush, 0, 0, 64, 64);
                }
            }

            using (MemoryStream m = new MemoryStream())
            {
                bmp.Save(m, System.Drawing.Imaging.ImageFormat.Bmp);
                m.Position = 0;
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = m;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();

                foreach (System.Windows.Controls.Image i in FindVisualChildren<System.Windows.Controls.Image>(boardGrid))
                {
                    i.Source = img;
                }
            }

            board = new Board();
        }

        private void BoardSquareSelected(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image item = (System.Windows.Controls.Image)sender;
            ItemPlacedEventArgs args = new ItemPlacedEventArgs();
            string[] itemName = item.Name.Split('_');
            string coordinates = itemName[itemName.Length - 1];
            int x = Convert.ToInt32(coordinates[0].ToString());
            int y = Convert.ToInt32(coordinates[1].ToString());
            if (board.board[x, y] == null)
            {
                args.x = x;
                args.y = y;
                ItemPlaced(this, args);
            }
        }

        //https://stackoverflow.com/questions/974598/find-all-controls-in-wpf-window-by-type
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public void SetPiece(Piece p, int x, int y)
        {
            board.SetPiece(p, x, y);

            foreach (System.Windows.Controls.Image z in FindVisualChildren<System.Windows.Controls.Image>(boardGrid))
            {
                string[] iName = z.Name.Split('_');
                int i = Convert.ToInt32(iName[1][0].ToString());
                int j = Convert.ToInt32(iName[1][1].ToString());
                if (i == x && j == y) { z.Source = p.GetImageSource(); break; }
            }
        }
    }
}
