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
        public AvailablePieces availablePieces = new AvailablePieces();
        public PieceSlots()
        {
            InitializeComponent();
            FillSlots();
        }

        private void FillSlots()
        {
            slot_0.Source = availablePieces.RemainingPieces[0].GetImageSource();
            slot_1.Source = availablePieces.RemainingPieces[1].GetImageSource();
            slot_2.Source = availablePieces.RemainingPieces[2].GetImageSource();
            slot_3.Source = availablePieces.RemainingPieces[3].GetImageSource();
            slot_4.Source = availablePieces.RemainingPieces[4].GetImageSource();
            slot_5.Source = availablePieces.RemainingPieces[5].GetImageSource();
            slot_6.Source = availablePieces.RemainingPieces[6].GetImageSource();
            slot_7.Source = availablePieces.RemainingPieces[7].GetImageSource();
            slot_8.Source = availablePieces.RemainingPieces[8].GetImageSource();
            slot_9.Source = availablePieces.RemainingPieces[9].GetImageSource();
            slot_10.Source = availablePieces.RemainingPieces[10].GetImageSource();
            slot_11.Source = availablePieces.RemainingPieces[11].GetImageSource();
            slot_12.Source = availablePieces.RemainingPieces[12].GetImageSource();
            slot_13.Source = availablePieces.RemainingPieces[13].GetImageSource();
            slot_14.Source = availablePieces.RemainingPieces[14].GetImageSource();
            slot_15.Source = availablePieces.RemainingPieces[15].GetImageSource();
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
            for (int i=0; i < availablePieces.RemainingPieces.Length; i++)
            {
                if (availablePieces.RemainingPieces[i] != null)
                {
                    if (availablePieces.RemainingPieces[i].Equals(piece))
                    {
                        index = i;
                        availablePieces.RemainingPieces[i] = null;
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
            if(availablePieces.RemainingPieces[index] != null)
            {
                args.SelectedPiece = availablePieces.RemainingPieces[index];
                ItemSelected(this, args);
            }
        }
    }
}
