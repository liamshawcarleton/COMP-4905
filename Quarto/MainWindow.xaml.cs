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
        DNA SmartAI = new DNA("99,29,21,8,168,1.8464,1.8464,1.8464,1.3562,1.3562,1.3562,0,0.692,0.692,0.692");
        DNA RandomAI = new DNA("10,10,10,1,177,1.090721314,1.090721314,1.090721314,0.455185238,0.455185238,0.455185238,0,0.778888538,0.778888538,0.778888538");
        Player smartPlayer;
        Player randomPlayer;
        bool pieceSelected = false;

        public MainWindow()
        {
            InitializeComponent();
            //SmartAI.Generate();
            RandomAI.Generate();
            smartPlayer = new Player("Player1", ref SmartAI);
            randomPlayer = new Player("Player2", ref RandomAI);
            smartPlayer.active = true;
            randomPlayer.active = false;
            player1Slot.SetPlayer(smartPlayer);
            player2Slot.SetPlayer(randomPlayer);
            pieceSlots.ItemSelected += PieceSelected;
            mainBoard.ItemPlaced += ItemPlaced;
            Thread t = new Thread(() => { MainLoop(); });
            t.Start();
        }

        void MainLoop()
        {
            
            this.Dispatcher.Invoke(() => { player1Slot.SetPlayerTurn(true); });
            this.Dispatcher.Invoke(() => { player2Slot.SetPlayerTurn(false); });
            Piece firstPick = SmartAI.RandomPickPiece(mainBoard.board, pieceSlots.availablePieces);
            Thread.Sleep(1000);
            this.Dispatcher.Invoke(() => { pieceSlots.RemoveItem(firstPick); });
            randomPlayer.selectedPiece = firstPick;
            this.Dispatcher.Invoke(() => { player2Slot.SetPiece(firstPick); });
            Thread.Sleep(1000);
            this.Dispatcher.Invoke(() => { ChangeTurns(); });
            while (true)
            {
                if (smartPlayer.active)
                {
                    Thread.Sleep(1000);
                    object[] move = SmartAI.MinMaxPlay(mainBoard.board, smartPlayer.selectedPiece, pieceSlots.availablePieces, 2, 0, false);
                    int[] position = (int[])move[0];
                    Piece myPick = (Piece)move[1];
                    //int[] placeLocation = SmartAI.RecursivePlayPiece(mainBoard.board, smartPlayer.selectedPiece, pieceSlots.availablePieces);
                    //this.Dispatcher.Invoke(() => { mainBoard.SetPiece(smartPlayer.selectedPiece, placeLocation[0], placeLocation[1]); });
                    this.Dispatcher.Invoke(() => { mainBoard.SetPiece(smartPlayer.selectedPiece, position[0], position[1]); });

                    if (mainBoard.board.CheckWin()) { break; }
                    this.Dispatcher.Invoke(() => { player1Slot.RemovePiece(); });
                    Thread.Sleep(1000);
                    if (pieceSlots.availablePieces.GetRemainingCount() == 0) { break; }
                    Piece pick = myPick;
                    if (pick == null) { MessageBox.Show("Smart AI Picked Null Piece"); }
                    this.Dispatcher.Invoke(() => { pieceSlots.RemoveItem(pick); });
                    randomPlayer.selectedPiece = pick;
                    this.Dispatcher.Invoke(() => { player2Slot.SetPiece(pick); });
                    this.Dispatcher.Invoke(() => { ChangeTurns(); });
                }
                else
                {
                    /*
                    Thread.Sleep(1000);
                    int[] placeLocation = RandomAI.RandomPlayPiece(mainBoard.board, randomPlayer.selectedPiece, pieceSlots.availablePieces);
                    this.Dispatcher.Invoke(() => { mainBoard.SetPiece(randomPlayer.selectedPiece, placeLocation[0], placeLocation[1]); });
                    if (mainBoard.board.CheckWin()) { break; }
                    this.Dispatcher.Invoke(() => { player2Slot.RemovePiece(); });
                    Thread.Sleep(1000);
                    if (pieceSlots.availablePieces.GetRemainingCount() == 0) { break; }
                    Piece pick = RandomAI.RandomPickPiece(mainBoard.board, pieceSlots.availablePieces);
                    if (pick == null) { MessageBox.Show("Random AI Picked Null Piece"); }
                    this.Dispatcher.Invoke(() => { pieceSlots.RemoveItem(pick); });
                    smartPlayer.selectedPiece = pick;
                    this.Dispatcher.Invoke(() => { player1Slot.SetPiece(pick); });
                    this.Dispatcher.Invoke(() => { ChangeTurns(); });*/
                }
            }
            if (mainBoard.board.CheckWin())
            {
                if (smartPlayer.active) { MessageBox.Show(smartPlayer.name + " Wins!"); }
                else { MessageBox.Show(randomPlayer.name + " Wins!"); }
            }
            else { MessageBox.Show("Draw!"); }
            this.Dispatcher.Invoke(() => { this.Close(); });
        }

        void ChangeTurns()
        {
            if (smartPlayer.active)
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
            if (smartPlayer.active && !pieceSelected)
            {
                randomPlayer.selectedPiece = e.SelectedPiece;
                player2Slot.SetPiece(e.SelectedPiece);
                pieceSlots.RemoveItem(e.SelectedPiece);
                ChangeTurns();
            }
            else if (randomPlayer.active && !pieceSelected)
            {
                smartPlayer.selectedPiece = e.SelectedPiece;
                player1Slot.SetPiece(e.SelectedPiece);
                pieceSlots.RemoveItem(e.SelectedPiece);
                ChangeTurns();
            }
        }

        void ItemPlaced(object sender, ItemPlacedEventArgs e)
        {
            if (smartPlayer.active && !pieceSelected)
            {
                mainBoard.SetPiece(smartPlayer.selectedPiece, e.x, e.y);
                smartPlayer.selectedPiece = null;
                player1Slot.RemovePiece();
                if (mainBoard.board.CheckWin()) { MessageBox.Show("player 1 wins"); }
            }
            else if (randomPlayer.active && !pieceSelected)
            {
                mainBoard.SetPiece(randomPlayer.selectedPiece, e.x, e.y);
                randomPlayer.selectedPiece = null;
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
            if (smartPlayer.active)
            {
                SmartAI.PickPiece(this.mainBoard.board, this.pieceSlots.availablePieces);
            }
            else
            {
                SmartAI.PickPiece(this.mainBoard.board, this.pieceSlots.availablePieces);
            }
        }
    }
}
