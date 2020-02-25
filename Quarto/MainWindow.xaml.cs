using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        DNA AI1 = new DNA();
        DNA AI2 = new DNA();
        Player player1;
        Player player2;
        bool pieceSelected = false;

        public MainWindow()
        {
            InitializeComponent();
            AI1.Generate();
            AI2.Generate();
            player1 = new Player("Player1", ref AI1);
            player2 = new Player("Player2", ref AI2);
            player1.active = true;
            player2.active = false;
            player1Slot.SetPlayer(player1);
            player2Slot.SetPlayer(player2);
            pieceSlots.ItemSelected += PieceSelected;
            mainBoard.ItemPlaced += ItemPlaced;
            Thread t = new Thread(() => { MainLoop(); });
            t.Start();
        }

        void MainLoop()
        {
            this.Dispatcher.Invoke(() => { player1Slot.SetPlayerTurn(true); });
            this.Dispatcher.Invoke(() => { player2Slot.SetPlayerTurn(false); });
            Piece firstPick = AI1.RecursivePickPiece(mainBoard.board, pieceSlots.availablePieces);
            Thread.Sleep(1000);
            this.Dispatcher.Invoke(() => { pieceSlots.RemoveItem(firstPick); });
            player2.selectedPiece = firstPick;
            this.Dispatcher.Invoke(() => { player2Slot.SetPiece(firstPick); });
            Thread.Sleep(1000);
            this.Dispatcher.Invoke(() => { ChangeTurns(); });
            while (true)
            {
                if (player1.active)
                {
                    Thread.Sleep(1000);
                    int[] placeLocation = AI1.RecursivePlayPiece(mainBoard.board, player1.selectedPiece, pieceSlots.availablePieces);
                    this.Dispatcher.Invoke(() => { mainBoard.SetPiece(player1.selectedPiece, placeLocation[0], placeLocation[1]); });
                    if (mainBoard.board.CheckWin()) { break; }
                    this.Dispatcher.Invoke(() => { player1Slot.RemovePiece(); });
                    Thread.Sleep(1000);
                    if (pieceSlots.availablePieces.GetRemainingCount() == 0) { break; }
                    Piece pick = AI1.RecursivePickPiece(mainBoard.board, pieceSlots.availablePieces);
                    this.Dispatcher.Invoke(() => { pieceSlots.RemoveItem(pick); });
                    player2.selectedPiece = pick;
                    this.Dispatcher.Invoke(() => { player2Slot.SetPiece(pick); });
                    this.Dispatcher.Invoke(() => { ChangeTurns(); });
                }
                else
                {
                    Thread.Sleep(1000);
                    int[] placeLocation = AI2.PlayPiece(mainBoard.board, player2.selectedPiece, pieceSlots.availablePieces);
                    this.Dispatcher.Invoke(() => { mainBoard.SetPiece(player2.selectedPiece, placeLocation[0], placeLocation[1]); });
                    if (mainBoard.board.CheckWin()) { break; }
                    this.Dispatcher.Invoke(() => { player2Slot.RemovePiece(); });
                    Thread.Sleep(1000);
                    if (pieceSlots.availablePieces.GetRemainingCount() == 0) { break; }
                    Piece pick = AI2.PickPiece(mainBoard.board, pieceSlots.availablePieces);
                    this.Dispatcher.Invoke(() => { pieceSlots.RemoveItem(pick); });
                    player1.selectedPiece = pick;
                    this.Dispatcher.Invoke(() => { player1Slot.SetPiece(pick); });
                    this.Dispatcher.Invoke(() => { ChangeTurns(); });
                }
            }
            if (mainBoard.board.CheckWin())
            {
                if (player1.active) { MessageBox.Show(player1.name + " Wins!"); }
                else { MessageBox.Show(player2.name + " Wins!"); }
            }
            else { MessageBox.Show("Draw!"); }
            this.Dispatcher.Invoke(() => { this.Close(); });
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

        private void btnBinaryXOR_Click(object sender, RoutedEventArgs e)
        {
            BinaryXOR_View v = new BinaryXOR_View(mainBoard.board);
            v.ShowDialog();
        }

        private void btnSpotsRemaining_Click(object sender, RoutedEventArgs e)
        {
            SpotsRemaining_View v = new SpotsRemaining_View(mainBoard.board);
            v.ShowDialog();
        }

        private void btnEvaluate_Click(object sender, RoutedEventArgs e)
        {
            if (player1.active)
            {
                int[] location = AI2.PlayPiece(mainBoard.board, player1.selectedPiece, pieceSlots.availablePieces);
                MessageBox.Show("[" + location[0] + "," + location[1] + "]");
            }
            else
            {
                int[] location = AI2.PlayPiece(mainBoard.board, player2.selectedPiece, pieceSlots.availablePieces);
                MessageBox.Show("[" + location[0] + "," + location[1] + "]");
            }
        }
    }
}
