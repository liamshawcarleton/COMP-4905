using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto
{
    static class EvaluationFunctions
    {
        public enum EvaluationDirection
        {
            Row,
            Column,
            Diagonal
        }

        public static void WinningMoveAvailable(Piece p, Board b)
        {

        }

        public static int[] CommonProperties(EvaluationDirection d, Board b, int x = 0, int y = 0)
        {
            List<Piece> pieceList = new List<Piece>();
            int[] summary = new int[4] { 1, 1, 1, 1 };
            switch (d)
            {
                case EvaluationDirection.Row:
                    for (int i = 0; i < 4; i++)
                    {
                        if (b.GetPiece(i, y) == null) { continue; }
                        else { pieceList.Add(b.GetPiece(i, y)); }
                    }
                    if (pieceList.Count > 1) { summary = pieceList[0].GetBinary(); }
                    else { return summary; }
                    for (int i = 1; i < pieceList.Count; i++)
                    {
                        summary = pieceList[i].BinaryXOR(summary);
                    }
                    return summary;
                case EvaluationDirection.Column:
                    for (int i = 0; i < 4; i++)
                    {
                        if (b.GetPiece(x, i) == null) { continue; }
                        else { pieceList.Add(b.GetPiece(x, i)); }
                    }
                    if (pieceList.Count > 1) { summary = pieceList[0].GetBinary(); }
                    else { return summary; }
                    for (int i = 1; i < pieceList.Count; i++)
                    {
                        summary = pieceList[i].BinaryXOR(summary);
                    }
                    return summary;
                case EvaluationDirection.Diagonal:
                    if (x == y)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (b.GetPiece(i, i) == null) { continue; }
                            else
                            {
                                pieceList.Add(b.GetPiece(i, i));
                            }
                        }
                    }
                    else if (x + y == 3)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (b.GetPiece(3-i, i) == null) { continue; }
                            else { pieceList.Add(b.GetPiece(3-i, i)); }
                        }
                    }
                    else
                    {
                        //point given is not on a diagonal
                        return new int[4] { 0, 0, 0, 0 };
                    }
                    if (pieceList.Count > 1) { summary = pieceList[0].GetBinary(); }
                    else { return summary; }
                    for (int i = 1; i < pieceList.Count; i++)
                    {
                        summary = pieceList[i].BinaryXOR(summary);
                    }
                    return summary;
            }
            return null;
        }

        public static int SpotsRemaining(EvaluationDirection d, Board b, int x, int y)
        {
            int count = 0;
            switch (d)
            {
                case EvaluationDirection.Row:
                    for (int i = 0; i < 4; i++)
                    {
                        if (b.GetPiece(i, y) == null) { count++; }
                    }
                    break;
                case EvaluationDirection.Column:
                    for (int i = 0; i < 4; i++)
                    {
                        if (b.GetPiece(x, i) == null) { count++; }
                    }
                    break;
                case EvaluationDirection.Diagonal:
                    if (x == y)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (b.GetPiece(i, i) == null) { count++; }
                        }
                    }
                    else if (x + y == 3)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (b.GetPiece(3 - i, i) == null) { count++; }
                        }
                    }
                    else
                    {
                        return 0;
                    }
                    break;
            }
            return count;
        }

        public static int BinarySum(int[] binary)
        {
            int result = 0;
            foreach(int i in binary)
            {
                result += i;
            }
            return result;
        }

        public static int IsWinningMove(Piece p, Board b, int x = 0, int y = 0)
        {
            int rowSum = BinarySum(p.BinaryXOR(CommonProperties(EvaluationDirection.Row, b, x, y)));
            if (SpotsRemaining(EvaluationDirection.Row, b, x, y) != 1) { rowSum = 0; }
            int columnSum = BinarySum(p.BinaryXOR(CommonProperties(EvaluationDirection.Column, b, x, y)));
            if (SpotsRemaining(EvaluationDirection.Column, b, x, y) != 1) { columnSum = 0; }
            int diagonalSum = BinarySum(p.BinaryXOR(CommonProperties(EvaluationDirection.Diagonal, b, x, y)));
            if (SpotsRemaining(EvaluationDirection.Diagonal, b, x, y) != 1) { diagonalSum = 0; }

            if (rowSum + columnSum + diagonalSum > 0) { return 1; }
            return 0;
        }

        public static int NumberPiecesRemaining(Piece[] remainingPieces)
        {
            int count = 0;
            for (int i = 0; i < remainingPieces.Length; i++)
            {
                if (remainingPieces[i] != null) { count++; }
            }
            return count;
        }

        public static int CommonPiecesRemaining(EvaluationDirection d, Board b, Piece[] remainingPieces, int x = 0, int y = 0)
        {
            int[] binary;
            int count = 0;
            switch (d)
            { 
                case EvaluationDirection.Row:
                    binary = CommonProperties(EvaluationDirection.Row, b, x, y);
                    count = 0;
                    for (int i = 0; i < remainingPieces.Length; i++)
                    {
                        if (remainingPieces[i] != null)
                        {
                            int[] xor = remainingPieces[i].BinaryXOR(binary);
                            if (BinarySum(xor) > 0) { count++; }
                        }
                    }
                    return count;
                case EvaluationDirection.Column:
                    binary = CommonProperties(EvaluationDirection.Column, b, x, y);
                    count = 0;
                    for (int i = 0; i < remainingPieces.Length; i++)
                    {
                        if (remainingPieces[i] != null)
                        {
                            int[] xor = remainingPieces[i].BinaryXOR(binary);
                            if (BinarySum(xor) > 0) { count++; }
                        }
                    }
                    return count;
                case EvaluationDirection.Diagonal:
                    binary = CommonProperties(EvaluationDirection.Diagonal, b, x, y);
                    count = 0;
                    for (int i = 0; i < remainingPieces.Length; i++)
                    {
                        if (remainingPieces[i] != null)
                        {
                            int[] xor = remainingPieces[i].BinaryXOR(binary);
                            if (BinarySum(xor) > 0) { count++; }
                        }
                    }
                    return count;
            }
            return 0;
        }
    }
}
