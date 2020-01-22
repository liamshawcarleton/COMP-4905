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

namespace Quarto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Player player1 = new Player("player 1");
        Player player2 = new Player("player 2");
        bool pieceSelected = false;

        public MainWindow()
        {
            InitializeComponent();
            player1.active = true;
            player2.active = false;
            player1Slot.SetPlayer(player1);
            player2Slot.SetPlayer(player2);
            pieceSlots.ItemSelected += PieceSelected;
            mainBoard.ItemPlaced += ItemPlaced;
        }

        void ChangeTurns()
        {
            if (player1.active)
            {
                player1Slot.SetPlayerTurn(false);
                player2Slot.SetPlayerTurn(true);
            }
            else
            {
                player1Slot.SetPlayerTurn(true);
                player2Slot.SetPlayerTurn(false);
            }
        }

        void PieceSelected(object sender, ItemSelectedEventArgs e)
        {
            if (player1.active && !pieceSelected)
            {
                player2.selectedPiece = e.SelectedPiece;
                player2Slot.SetPiece(e.SelectedPiece);
                pieceSlots.RemoveItem(e.SelectedPiece);
                ChangeTurns();
            }
            else if (player2.active && !pieceSelected)
            {
                player1.selectedPiece = e.SelectedPiece;
                player1Slot.SetPiece(e.SelectedPiece);
                pieceSlots.RemoveItem(e.SelectedPiece);
                ChangeTurns();
            }
        }

        void ItemPlaced(object sender, ItemPlacedEventArgs e)
        {
            if (player1.active && !pieceSelected)
            {
                mainBoard.SetPiece(player1.selectedPiece, e.x, e.y);
                player1.selectedPiece = null;
                player1Slot.RemovePiece();
                if (mainBoard.board.CheckWin()) { MessageBox.Show("player 1 wins"); }
            }
            else if (player2.active && !pieceSelected)
            {
                mainBoard.SetPiece(player2.selectedPiece, e.x, e.y);
                player2.selectedPiece = null;
                player2Slot.RemovePiece();
                if (mainBoard.board.CheckWin()) { MessageBox.Show("player 2 wins"); }
            }
        }
    }
}
