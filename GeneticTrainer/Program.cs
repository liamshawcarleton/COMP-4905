using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Quarto;

namespace GeneticTrainer
{
    class Program
    {
        static Timer t = new Timer();
        public static List<DNA> Population = new List<DNA>();
        public static int PopulationSize { get; set; } = 0;
        public static int QuickIterationCount { get; set; } = 0;
        public static int SlowIterationCount { get; set; } = 0;
        public static string FilePath = @"C:\Users\Stilts\Desktop\Results\genetic_evolution.csv";
        static int time = 0;
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            Console.WriteLine("Load From File?");
            string load = Console.ReadLine();
            if (load.ToLower() == "y")
            {
                Console.WriteLine("Enter File Path");
                FilePath = Console.ReadLine();
            }
            Console.WriteLine("Population Size: ");
            PopulationSize = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Quick Play Iteration Count: ");
            QuickIterationCount = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Slow Play Iteration Count: ");
            SlowIterationCount = Convert.ToInt32(Console.ReadLine());
            GeneratePopulation();
            MainLoop();
            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        static void GeneratePopulation()
        {
            for (int i = 0; i< PopulationSize; i++)
            {
                DNA dna = new DNA();
                dna.Generate();
                Population.Add(dna);
            }
        }

        static void MainLoop()
        {
            t.Interval = 1000;
            Console.WriteLine("Starting Fast Iteration");
            for (int i = 0; i < QuickIterationCount; i++)
            {
                time = 0;
                t.Start();
                t.Elapsed += T_Elapsed;
                Console.WriteLine("Weights");
                foreach (DNA dna in Population)
                {
                    Console.WriteLine(dna.PrintInfo());
                }

                List<DNA> localPopulation = new List<DNA>(Population);
                Population.Clear();
                while (localPopulation.Count > 0)
                {
                    DNA selection = localPopulation[0];
                    localPopulation.Remove(selection);
                    foreach (DNA k in localPopulation)
                    {
                        DNA opponent = k;
                        QuickPlay(ref selection, ref opponent);
                    }
                    Population.Add(selection);
                }
                Console.WriteLine("Beginning Selection");
                Selection();
                Console.WriteLine("Beginning Mutation");
                Mutation();
                Console.WriteLine("Saving");
                Save(i);
                t.Stop();
                int hours = (int)Math.Floor(Convert.ToDecimal(time / 3600));
                if (time < 3600) { hours = 0; }
                int minutes = (int)Math.Floor(Convert.ToDecimal((time - (hours * 3600)) / 60));
                if (time < 60) { minutes = 0; }
                int seconds = (time - (hours * 3600)) - (minutes * 60);
                Console.WriteLine("Quick Iteration Time: " + hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2"));
            }

            Console.WriteLine("Starting Slow Iteration");
            for (int i = 0; i < SlowIterationCount; i++)
            {
                time = 0;
                t.Start();
                foreach (DNA dna in Population)
                {
                    Console.WriteLine(dna.PrintInfo());
                }

                List<DNA> localPopulation = new List<DNA>(Population);
                Population.Clear();
                while (localPopulation.Count > 0)
                {
                    DNA selection = localPopulation[0];
                    localPopulation.Remove(selection);
                    foreach (DNA k in localPopulation)
                    {
                        DNA opponent = k;
                        Play(ref selection, ref opponent);
                    }
                    Population.Add(selection);
                }
                Console.WriteLine("Beginning Selection");
                Selection();
                Console.WriteLine("Beginning Mutation");
                Mutation();
                Console.WriteLine("Saving");
                Save(i);
                t.Stop();
                int hours = time % 3600;
                if (time < 3600) { hours = 0; }
                int minutes = (time - hours * 3600) % 60;
                if (time < 60) { minutes = 0; }
                int seconds = (time - hours * 3600) - minutes * 60;
                Console.WriteLine("Slow Iteration Time: " + hours + ":" + minutes + ":" + seconds);
            }
        }

        private static void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            time++;
        }

        static void Play(ref DNA p1, ref DNA p2)
        {
            AvailablePieces available = new AvailablePieces();
            Board board = new Board();
            Player player1 = new Player("Player 1", ref p1);
            Player player2 = new Player("Player 2", ref p2);
            int player1First = rnd.Next(0, 2);
            if (player1First == 0)
            {
                player1.active = true;
                player2.active = false;
                GivePiece(ref player1, ref player2, ref board, ref available);
            }
            else
            {
                player1.active = false;
                player2.active = true;
                GivePiece(ref player2, ref player1, ref board, ref available);
            }
            while (true)
            {
                if (player1.active)
                {
                    PlayPiece(ref player1, ref board, ref available);
                    if (board.CheckWin()) { GameWon(ref player1, ref player2); break; }
                    if(available.GetRemainingCount() == 0) { GameTie(ref player1, ref player2); break; }
                    GivePiece(ref player1, ref player2, ref board, ref available);
                }
                else
                {
                    PlayPiece(ref player2, ref board, ref available);
                    if (board.CheckWin()) { GameWon(ref player2, ref player1); break; }
                    if (available.GetRemainingCount() == 0) { GameTie(ref player1, ref player2); break; }
                    GivePiece(ref player2, ref player1, ref board, ref available);
                }
            }
        }

