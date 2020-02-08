using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto
{
    public class Board
    {
        public Piece[,] board = new Piece[4, 4];
        public Board()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    board[i, j] = null;
                }
            }
        }
        public Piece GetPiece(int x, int y)
        {
            return board[x, y];
        }
        public bool SetPiece(Piece p, int x, int y)
        {
            if (board[x, y] == null) { board[x, y] = p; return true; }
            return false;
        }
        public bool CheckWin()
        {
            for (int i = 0; i < 4; i++)
            {
                if (CheckRow(i)) { return true; }
                if (CheckColumn(i)) { return true; }
            }
            if (CheckDiagonal(true)) { return true; }
            if (CheckDiagonal(false)) { return true; }
            return false;
        }
        public bool CheckRow(int y)
        {
            //assert 4 pieces exist in the row
            if (RowFull(y))
            {
                int[] totalSum = new int[] { 0, 0, 0, 0 };
                for (int i = 0; i < 4; i++)
                {
                    int[] properties = board[i, y].GetBinary();
                    for (int j = 0; j < 4; j++)
                    {
                        totalSum[j] += properties[j];
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (totalSum[i] == 4 || totalSum[i] == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool CheckColumn(int x)
        {
            //assert 4 pieces exist in the row
            if (ColumnFull(x))
            {
                int[] totalSum = new int[] { 0, 0, 0, 0 };
                for (int i = 0; i < 4; i++)
                {
                    int[] properties = board[x, i].GetBinary();
                    for (int j = 0; j < 4; j++)
                    {
                        totalSum[j] += properties[j];
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (totalSum[i] == 4 || totalSum[i] == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool CheckDiagonal(bool topLeft)
        {
            int[] totalSum = new int[] { 0, 0, 0, 0 };
            int x = 0; int y = 0;
            if (!topLeft) { x = 3; }

            if (DiagonalFull(topLeft))
            {
                for (int i = 0; i < 4; i++)
                {
                    int[] properties = board[x, y].GetBinary();
                    for (int j = 0; j < 4; j++)
                    {
                        totalSum[j] += properties[j];
                    }
                    if (topLeft) { x++; } else { x--; }
                    y++;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (totalSum[i] == 4 || totalSum[i] == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public List<int[]> GetOpenSpots()
        {
            List<int[]> coordinates = new List<int[]>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (board[i,j] == null)
                    {
                        coordinates.Add(new int[] { i, j });
                    }
                }
            }
            return coordinates;
        }
        public Board Copy()
        {
            Board b = new Board();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    b.SetPiece(board[i, j], i, j);
                }
            }
            return b;
        }
        private bool RowFull(int y)
        {
            for (int i = 0; i < 4; i++)
            {
                if (board[i, y] == null) { return false; }
            }
            return true;
        }
        private bool ColumnFull(int x)
        {
            for (int i = 0; i < 4; i++)
            {
                if (board[x, i] == null) { return false; }
            }
            return true;
        }
        private bool DiagonalFull(bool topLeft)
        {
            int x = 0, y = 0;
            if (!topLeft) { x = 3; }
            for (int i = 0; i < 4; i++)
            {
                if (board[x, y] == null) { return false; }
                if (topLeft) { x++; } else { x--; }
                y++;
            }
            return true;
        }
    }
}
