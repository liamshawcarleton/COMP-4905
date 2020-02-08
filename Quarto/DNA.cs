using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto
{
    class DNA
    {
        double[] Coefficients = new double[11];
        public int SearchDepth { get; set; } = 3;

        public DNA()
        {

        }

        public void Generate()
        {
            Random rnd = new Random();
            for (int i = 0; i < Coefficients.Length; i++)
            {
                Coefficients[i] = rnd.NextDouble();
            }
        }

        public int[] PlayPiece(Board b, Piece p)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            Dictionary<int[], double> evaluationDict = new Dictionary<int[], double>();
            foreach(int[] coordinates in playableLocations)
            {
                evaluationDict.Add(coordinates, Evaluate(b, p, coordinates));
            }

            List<int[]> sorted = evaluationDict.OrderBy(x => x.Value).ToDictionary(pair => pair.Key).Keys.ToList();
            return sorted[0];
        }

        public double Evaluate(Board b, Piece p, int[] coordinates)
        {
            double v1 = Coefficients[0] * EvaluationFunctions.BinarySum(EvaluationFunctions.CommonProperties(EvaluationFunctions.EvaluationDirection.Row, b, coordinates[0], coordinates[1]));
            double v2 = Coefficients[1] * EvaluationFunctions.BinarySum(EvaluationFunctions.CommonProperties(EvaluationFunctions.EvaluationDirection.Column, b, coordinates[0], coordinates[1]));
            double v3 = Coefficients[2] * EvaluationFunctions.BinarySum(EvaluationFunctions.CommonProperties(EvaluationFunctions.EvaluationDirection.Diagonal, b, coordinates[0], coordinates[1]));
            double v4 = Coefficients[3] * EvaluationFunctions.SpotsRemaining(EvaluationFunctions.EvaluationDirection.Row, b, coordinates[0], coordinates[1]);
            double v5 = Coefficients[4] * EvaluationFunctions.SpotsRemaining(EvaluationFunctions.EvaluationDirection.Column, b, coordinates[0], coordinates[1]);
            double v6 = Coefficients[5] * EvaluationFunctions.SpotsRemaining(EvaluationFunctions.EvaluationDirection.Diagonal, b, coordinates[0], coordinates[1]);
            double v7 = Coefficients[6] * AvailablePieces.GetRemainingCount();
            double v8 = Coefficients[7] * EvaluationFunctions.CommonPiecesRemaining(EvaluationFunctions.EvaluationDirection.Row, b, AvailablePieces.RemainingPieces, coordinates[0], coordinates[1]);
            double v9 = Coefficients[8] * EvaluationFunctions.CommonPiecesRemaining(EvaluationFunctions.EvaluationDirection.Column, b, AvailablePieces.RemainingPieces, coordinates[0], coordinates[1]);
            double v10 = Coefficients[9] * EvaluationFunctions.CommonPiecesRemaining(EvaluationFunctions.EvaluationDirection.Diagonal, b, AvailablePieces.RemainingPieces, coordinates[0], coordinates[1]);

            return v1 + v2 + v3 + v4 + v5 + v6 + v7 + v8 + v9 + v10;
        }

        public Piece PickPiece()
        {
            int count = AvailablePieces.GetRemainingCount();
            Random r = new Random();
            int index = r.Next(0, count);
            int a = 0;
            for (int i = 0; i < 16; i++)
            {
                if (AvailablePieces.RemainingPieces[i] != null) { 
                    if (a == index) { return AvailablePieces.RemainingPieces[i]; }
                    else { a++; }
                }
            }
            return null;
        }

        private void RankMoves()
        {

        }
    }
}
