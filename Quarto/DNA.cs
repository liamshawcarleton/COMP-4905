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
        public int SearchDepth = 0;

        public DNA()
        {

        }

        public DNA(string progress) { LoadFromProgress(progress); }

        //the original initialization algorithm which initialized everything randomly
        public void Generate()
        {
            for (int i = 0; i < Coefficients.Length; i++)
            {
                int makenegative = rnd.Next(0, 2) * 2 - 1;
                Coefficients[i] = makenegative * rnd.NextDouble();
            }
        }

        //the new initialize algorithm that initalized related values together
        public void NewGenerate()
        {
            Coefficients[7] = rnd.NextDouble();
            GenerateBinarySum();
            GenerateSpotsRemaining();
            GenerateCommonPieces();
        }

        //experimented with generating AI models using the line of best fit in for the dataset
        public void GenerateVariableAI(int iteration)
        {
            double bSum = 0.0085 * iteration + 0.7218;
            double cPieces = 0.011 * iteration + 0.1766;
            double sRemaining = 0.2686 * Math.Log10(iteration) - 0.3199;

            for (int i = 0; i < 3; i++) { Coefficients[i] = bSum; }
            for (int i = 3; i < 6; i++) { Coefficients[i] = cPieces; }
            for (int i = 6; i < 9; i++) { Coefficients[i] = sRemaining; }
        }

        public void BuildPredefinedAI(int difficulty)
        {
            //results obtained after training
            switch (difficulty)
            {
                case 0:
                    LoadFromProgress("39,29,15,14,147,0.5965,0.5965,0.5965,0.9266,0.9266,0.9266,0,-0.3561,-0.3561,-0.3561");
                    this.SearchDepth = 0;
                    break;
                case 1:
                    LoadFromProgress("39,29,0,21,0,0.5965,0.5965,0.5965,-0.4577,-0.4577,-0.4577,0,-0.0403,-0.0403,-0.0403");
                    this.SearchDepth = 1;
                    break;
                default:
                    LoadFromProgress("10,29,0,0,0,0.726896913,0.726896913,0.726896913,0.056102734,0.056102734,0.056102734,0,0.125947166,0.125947166,0.125947166");
                    this.SearchDepth = 2;
                    break;
            }
        }

        //initialize the binary sum chromosomes
        private void GenerateBinarySum()
        {
            double min_value = 0.3;
            double max_value = 0.9;
            double initial_value = rnd.NextDouble() * (max_value - min_value) + min_value;
            Coefficients[0] = initial_value;
            Coefficients[1] = initial_value;
            Coefficients[2] = initial_value;
        }

        //initialize the spots remaining chromosomes
        private void GenerateSpotsRemaining()
        {
            double min_value = 2;
            double max_value = 8;
            double initial_value = rnd.NextDouble() * (max_value - min_value) + min_value;
            Coefficients[3] = initial_value;
            Coefficients[4] = initial_value;
            Coefficients[5] = initial_value;
        }

        ////initialize the common pieces chromosomes
        private void GenerateCommonPieces()
        {
            double min_value = -0.3;
            double max_value = 0.3;
            double initial_value = rnd.NextDouble() * (max_value - min_value) + min_value;
            Coefficients[7] = initial_value;
            Coefficients[8] = initial_value;
            Coefficients[9] = initial_value;
        }

        //selects a location to play a piece by evaluating the board with a depth of 0 (used during training)
        public int[] PlayPiece(Board b, Piece p, AvailablePieces a)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            Dictionary<int[], double> evaluationDict = new Dictionary<int[], double>();
            foreach (int[] coordinates in playableLocations)
            {
                evaluationDict.Add(coordinates, StaticPlayEvaluate(b, p, coordinates, a));
            }

            List<int[]> sorted = evaluationDict.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key).Keys.ToList();
            return sorted[0];
        }

        //original attempt at MinMax play, no longer used
        public int[] RecursivePlayPiece(Board b, Piece p, AvailablePieces a)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            Dictionary<int[], double> staticEval = new Dictionary<int[], double>();
            List<int[]> playableSubset;
            foreach (int[] i in playableLocations)
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
            playableSubset = staticEval.OrderByDescending(x => x.Value).ToDictionary(x => x.Key).Keys.ToList().Take(Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(a.GetRemainingCount() * 1 / (4 - searchDepth + 1))))).ToList();
            Dictionary<int[], double> evaluationDict = new Dictionary<int[], double>();
            /*foreach(Piece temp in a.RemainingPieces)
            {
                if (temp != null) { piecesLeft++; }
            }*/
            foreach (int[] coordinates in playableSubset)
            {
                double value = 0;
                Dictionary<int[], double> abc = RecursivePlayEvaluation(b, p, a, searchDepth);
                foreach (KeyValuePair<int[], double> i in abc)
                {
                    value += i.Value;
                }
                evaluationDict.Add(coordinates, value);
            }

            List<int[]> sorted = evaluationDict.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key).Keys.ToList();
            if (sorted.Count == 0) { return PlayPiece(b, p, a); }
            return sorted[0];
        }

        //picks a random location to play a pieces (used for random opponent)
        public int[] RandomPlayPiece(Board b, Piece p, AvailablePieces a)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            int rng = rnd.Next(playableLocations.Count - 1);
            return playableLocations[rng];
        }

        //Evaluation Function for placing a piece
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
            int v12 = EvaluationFunctions.WinningMoveAvailable(p, b, coordinates);

            //return v1 + v2 + v3 + v4 + v5 + v6 + v7 + v8 + v9 + v10;
            return (v1 * v4 * v8) + (v2 * v5 * v9) + (v3 * v6 * v10 * v11) + v12;
            //return (v1 * v8 - v4) + (v2 * v9 - v5) + (v3 * v10 * v11 - v6);
        }

        //evaluation function for picking a piece (opposite of playing a piece)
        public double StaticPickEvaluate(Board b, Piece p, AvailablePieces a)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
            double value = 0;
            foreach (int[] i in playableLocations)
            {
                AvailablePieces temp = a.Copy();
                temp.RemovePiece(p);
                if (StaticPlayEvaluate(b, p, i, temp) > value)
                {
                    value = StaticPlayEvaluate(b, p, i, temp);
                }
                //value += StaticPlayEvaluate(b, p, i, temp);
            }
            return value;
        }

        //original attempt at MinMax evaluation, not used anymore
        public Dictionary<int[], double> RecursivePlayEvaluation(Board b, Piece p, AvailablePieces a, int depth)
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
                foreach (int[] i in playableLocations)
                {
                    Board temp = b.Copy();
                    temp.SetPiece(p, i[0], i[1]);
                    Dictionary<Piece, double> pickEvaluation = RecursivePickEvaluation(temp, a, depth - 1);
                    double value = 0;
                    foreach (KeyValuePair<Piece, double> pair in pickEvaluation)
                    {
                        value += pair.Value;
                    }
                    evaluationDict.Add(i, value);
                }
                return evaluationDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        //picks a piece without evaluation future board states (depth 0)
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

        //original attempt at MinMax pick, not used anymore
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
                    foreach (KeyValuePair<Piece, double> i in abc)
                    {
                        value += i.Value;
                    }
                    evaluationDict.Add(p, value);
                }
            }
            List<Piece> sorted = evaluationDict.OrderBy(x => x.Value).ToDictionary(pair => pair.Key).Keys.ToList();
            return sorted[0];
        }

        //randomly picks a piece (used for random opponent)
        public Piece RandomPickPiece(Board b, AvailablePieces av)
        {
            Piece[] pieces = av.RemainingPieces;
            int count = av.GetRemainingCount();
            int rng = rnd.Next(count - 1);
            int it = 0;
            foreach (Piece p in pieces)
            {
                if (rng == it) { if (p == null) { continue; } else { return p; } }
                if (p != null) { it++; }
            }
            return null;
        }

        //original MinMax pick evaluation attempt, not used anymore
        public Dictionary<Piece, double> RecursivePickEvaluation(Board b, AvailablePieces av, int depth)
        {
            if (depth == 0)
            {
                Dictionary<Piece, double> evaluationDict = new Dictionary<Piece, double>();
                foreach (Piece p in av.RemainingPieces)
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

        //prints out DNA information
        public string PrintInfo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (double d in Coefficients)
            {
                sb.Append(" " + Math.Round(d, 4));
            }
            return sb.ToString();
        }

        //prints out the individuals game statistics
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

        //builds the individual by parsing values from a csv
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

        //original crossover function (all values crossover randomly)
        public static DNA Crossover(DNA dna1, DNA dna2)
        {
            int crossoverPoint = rnd.Next(CrossoverMinimum, CrossoverMaximum);
            DNA child = new DNA();
            for (int i = 0; i <= crossoverPoint; i++)
            {
                child.Coefficients[i] = dna1.Coefficients[i];
            }
            for (int i = crossoverPoint + 1; i < dna1.Coefficients.Length; i++)
            {
                child.Coefficients[i] = dna2.Coefficients[i];
            }
            return child;
        }

        //revised crossover function which crossedover values in groups
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

        //original mutate function (randomly assign new value
        public bool Mutate()
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

        //revised mutate which mutated groups of chromosomes as opposed to individually
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

        //the function to mutate the binary sum group. Originally implemented using a gradient (commented out), now set to random for experiment
        private void MutateSum()
        {
            int gradient_sign = rnd.Next(0, 2) * 2 - 1;
            //double gradient_value = rnd.NextDouble() * (MutationGradientMax - MutationGradientMin) + MutationGradientMin;
            //gradient_value *= gradient_sign;
            double value = rnd.NextDouble() * gradient_sign;
            Coefficients[0] = value;
            Coefficients[1] = value;
            Coefficients[2] = value;
        }

        //the function to mutate the spots remaining group. Originally implemented using a gradient (commented out), now set to random for experiment
        private void MutateSpotsRemaining()
        {
            int gradient_sign = rnd.Next(0, 2) * 2 - 1;
            //double gradient_value = rnd.NextDouble() * (MutationGradientMax - MutationGradientMin) + MutationGradientMin;
            //gradient_value *= gradient_sign;
            double value = rnd.NextDouble() * gradient_sign;
            Coefficients[3] = value;
            Coefficients[4] = value;
            Coefficients[5] = value;
        }

        //the function to mutate the common pieces remaining group. Originally implemented using a gradient (commented out), now set to random for experiment
        private void MutateCommonPieces()
        {
            int gradient_sign = rnd.Next(0, 2) * 2 - 1;
            //double gradient_value = rnd.NextDouble() * (MutationGradientMax - MutationGradientMin) + MutationGradientMin;
            //gradient_value *= gradient_sign;
            double value = rnd.NextDouble() * gradient_sign;
            Coefficients[7] = value;
            Coefficients[8] = value;
            Coefficients[9] = value;
        }

        //fitness function (determinted by winrate)
        public void EvaluateFitness()
        {
            if (Wins == 0) { Fitness = 0; return; }
            double averageTurns = TotalMoves / Wins;
            double winPercentage = GamesPlayed / Wins;
            double turnModifier = (16 - averageTurns) / 16;
            Fitness = winPercentage * turnModifier;
        }

        //save this individuals dna to a file
        public void Save(int generation, string filePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(generation);
            foreach (double d in Coefficients)
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

        //MinMax play algorithm 
        //[int[], piece, value]
        public object[] MinMaxPlay(Board b, Piece p, AvailablePieces av, int depth, double value, bool opponent)
        {
            int[] useless = p.GetBinary();
            List<int[]> playableLocations = b.GetOpenSpots();
            int evaluationCount = 0;
            if (playableLocations.Count < 4) { evaluationCount = playableLocations.Count; }
            else { evaluationCount = 4; }
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
                if (temp.CheckWin() && depth == 2) { return new object[] { sorted[i], p, 1000 }; }
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
                return new object[] { playableLocations[0], null, 0 };
            }
            object[] bestPlay = evaluationResults[0];
            if (!opponent)
            {
                foreach (object[] o in evaluationResults)
                {
                    try
                    {
                        if ((double)o[2] < (double)bestPlay[2]) { bestPlay = o; }
                    }
                    catch
                    {

                    }
                }
                //bestPlay = evaluationResults.OrderByDescending(x => x[1]).ToList<object[]>()[0];
            }
            else
            {
                foreach (object[] o in evaluationResults)
                {
                    try
                    {
                        if ((double)o[2] > (double)bestPlay[2]) { bestPlay = o; }
                    }
                    catch
                    {

                    }
                }
                //bestPlay = evaluationResults.OrderBy(x => x[1]).ToList<object[]>()[0];
            }
            return bestPlay;
        }

        //MinMax pick algorithm
        //[piece, value]
        public object[] MinMaxPick(Board b, AvailablePieces av, int depth, double value, bool opponent)
        {
            Piece p = PickPiece(b, av);
            AvailablePieces copy = av.Copy();
            value -= StaticPickEvaluate(b, p, av);
            if (depth == 0)
            {
                if (value == 0) { value = 0.0; }
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
