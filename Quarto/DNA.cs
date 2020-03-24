using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Quarto
{
    public class DNA
    {
        public static Random rnd = new Random();
        public static double MutationProbability = 0.3;
        public static double MutationGradientMin = 0.2;
        public static double MutationGradientMax = 0.6;
        public static int CrossoverMinimum = 2;
        public static int CrossoverMaximum = 9;

        public double[] Coefficients = new double[10];
        public double Fitness { get; set; } = 0;
        public int GamesPlayed { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int TotalMoves { get; set; } = 0;

        public DNA()
        {

        }

        public DNA(string progress) { LoadFromProgress(progress); }

        public void Generate()
        {
            for (int i = 0; i < Coefficients.Length; i++)
            {
                int makenegative = rnd.Next(0, 2) * 2 - 1;
                Coefficients[i] = makenegative * rnd.NextDouble();
            }
        }

        public void NewGenerate()
        {
            Coefficients[7] = rnd.NextDouble();
            GenerateBinarySum();
            GenerateSpotsRemaining();
            GenerateCommonPieces();
        }

        private void GenerateBinarySum()
        {
            double min_value = 0.3;
            double max_value = 0.9;
            double initial_value = rnd.NextDouble() * (max_value - min_value) + min_value;
            Coefficients[0] = initial_value;
            Coefficients[1] = initial_value;
            Coefficients[2] = initial_value;
        }

        private void GenerateSpotsRemaining()
        {
            double min_value = -0.3;
            double max_value = 0.3;
            double initial_value = rnd.NextDouble() * (max_value - min_value) + min_value;
            Coefficients[3] = initial_value;
            Coefficients[4] = initial_value;
            Coefficients[5] = initial_value;
        }

        private void GenerateCommonPieces()
        {
            double min_value = -0.3;
            double max_value = 0.3;
            double initial_value = rnd.NextDouble() * (max_value - min_value) + min_value;
            Coefficients[7] = initial_value;
            Coefficients[8] = initial_value;
            Coefficients[9] = initial_value;
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
            int piecesLeft = a.GetRemainingCount();
            int searchDepth;
            if (piecesLeft > 0)
            {
                searchDepth = Convert.ToInt32(Math.Floor(Convert.ToDecimal(a.RemainingPieces.Length / (piecesLeft))));
                if (searchDepth > 4) { searchDepth = 4; }
            }
            else
            {
                searchDepth = 0;
            }
            playableSubset = staticEval.OrderByDescending(x => x.Value).ToDictionary(x => x.Key).Keys.ToList().Take(Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(a.GetRemainingCount() * 1 / (4-searchDepth + 1))))).ToList();
            Dictionary<int[], double> evaluationDict = new Dictionary<int[], double>();
            /*foreach(Piece temp in a.RemainingPieces)
            {
                if (temp != null) { piecesLeft++; }
            }*/
            foreach (int[] coordinates in playableSubset)
            {
                double value = 0;
                Dictionary<int[], double> abc = RecursivePlayEvaluation(b, p, a, searchDepth);
                foreach(KeyValuePair<int[], double> i in abc)
                {
                    value += i.Value;
                }
                evaluationDict.Add(coordinates, value);
            }

            List<int[]> sorted = evaluationDict.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key).Keys.ToList();
            if (sorted.Count == 0) { return PlayPiece(b, p, a); }
            return sorted[0];
        }

        public int[] RandomPlayPiece(Board b, Piece p, AvailablePieces a)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            int rng = rnd.Next(playableLocations.Count - 1);
            return playableLocations[rng];
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
            int v11 = EvaluationFunctions.Winnable(EvaluationFunctions.EvaluationDirection.Diagonal, b, coordinates[0], coordinates[1]);

            //return v1 + v2 + v3 + v4 + v5 + v6 + v7 + v8 + v9 + v10;
            return (v1 * v4 * v8) + (v2 * v5 * v9) + (v3 * v6 * v10 * v11);
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
            return value;
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
            int piecesLeft = av.GetRemainingCount();
            /*foreach (Piece temp in av.RemainingPieces)
            {
                if (temp != null) { piecesLeft++; }
            }*/
            //int searchDepth = Convert.ToInt32(Math.Floor(Convert.ToDecimal(av.RemainingPieces.Length / piecesLeft)));
            int searchDepth = 2;
            foreach (Piece p in av.RemainingPieces)
            {
                if (p != null)
                {
                    double value = 0;
                    Dictionary<Piece, double> abc = RecursivePickEvaluation(b, av, searchDepth);
                    foreach(KeyValuePair<Piece, double> i in abc)
                    {
                        value += i.Value;
                    }
                    evaluationDict.Add(p, value);
                }
            }
            List<Piece> sorted = evaluationDict.OrderBy(x => x.Value).ToDictionary(pair => pair.Key).Keys.ToList();
            return sorted[0];
        }

        public Piece RandomPickPiece(Board b, AvailablePieces av)
        {
            Piece[] pieces = av.RemainingPieces;
            int count = av.GetRemainingCount();
            int rng = rnd.Next(count - 1);
            int it = 0;
            foreach(Piece p in pieces)
            {
                if (rng == it) { if (p == null) { continue; } else { return p; } }
                if (p != null) { it++; }
            }
            return null;
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
                            value += i.Value;
                        }
                        evaluationDict.Add(p, value);
                    }
                }
                return evaluationDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public string PrintInfo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (double d in Coefficients)
            {
                sb.Append(" " + Math.Round(d, 4));
            }
            return sb.ToString();
        }

        public string PrintProgress(int currentGeneration)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(currentGeneration);
            sb.Append("," + GamesPlayed);
            sb.Append("," + Wins);
            sb.Append("," + Losses);
            sb.Append("," + TotalMoves);
            foreach (double d in Coefficients)
            {
                sb.Append("," + Math.Round(d, 4));
            }
            return sb.ToString();
        }

        public void LoadFromProgress(string progress)
        {
            string[] values = progress.Split(',');
            GamesPlayed = Convert.ToInt32(values[1]);
            Wins = Convert.ToInt32(values[2]);
            Losses = Convert.ToInt32(values[3]);
            TotalMoves = Convert.ToInt32(values[4]);
            for (int i = 5; i < values.Length; i++)
            {
                Coefficients[i - 5] = Convert.ToDouble(values[i]);
            }
        }

        public static DNA Crossover(DNA dna1, DNA dna2)
        {
            int crossoverPoint = rnd.Next(CrossoverMinimum, CrossoverMaximum);
            DNA child = new DNA();
            for (int i = 0; i <= crossoverPoint; i++)
            {
                child.Coefficients[i] = dna1.Coefficients[i];
            }
            for (int i = crossoverPoint+1; i < dna1.Coefficients.Length; i++)
            {
                child.Coefficients[i] = dna2.Coefficients[i];
            }
            return child;
        }

        public static DNA NewCrossover(DNA dna1, DNA dna2)
        {
            double[] new_values = new double[10];

            double choose_parent = rnd.NextDouble();
            if (choose_parent < 0.5)
            {
                new_values[0] = dna1.Coefficients[0];
                new_values[1] = dna1.Coefficients[1];
                new_values[2] = dna1.Coefficients[2];
            }
            else
            {
                new_values[0] = dna2.Coefficients[0];
                new_values[1] = dna2.Coefficients[1];
                new_values[2] = dna2.Coefficients[2];
            }

            choose_parent = rnd.NextDouble();
            if (choose_parent < 0.5)
            {
                new_values[3] = dna1.Coefficients[3];
                new_values[4] = dna1.Coefficients[4];
                new_values[5] = dna1.Coefficients[5];
            }
            else
            {
                new_values[3] = dna2.Coefficients[3];
                new_values[4] = dna2.Coefficients[4];
                new_values[5] = dna2.Coefficients[5];
            }

            choose_parent = rnd.NextDouble();
            if (choose_parent < 0.5)
            {
                new_values[7] = dna1.Coefficients[7];
                new_values[8] = dna1.Coefficients[8];
                new_values[9] = dna1.Coefficients[9];
            }
            else
            {
                new_values[7] = dna2.Coefficients[7];
                new_values[8] = dna2.Coefficients[8];
                new_values[9] = dna2.Coefficients[9];
            }

            choose_parent = rnd.NextDouble();
            if (choose_parent < 0.5)
            {
                new_values[6] = dna1.Coefficients[6];
            }
            else
            {
                new_values[6] = dna2.Coefficients[6];
            }

            DNA child = new DNA();
            for (int i = 0; i < new_values.Length; i++) { child.Coefficients[i] = new_values[i]; }
            return child;
        }

        public bool Mutate ()
        {
            bool ret = false;
            double probPerCoefficient = MutationProbability / Coefficients.Length;
            for (int i = 0; i < Coefficients.Length; i++)
            {
                double prob = rnd.NextDouble();
                if (prob < probPerCoefficient)
                {
                    int makenegative = rnd.Next(0, 2) * 2 - 1;
                    Coefficients[i] = makenegative * rnd.NextDouble();
                    ret = true;
                }
            }
            return ret;
        }

        public bool NewMutate()
        {
            bool mut = false;
            double prob = MutationProbability / 3;
            double val = rnd.NextDouble();
            if (val < prob)
            {
                MutateSum();
                mut = true;
            }
            val = rnd.NextDouble();
            if (val < prob)
            {
                MutateSpotsRemaining();
                mut = true;
            }
            val = rnd.NextDouble();
            if (val < prob)
            {
                MutateCommonPieces();
                mut = true;
            }
            return mut;
        }

        private void MutateSum()
        {
            int gradient_sign = rnd.Next(0, 2) * 2 - 1;
            double gradient_value = rnd.NextDouble() * (MutationGradientMax - MutationGradientMin) + MutationGradientMin;
            gradient_value *= gradient_sign;
            Coefficients[0] += gradient_value;
            Coefficients[1] += gradient_value;
            Coefficients[2] += gradient_value;
        }

        private void MutateSpotsRemaining()
        {
            int gradient_sign = rnd.Next(0, 2) * 2 - 1;
            double gradient_value = rnd.NextDouble() * (MutationGradientMax - MutationGradientMin) + MutationGradientMin;
            gradient_value *= gradient_sign;
            Coefficients[3] += gradient_value;
            Coefficients[4] += gradient_value;
            Coefficients[5] += gradient_value;
        }

        private void MutateCommonPieces()
        {
            int gradient_sign = rnd.Next(0, 2) * 2 - 1;
            double gradient_value = rnd.NextDouble() * (MutationGradientMax - MutationGradientMin) + MutationGradientMin;
            gradient_value *= gradient_sign;
            Coefficients[7] += gradient_value;
            Coefficients[8] += gradient_value;
            Coefficients[9] += gradient_value;
        }

        public void EvaluateFitness()
        {
            if (Wins == 0) { Fitness = 0; return; }
            double averageTurns = TotalMoves / Wins;
            double winPercentage = GamesPlayed / Wins;
            double turnModifier = (16 - averageTurns) / 16;
            Fitness =  winPercentage * turnModifier;
        }

        public void Save(int generation, string filePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(generation);
            foreach(double d in Coefficients)
            {
                sb.Append("," + d);
            }

            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(sb.ToString());
                sw.Flush();
            }
        }

        //source: https://stackoverflow.com/questions/273313/randomize-a-listt
        private static List<double> Shuffle(List<double> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                double value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        //[int[], piece, value]
        public object[] MinMaxPlay(Board b, Piece p, AvailablePieces av, int depth, double value, bool opponent)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            int evaluationCount = Convert.ToInt32(Math.Floor(Convert.ToDecimal(playableLocations.Count / 2)));
            Dictionary<int[], double> evaluationDict = new Dictionary<int[], double>();
            foreach (int[] coordinates in playableLocations)
            {
                evaluationDict.Add(coordinates, StaticPlayEvaluate(b, p, coordinates, av));
            }
            List<int[]> sorted = evaluationDict.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key).Keys.ToList();

            List<object[]> evaluationResults = new List<object[]>();
            for (int i = 0; i < evaluationCount; i++)
            {
                Board temp = b.Copy();
                temp.SetPiece(p, sorted[i][0], sorted[i][1]);
                if (!opponent)
                {
                    object[] eval = MinMaxPick(temp, av, depth, evaluationDict[sorted[i]], opponent);
                    evaluationResults.Add(new object[] { sorted[i], eval[0], eval[1] });
                }
                else
                {
                    object[] eval = MinMaxPick(temp, av, depth, -1 * evaluationDict[sorted[i]], opponent);
                    evaluationResults.Add(new object[] { sorted[i], eval[0], eval[1] });
                }
            }

            if (evaluationResults.Count == 0)
            {
                return new object[] { playableLocations[0], null, 0};
            }
            object[] bestPlay = evaluationResults[0];
            if (!opponent)
            {
                foreach (object[] o in evaluationResults)
                {
                    if ((double)o[2] > (double)bestPlay[2]) { bestPlay = o; }
                }
                //bestPlay = evaluationResults.OrderByDescending(x => x[1]).ToList<object[]>()[0];
            }
            else
            {
                foreach (object[] o in evaluationResults)
                {
                    if ((double)o[2] < (double)bestPlay[2]) { bestPlay = o; }
                }
                //bestPlay = evaluationResults.OrderBy(x => x[1]).ToList<object[]>()[0];
            }
            return bestPlay;
        }

        //[piece, value]
        public object[] MinMaxPick(Board b, AvailablePieces av, int depth, double value, bool opponent)
        {
            Piece p = PickPiece(b, av);
            AvailablePieces copy = av.Copy();
            value -= StaticPickEvaluate(b, p, av);
            if (depth == 0)
            {
                return new object[] { p, value };
            }
            else
            {
                copy.RemovePiece(p);
                object[] evaluation = MinMaxPlay(b, p, av, depth - 1, value, !opponent);
                return new object[] { p, evaluation[2] };
            }
        }
    }
}