        static void QuickPlay(ref DNA p1, ref DNA p2)
        {
            AvailablePieces available = new AvailablePieces();
            Board board = new Board();
            Player player1 = new Player("Player 1", ref p1);
            Player player2 = new Player("Player 2", ref p2);
            int player1First = rnd.Next(0, 2);
            if (player1First == 0)
            {
                player1.active = true;
                player2.active = false;
                QuickGivePiece(ref player1, ref player2, ref board, ref available);
            }
            else
            {
                player1.active = false;
                player2.active = true;
                QuickGivePiece(ref player2, ref player1, ref board, ref available);
            }
            while (true)
            {
                if (player1.active)
                {
                    QuickPlayPiece(ref player1, ref board, ref available);
                    if (board.CheckWin()) { GameWon(ref player1, ref player2); break; }
                    if (available.GetRemainingCount() == 0) { GameTie(ref player1, ref player2); break; }
                    QuickGivePiece(ref player1, ref player2, ref board, ref available);
                }
                else
                {
                    QuickPlayPiece(ref player2, ref board, ref available);
                    if (board.CheckWin()) { GameWon(ref player2, ref player1); break; }
                    if (available.GetRemainingCount() == 0) { GameTie(ref player1, ref player2); break; }
                    QuickGivePiece(ref player2, ref player1, ref board, ref available);
                }
            }
        }

        static void PlayPiece (ref Player player, ref Board board, ref AvailablePieces available)
        {
            int[] location = player.ai.RecursivePlayPiece(board, player.selectedPiece, available);
            board.SetPiece(player.selectedPiece, location[0], location[1]);
            player.Turns += 1;
            player.selectedPiece = null;
        }

        static void QuickPlayPiece(ref Player player, ref Board board, ref AvailablePieces available)
        {
            int[] location = player.ai.PlayPiece(board, player.selectedPiece, available);
            board.SetPiece(player.selectedPiece, location[0], location[1]);
            player.Turns += 1;
            player.selectedPiece = null;
        }

        static void GivePiece(ref Player picker, ref Player receiver, ref Board board, ref AvailablePieces available)
        {
            Piece p = picker.ai.RecursivePickPiece(board, available);
            receiver.selectedPiece = p;
            picker.Turns += 1;
            available.RemovePiece(p);
            ChangeTurns(ref picker, ref receiver);
        }

        static void QuickGivePiece(ref Player picker, ref Player receiver, ref Board board, ref AvailablePieces available)
        {
            Piece p = picker.ai.PickPiece(board, available);
            receiver.selectedPiece = p;
            picker.Turns += 1;
            available.RemovePiece(p);
            ChangeTurns(ref picker, ref receiver);
        }

        static void ChangeTurns(ref Player player1, ref Player player2)
        {
            player1.active = !player1.active;
            player2.active = !player2.active;
        }

        static void GameWon(ref Player winner, ref Player loser)
        {
            winner.ai.GamesPlayed += 1;
            loser.ai.GamesPlayed += 1;

            winner.ai.Wins += 1;
            winner.ai.TotalMoves += winner.Turns;
            winner.Turns = 0;
            winner.selectedPiece = null;
            Console.WriteLine(winner.name + " Won");

            loser.ai.Losses += 1;
            loser.Turns = 0;
            loser.selectedPiece = null;
        }

        static void GameTie(ref Player winner, ref Player loser)
        {
            winner.ai.GamesPlayed += 1;
            loser.ai.GamesPlayed += 1;
        }

        static void Selection()
        {
            foreach (DNA d in Population){ d.EvaluateFitness(); }
            List<DNA> selectionList = Population.OrderByDescending(x => x.Fitness).Take((int)Math.Floor(Convert.ToDecimal(Population.Count * 3/4))).ToList();
            Population.Clear();

            double fitnessSum = 0;
            foreach (DNA d in selectionList){ fitnessSum += d.Fitness; }

            while(Population.Count < PopulationSize)
            {
                double fit1 = fitnessSum * rnd.NextDouble();
                double fit2 = fitnessSum * rnd.NextDouble();
                double sum1 = 0;
                double sum2 = 0;
                int index1 = 0;
                int index2 = 0;
                while(sum1 < fit1)
                {
                    sum1 += selectionList[index1].Fitness;
                    if (sum1 < fit1) { index1++; }
                }
                while (sum2 < fit2)
                {
                    sum2 += selectionList[index2].Fitness;
                    if (sum2 < fit2) { index2++; }
                }
                if (index1 == index2) { continue; }
                else
                {
                    DNA child = DNA.Crossover(selectionList[index1], selectionList[index2]);
                    Population.Add(child);
                }
            }
        }

        static void Mutation()
        {
            foreach(DNA d in Population)
            {
                if (d.Mutate())
                {
                    Console.WriteLine("DNA Mutated");
                }
            }
        }

        static void Save(int iteration)
        {
            if (string.IsNullOrEmpty(FilePath)) { return; }
            foreach(DNA d in Population)
            {
                d.Save(iteration, FilePath);
            }
        }
    }
}
