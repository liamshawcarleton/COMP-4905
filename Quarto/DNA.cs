using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto
{
    class DNA
    {
        //temporary comment for push
        double[] Coefficients = new double[11];
        public int SearchDepth { get; set; } = 1;

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

        public int[] PlayPiece(Board b, Piece p, AvailablePieces a)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            Dictionary<int[], double> evaluationDict = new Dictionary<int[], double>();
            foreach(int[] coordinates in playableLocations)
            {
                evaluationDict.Add(coordinates, StaticPlayEvaluate(b, p, coordinates, a));
            }

            List<int[]> sorted = evaluationDict.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key).Keys.ToList();
            return sorted[0];
        }

        public int[] RecursivePlayPiece(Board b, Piece p, AvailablePieces a)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            Dictionary<int[], double> staticEval = new Dictionary<int[], double>();
            List<int[]> playableSubset;
            foreach(int[] i in playableLocations)
            {
                staticEval.Add(i, StaticPlayEvaluate(b, p, i, a));
            }
            playableSubset = staticEval.OrderByDescending(x => x.Value).ToDictionary(x => x.Key).Keys.ToList().Take(Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(a.GetRemainingCount() * 1 / 3)))).ToList();
            Dictionary<int[], double> evaluationDict = new Dictionary<int[], double>();
            foreach (int[] coordinates in playableSubset)
            {
                double value = 0;
                Dictionary<int[], double> abc = RecursivePlayEvaluation(b, p, a, SearchDepth);
                foreach(KeyValuePair<int[], double> i in abc)
                {
                    value += i.Value;
                }
                evaluationDict.Add(coordinates, value);
            }

            List<int[]> sorted = evaluationDict.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key).Keys.ToList();
            return sorted[0];
        }

        public double StaticPlayEvaluate(Board b, Piece p, int[] coordinates, AvailablePieces a)
        {
            double v1 = Coefficients[0] * EvaluationFunctions.BinarySum(EvaluationFunctions.CommonProperties(EvaluationFunctions.EvaluationDirection.Row, b, coordinates[0], coordinates[1]));
            double v2 = Coefficients[1] * EvaluationFunctions.BinarySum(EvaluationFunctions.CommonProperties(EvaluationFunctions.EvaluationDirection.Column, b, coordinates[0], coordinates[1]));
            double v3 = Coefficients[2] * EvaluationFunctions.BinarySum(EvaluationFunctions.CommonProperties(EvaluationFunctions.EvaluationDirection.Diagonal, b, coordinates[0], coordinates[1]));
            double v4 = Coefficients[3] * EvaluationFunctions.SpotsRemaining(EvaluationFunctions.EvaluationDirection.Row, b, coordinates[0], coordinates[1]);
            double v5 = Coefficients[4] * EvaluationFunctions.SpotsRemaining(EvaluationFunctions.EvaluationDirection.Column, b, coordinates[0], coordinates[1]);
            double v6 = Coefficients[5] * EvaluationFunctions.SpotsRemaining(EvaluationFunctions.EvaluationDirection.Diagonal, b, coordinates[0], coordinates[1]);
            double v7 = Coefficients[6] * a.GetRemainingCount();
            double v8 = Coefficients[7] * EvaluationFunctions.CommonPiecesRemaining(EvaluationFunctions.EvaluationDirection.Row, b, a.RemainingPieces, coordinates[0], coordinates[1]);
            double v9 = Coefficients[8] * EvaluationFunctions.CommonPiecesRemaining(EvaluationFunctions.EvaluationDirection.Column, b, a.RemainingPieces, coordinates[0], coordinates[1]);
            double v10 = Coefficients[9] * EvaluationFunctions.CommonPiecesRemaining(EvaluationFunctions.EvaluationDirection.Diagonal, b, a.RemainingPieces, coordinates[0], coordinates[1]);

            return v1 + v2 + v3 + v4 + v5 + v6 + v7 + v8 + v9 + v10;
        }

        public double StaticPickEvaluate(Board b, Piece p, AvailablePieces a)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            double value = 0;
            foreach(int[] i in playableLocations)
            {
                AvailablePieces temp = a.Copy();
                temp.RemovePiece(p);
                value += StaticPlayEvaluate(b, p, i, temp);
            }
            return 1/value;
        }

        public Dictionary<int[], double> RecursivePlayEvaluation (Board b, Piece p, AvailablePieces a, int depth)
        {
            if (depth == 0)
            {
                List<int[]> playableLocations = b.GetOpenSpots();
                Dictionary<int[], double> evaluationDict = new Dictionary<int[], double>();
                foreach (int[] coordinates in playableLocations)
                {
                    evaluationDict.Add(coordinates, StaticPlayEvaluate(b, p, coordinates, a));
                }

                evaluationDict = evaluationDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                return evaluationDict;
            }
            else
            {
                List<int[]> playableLocations = b.GetOpenSpots();
                Dictionary<int[], double> evaluationDict = new Dictionary<int[], double>();
                foreach(int[] i in playableLocations)
                {
                    Board temp = b.Copy();
                    temp.SetPiece(p, i[0], i[1]);
                    Dictionary<Piece, double> pickEvaluation = RecursivePickEvaluation(temp, a, depth - 1);
                    double value = 0;
                    foreach(KeyValuePair<Piece, double> pair in pickEvaluation)
                    {
                        value += pair.Value;
                    }
                    evaluationDict.Add(i, value);
                }
                return evaluationDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public Piece PickPiece(Board b, AvailablePieces av)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            Dictionary<Piece, double> evaluationDict = new Dictionary<Piece, double>();
            foreach (Piece p in av.RemainingPieces)
            {
                if (p != null)
                {
                    evaluationDict.Add(p, StaticPickEvaluate(b, p, av));
                }
            }
            List<Piece> sorted = evaluationDict.OrderBy(x => x.Value).ToDictionary(pair => pair.Key).Keys.ToList();
            return sorted[0];
        }

        public Piece RecursivePickPiece(Board b, AvailablePieces av)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            Dictionary<Piece, double> evaluationDict = new Dictionary<Piece, double>();
            foreach (Piece p in av.RemainingPieces)
            {
                if (p != null)
                {
                    double value = 0;
                    Dictionary<Piece, double> abc = RecursivePickEvaluation(b, av, SearchDepth);
                    foreach(KeyValuePair<Piece, double> i in abc)
                    {
                        value += i.Value;
                    }
                    evaluationDict.Add(p, value);
                }
            }
            List<Piece> sorted = evaluationDict.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key).Keys.ToList();
            return sorted[0];
        }

        public Dictionary<Piece, double> RecursivePickEvaluation(Board b, AvailablePieces av, int depth)
        {
            if (depth == 0)
            { 
                Dictionary<Piece, double> evaluationDict = new Dictionary<Piece, double>();
                foreach(Piece p in av.RemainingPieces)
                {
                    if (p != null)
                    {
                        double value = StaticPickEvaluate(b, p, av);
                        evaluationDict.Add(p, value);
                    }
                }
                return evaluationDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                Dictionary<Piece, double> evaluationDict = new Dictionary<Piece, double>();
                foreach (Piece p in av.RemainingPieces) 
                {
                    if (p != null)
                    {
                        Dictionary<int[], double> playDict = RecursivePlayEvaluation(b, p, av, depth - 1);
                        double value = 0;
                        foreach (KeyValuePair<int[], double> i in playDict)
                        {
                            value += 1 / i.Value;
                        }
                        evaluationDict.Add(p, value);
                    }
                }
                return evaluationDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }
}
