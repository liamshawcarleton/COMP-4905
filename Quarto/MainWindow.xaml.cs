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
        DNA DNA1 = new DNA();
        DNA DNA2 = new DNA();
        Player AI1;
        Player AI2;
        bool pieceSelected = false;

        public MainWindow()
        {
            InitializeComponent();
            /*
            DNA1.BuildPredefinedAI(2);
            DNA2.BuildPredefinedAI(0);
            AI1 = new Player("Hard AI", ref DNA1);
            AI2 = new Player("Human", ref DNA2);
            AI1.active = true;
            AI2.active = false;
            player1Slot.SetPlayer(AI1);
            player2Slot.SetPlayer(AI2);
            */
            pieceSlots.ItemSelected += PieceSelected;
            mainBoard.ItemPlaced += ItemPlaced;
            /*
            Thread t = new Thread(() => { MainLoop(); });
            t.Start();
            */
        }

        void MainLoop()
        {
            bool playing_against_player = true;
            this.Dispatcher.Invoke(() => { player1Slot.SetPlayerTurn(true); });
            this.Dispatcher.Invoke(() => { player2Slot.SetPlayerTurn(false); });
            Piece firstPick = DNA1.RandomPickPiece(mainBoard.board, pieceSlots.availablePieces);
            Thread.Sleep(1000);
            this.Dispatcher.Invoke(() => { pieceSlots.RemoveItem(firstPick); });
            AI2.selectedPiece = firstPick;
            this.Dispatcher.Invoke(() => { player2Slot.SetPiece(firstPick); });
            Thread.Sleep(1000);
            this.Dispatcher.Invoke(() => { ChangeTurns(); });
            while (true)
            {
                if (AI1.active)
                {
                    Thread.Sleep(1000);
                    object[] move = DNA1.MinMaxPlay(mainBoard.board, AI1.selectedPiece, pieceSlots.availablePieces, DNA1.SearchDepth, 0, false);
                    int[] position = (int[])move[0];
                    Piece myPick = (Piece)move[1];
                    //int[] placeLocation = SmartAI.RecursivePlayPiece(mainBoard.board, smartPlayer.selectedPiece, pieceSlots.availablePieces);
                    //this.Dispatcher.Invoke(() => { mainBoard.SetPiece(smartPlayer.selectedPiece, placeLocation[0], placeLocation[1]); });
                    this.Dispatcher.Invoke(() => { mainBoard.SetPiece(AI1.selectedPiece, position[0], position[1]); });

                    if (mainBoard.board.CheckWin()) { break; }
                    this.Dispatcher.Invoke(() => { player1Slot.RemovePiece(); });
                    Thread.Sleep(1000);
                    if (pieceSlots.availablePieces.GetRemainingCount() == 0) { break; }
                    Piece pick = myPick;
                    if (pick == null) { MessageBox.Show("Smart AI Picked Null Piece"); }
                    this.Dispatcher.Invoke(() => { pieceSlots.RemoveItem(pick); });
                    AI2.selectedPiece = pick;
                    this.Dispatcher.Invoke(() => { player2Slot.SetPiece(pick); });
                    this.Dispatcher.Invoke(() => { ChangeTurns(); });
                }
                else
                {
                    if (!playing_against_player)
                    {
                        Thread.Sleep(1000);
                        object[] move = DNA2.MinMaxPlay(mainBoard.board, AI2.selectedPiece, pieceSlots.availablePieces, DNA2.SearchDepth, 0, false);
                        int[] position = (int[])move[0];
                        Piece myPick = (Piece)move[1];
                        //int[] placeLocation = SmartAI.RecursivePlayPiece(mainBoard.board, smartPlayer.selectedPiece, pieceSlots.availablePieces);
                        //this.Dispatcher.Invoke(() => { mainBoard.SetPiece(smartPlayer.selectedPiece, placeLocation[0], placeLocation[1]); });
                        this.Dispatcher.Invoke(() => { mainBoard.SetPiece(AI2.selectedPiece, position[0], position[1]); });

                        if (mainBoard.board.CheckWin()) { break; }
                        this.Dispatcher.Invoke(() => { player2Slot.RemovePiece(); });
                        Thread.Sleep(1000);
                        if (pieceSlots.availablePieces.GetRemainingCount() == 0) { break; }
                        Piece pick = myPick;
                        if (pick == null) { MessageBox.Show("Random AI Picked Null Piece"); }
                        this.Dispatcher.Invoke(() => { pieceSlots.RemoveItem(pick); });
                        AI1.selectedPiece = pick;
                        this.Dispatcher.Invoke(() => { player1Slot.SetPiece(pick); });
                        this.Dispatcher.Invoke(() => { ChangeTurns(); });
                    }
                }
            }
            if (mainBoard.board.CheckWin())
            {
                if (AI1.active) { MessageBox.Show(AI1.name + " Wins!"); }
                else { MessageBox.Show(AI2.name + " Wins!"); }
            }
            else { MessageBox.Show("Draw!"); }
            this.Dispatcher.Invoke(() => { this.Close(); });
        }

        void ChangeTurns()
        {
            if (AI1.active)
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
            if (AI1.active && !pieceSelected)
            {
                AI2.selectedPiece = e.SelectedPiece;
                player2Slot.SetPiece(e.SelectedPiece);
                pieceSlots.RemoveItem(e.SelectedPiece);
                ChangeTurns();
            }
            else if (AI2.active && !pieceSelected)
            {
                AI1.selectedPiece = e.SelectedPiece;
                player1Slot.SetPiece(e.SelectedPiece);
                pieceSlots.RemoveItem(e.SelectedPiece);
                ChangeTurns();
            }
        }

        void ItemPlaced(object sender, ItemPlacedEventArgs e)
        {
            if (AI1.active && !pieceSelected)
            {
                mainBoard.SetPiece(AI1.selectedPiece, e.x, e.y);
                AI1.selectedPiece = null;
                player1Slot.RemovePiece();
                if (mainBoard.board.CheckWin()) { MessageBox.Show("player 1 wins"); }
            }
            else if (AI2.active && !pieceSelected)
            {
                mainBoard.SetPiece(AI2.selectedPiece, e.x, e.y);
                AI2.selectedPiece = null;
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
            if (AI1.active)
            {
                DNA1.PickPiece(this.mainBoard.board, this.pieceSlots.availablePieces);
            }
            else
            {
                DNA1.PickPiece(this.mainBoard.board, this.pieceSlots.availablePieces);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnEasy_Click(object sender, RoutedEventArgs e)
        {
            DNA1.BuildPredefinedAI(0);
            DNA2.BuildPredefinedAI(0);
            AI1 = new Player("Easy AI", ref DNA1);
            AI2 = new Player("Human", ref DNA2);
            AI1.active = true;
            AI2.active = false;
            player1Slot.SetPlayer(AI1);
            player2Slot.SetPlayer(AI2);
            Thread t = new Thread(() => { MainLoop(); });
            t.Start();
        }

        private void BtnMedium_Click(object sender, RoutedEventArgs e)
        {
            DNA1.BuildPredefinedAI(1);
            DNA2.BuildPredefinedAI(0);
            AI1 = new Player("Medium AI", ref DNA1);
            AI2 = new Player("Human", ref DNA2);
            AI1.active = true;
            AI2.active = false;
            player1Slot.SetPlayer(AI1);
            player2Slot.SetPlayer(AI2);
            Thread t = new Thread(() => { MainLoop(); });
            t.Start();
        }

        private void BtnHard_Click(object sender, RoutedEventArgs e)
        {
            DNA1.BuildPredefinedAI(2);
            DNA2.BuildPredefinedAI(0);
            AI1 = new Player("Hard AI", ref DNA1);
            AI2 = new Player("Human", ref DNA2);
            AI1.active = true;
            AI2.active = false;
            player1Slot.SetPlayer(AI1);
            player2Slot.SetPlayer(AI2);
            Thread t = new Thread(() => { MainLoop(); });
            t.Start();
        }
    }
}
