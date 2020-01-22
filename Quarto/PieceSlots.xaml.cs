using System;
using System.Collections.Generic;
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
using System.Drawing;

namespace Quarto
{
    public partial class PieceSlots : UserControl
    {
        public EventHandler<ItemSelectedEventArgs> ItemSelected;
        public PieceSlots()
        {
            InitializeComponent();
            FillSlots();
        }

        private void FillSlots()
        {
            slot_0.Source = AvailablePieces.RemainingPieces[0].GetImageSource();
            slot_1.Source = AvailablePieces.RemainingPieces[1].GetImageSource();
            slot_2.Source = AvailablePieces.RemainingPieces[2].GetImageSource();
            slot_3.Source = AvailablePieces.RemainingPieces[3].GetImageSource();
            slot_4.Source = AvailablePieces.RemainingPieces[4].GetImageSource();
            slot_5.Source = AvailablePieces.RemainingPieces[5].GetImageSource();
            slot_6.Source = AvailablePieces.RemainingPieces[6].GetImageSource();
            slot_7.Source = AvailablePieces.RemainingPieces[7].GetImageSource();
            slot_8.Source = AvailablePieces.RemainingPieces[8].GetImageSource();
            slot_9.Source = AvailablePieces.RemainingPieces[9].GetImageSource();
            slot_10.Source = AvailablePieces.RemainingPieces[10].GetImageSource();
            slot_11.Source = AvailablePieces.RemainingPieces[11].GetImageSource();
            slot_12.Source = AvailablePieces.RemainingPieces[12].GetImageSource();
            slot_13.Source = AvailablePieces.RemainingPieces[13].GetImageSource();
            slot_14.Source = AvailablePieces.RemainingPieces[14].GetImageSource();
            slot_15.Source = AvailablePieces.RemainingPieces[15].GetImageSource();
        }

        private void UpdateSlot(int index)
        {
            switch (index)
            {
                case 0:
                    slot_0.Source = null;
                    break;
                case 1:
                    slot_1.Source = null;
                    break;
                case 2:
                    slot_2.Source = null;
                    break;
                case 3:
                    slot_3.Source = null;
                    break;
                case 4:
                    slot_4.Source = null;
                    break;
                case 5:
                    slot_5.Source = null;
                    break;
                case 6:
                    slot_6.Source = null;
                    break;
                case 7:
                    slot_7.Source = null;
                    break;
                case 8:
                    slot_8.Source = null;
                    break;
                case 9:
                    slot_9.Source = null;
                    break;
                case 10:
                    slot_10.Source = null;
                    break;
                case 11:
                    slot_11.Source = null;
                    break;
                case 12:
                    slot_12.Source = null;
                    break;
                case 13:
                    slot_13.Source = null;
                    break;
                case 14:
                    slot_14.Source = null;
                    break;
                case 15:
                    slot_15.Source = null;
                    break;
            }
        }

        public void RemoveItem(Piece piece)
        {
            int? index = null;
            for (int i=0; i < AvailablePieces.RemainingPieces.Length; i++)
            {
                if (AvailablePieces.RemainingPieces[i] != null)
                {
                    if (AvailablePieces.RemainingPieces[i].Equals(piece))
                    {
                        index = i;
                        AvailablePieces.RemainingPieces[i] = null;
                        UpdateSlot(i);
                    }
                }
            }
        }

        private void SlotPressed(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image item = (System.Windows.Controls.Image)sender;
            ItemSelectedEventArgs args = new ItemSelectedEventArgs();
            string[] itemName = item.Name.Split('_');
            int index = Convert.ToInt32(itemName[itemName.Length - 1].ToString());
            if(AvailablePieces.RemainingPieces[index] != null)
            {
                args.SelectedPiece = AvailablePieces.RemainingPieces[index];
                ItemSelected(this, args);
            }
        }
    }
}
