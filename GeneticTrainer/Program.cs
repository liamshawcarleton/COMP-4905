using System;
using System.Collections.Generic;
using System.IO;
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
        public static string EvolutionFilePath = @"C:\Users\5foot\Desktop\Results\genetic_evolution.csv";
        public static string ProgressFilePath = @"C:\Users\5foot\Desktop\Results\current_progress.csv";
        public static int CurrentGeneration { get; set; } = 0;
        static int time = 0;
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            Console.WriteLine("Play Game?");
            if (Console.ReadLine().ToLower() == "y")
            {
                DNA smartAI = new DNA();
                smartAI.BuildPredefinedAI(1);
                smartAI.Wins = 0;
                smartAI.Losses = 0;
                smartAI.GamesPlayed = 0;
                DNA randomAI = new DNA();
                randomAI.BuildPredefinedAI(2);
                randomAI.Wins = 0;
                randomAI.Losses = 0;
                randomAI.GamesPlayed = 0;
                Player smartPlayer = new Player("Medium Player", ref smartAI);
                Player randomPlayer = new Player("Hard Player", ref randomAI);
                Console.WriteLine("Game Count: ");
                int gamecount = Convert.ToInt32(Console.ReadLine());
                PlayLoop(gamecount, ref smartPlayer, ref randomPlayer, false);
                Console.Write("Games Played: ");
                Console.WriteLine(gamecount);
                Console.WriteLine(smartPlayer.name + " Won: " + smartPlayer.ai.Wins + " Games");
                Console.WriteLine(randomPlayer.name + " Won: " + randomPlayer.ai.Wins + " Games");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Use Default Evolution File?");
            if (Console.ReadLine().ToLower() == "n")
            {
                Console.WriteLine("Enter Path: ");
                EvolutionFilePath = Console.ReadLine();
            }
            Console.WriteLine("Use Default Progress File?");
            if (Console.ReadLine().ToLower() == "n")
            {
                Console.WriteLine("Enter Path: ");
                ProgressFilePath = Console.ReadLine();
            }
            
            Console.WriteLine("Load Evolution Progress?");
            if (Console.ReadLine().ToLower() == "y")
            {
                ReadEvolutionInfo();
            }
            else
            {
                Console.WriteLine("Population Size: ");
                PopulationSize = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Quick Play Iteration Count: ");
                QuickIterationCount = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Slow Play Iteration Count: ");
                SlowIterationCount = Convert.ToInt32(Console.ReadLine());
                CreateEvolutionFile();
            }
            Console.WriteLine("Load Iteration Progress?");
            if (Console.ReadLine().ToLower() == "y")
            {
                ReadProgressInfo();
            }
            else
            {
                //GeneratePopulation();
                NewGeneratePopulation();
            }
            NewMainLoop();
            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        //old population generation method using the old generate function
        static void GeneratePopulation()
        {
            for (int i = 0; i< PopulationSize; i++)
            {
                DNA dna = new DNA();
                dna.Generate();
                Population.Add(dna);
            }
        }

        //new population generation method using the new generate function
        static void NewGeneratePopulation()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                DNA dna = new DNA();
                dna.NewGenerate();
                Population.Add(dna);
            }
        }

        //called if you say 'y' to play game. Plays 2 AI's against eachother (used for testing)
        static void PlayLoop(int gamecount, ref Player player1, ref Player player2, bool playRandom)
        {
            for (int i = 0; i < gamecount; i++)
            {
                if (playRandom)
                {
                    PlayAgainstQuickAI(ref player1, ref player2);
                }
                else
                {
                    PlayAgainstSmartAI(ref player1, ref player2);
                }
            }
        }

        //old main loop, obsolete
        static void MainLoop()
        {
            t.Interval = 1000;
            Console.WriteLine("Beginning Training");
            Console.WriteLine("Current Generation: " + CurrentGeneration);
            Console.WriteLine("Quick Iteration Count: " + QuickIterationCount);
            Console.WriteLine("Slow Iteration Count: " + SlowIterationCount);
            Console.WriteLine("Starting Fast Iteration");
            while(CurrentGeneration < QuickIterationCount)
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
                    if (selection.GamesPlayed >= PopulationSize - 1) { localPopulation.Remove(selection); Population.Add(selection); continue; }
                    localPopulation.Remove(selection);
                    foreach (DNA k in localPopulation)
                    {
                        DNA opponent = k;
                        QuickPlay(ref selection, ref opponent);
                    }
                    Population.Add(selection);
                    SaveProgress(localPopulation);
                }
                Console.WriteLine("Beginning Selection");
                Selection();
                Console.WriteLine("Beginning Mutation");
                Mutation();
                Console.WriteLine("Saving");
                Save(CurrentGeneration);
                t.Stop();
                int hours = (int)Math.Floor(Convert.ToDecimal(time / 3600));
                if (time < 3600) { hours = 0; }
                int minutes = (int)Math.Floor(Convert.ToDecimal((time - (hours * 3600)) / 60));
                if (time < 60) { minutes = 0; }
                int seconds = (time - (hours * 3600)) - (minutes * 60);
                Console.WriteLine("Quick Iteration Time: " + hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2"));
                CurrentGeneration++;
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
                        Player p1 = new Player("Player 1", ref selection);
                        Player p2 = new Player("Player 2", ref opponent);
                        Play(ref p1, ref p2);
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

        //new main loop, iterates over n epochs having each population member play eachother
        static void NewMainLoop()
        {
            t.Interval = 1000;
            Console.WriteLine("Beginning Training");
            Console.WriteLine("Current Generation: " + CurrentGeneration);
            Console.WriteLine("Quick Iteration Count: " + QuickIterationCount);
            Console.WriteLine("Slow Iteration Count: " + SlowIterationCount);
            Console.WriteLine("Starting Fast Iteration");
            while (CurrentGeneration < QuickIterationCount)
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
                    if (selection.GamesPlayed >= PopulationSize - 1) { localPopulation.Remove(selection); Population.Add(selection); continue; }
                    localPopulation.Remove(selection);
                    foreach (DNA k in localPopulation)
                    {
                        DNA opponent = k;
                        QuickPlay(ref selection, ref opponent);
                    }
                    Population.Add(selection);
                    SaveProgress(localPopulation);
                }
                Console.WriteLine("Beginning Selection");
                NewSelection();
                Console.WriteLine("Beginning Mutation");
                NewMutate();
                Console.WriteLine("Saving");
                Save(CurrentGeneration);
                t.Stop();
                int hours = (int)Math.Floor(Convert.ToDecimal(time / 3600));
                if (time < 3600) { hours = 0; }
                int minutes = (int)Math.Floor(Convert.ToDecimal((time - (hours * 3600)) / 60));
                if (time < 60) { minutes = 0; }
                int seconds = (time - (hours * 3600)) - (minutes * 60);
                Console.WriteLine("Quick Iteration Time: " + hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2"));
                CurrentGeneration++;
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
                        Player p1 = new Player("Player 1", ref selection);
                        Player p2 = new Player("Player 2", ref opponent);
                        Play(ref p1, ref p2);
                    }
                    Population.Add(selection);
                }
                Console.WriteLine("Beginning Selection");
                NewSelection();
                Console.WriteLine("Beginning Mutation");
                NewMutate();
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

        //Play AI against random AI
        static void PlayAgainstRandom(ref Player player1, ref Player player2)
        {
            AvailablePieces available = new AvailablePieces();
            Board board = new Board();
            int player1First = rnd.Next(0, 2);
            if (player1First == 0)
            {

                player1.active = true;
                player2.active = false;
                RandomGivePiece(ref player1, ref player2, ref board, ref available);
            }
            else
            {
                player1.active = false;
                player2.active = true;
                RandomGivePiece(ref player2, ref player1, ref board, ref available);
            }
            while (true)
            {
                if (player1.active)
                {
                    Piece give = PlayPiece(ref player1, ref board, ref available);
                    if (board.CheckWin()) { GameWon(ref player1, ref player2); break; }
                    if (available.GetRemainingCount() == 0) { GameTie(ref player1, ref player2); break; }
                    GivePiece(ref player1, ref player2, ref give);
                }
                else
                {
                    RandomPlayPiece(ref player2, ref board, ref available);
                    if (board.CheckWin()) { GameWon(ref player2, ref player1); break; }
                    if (available.GetRemainingCount() == 0) { GameTie(ref player1, ref player2); break; }
                    RandomGivePiece(ref player2, ref player1, ref board, ref available);
                }
            }
        }

        //Play AI against another AI with search depth 0 (calls quick play method instead of MinMax)
        static void PlayAgainstQuickAI(ref Player player1, ref Player player2)
        {
            AvailablePieces available = new AvailablePieces();
            Board board = new Board();
            int player1First = rnd.Next(0, 2);
            if (player1First == 0)
            {
                player1.active = true;
                player2.active = false;
                RandomGivePiece(ref player1, ref player2, ref board, ref available);
            }
            else
            {
                player1.active = false;
                player2.active = true;
                RandomGivePiece(ref player2, ref player1, ref board, ref available);
            }
            while (true)
            {
                if (player1.active)
                {
                    Piece give = PlayPiece(ref player1, ref board, ref available);
                    if (board.CheckWin()) { GameWon(ref player1, ref player2); break; }
                    if (available.GetRemainingCount() == 0) { GameTie(ref player1, ref player2); break; }
                    GivePiece(ref player1, ref player2, ref give);
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

        //Play AI against another AI with standard search depth (calls MinMax function)
        static void PlayAgainstSmartAI(ref Player player1, ref Player player2)
        {
            AvailablePieces available = new AvailablePieces();
            Board board = new Board();
            int player1First = rnd.Next(0, 2);
            if (player1First == 0)
            {
                player1.active = true;
                player2.active = false;
                RandomGivePiece(ref player1, ref player2, ref board, ref available);
            }
            else
            {
                player1.active = false;
                player2.active = true;
                RandomGivePiece(ref player2, ref player1, ref board, ref available);
            }
            while (true)
            {
                if (player1.active)
                {
                    Piece give = PlayPiece(ref player1, ref board, ref available);
                    if (board.CheckWin()) { GameWon(ref player1, ref player2); break; }
                    if (available.GetRemainingCount() == 0) { GameTie(ref player1, ref player2); break; }
                    GivePiece(ref player1, ref player2, ref give);
                }
                else
                {
                    Piece give = PlayPiece(ref player2, ref board, ref available);
                    if (board.CheckWin()) { GameWon(ref player2, ref player1); break; }
                    if (available.GetRemainingCount() == 0) { GameTie(ref player1, ref player2); break; }
                    GivePiece(ref player2, ref player1, ref give);
                }
            }
        }

        //Pit two AI models against eachother
        static void Play(ref Player player1, ref Player player2)
        {
            AvailablePieces available = new AvailablePieces();
            Board board = new Board();
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
                    if(available.GetRemainingCount() == 0) { GameTie(ref player1, ref player2); break; }
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

        //Pit to AI models against eachother with search depth 0
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

        //Tell player to play a piece on the board (MinMax)
        static Piece PlayPiece (ref Player player, ref Board board, ref AvailablePieces available)
        {
            object[] play = player.ai.MinMaxPlay(board, player.selectedPiece, available, 2, 0, false);
            int[] location = (int[])play[0];
            Piece piece_play = (Piece)play[1];
            board.SetPiece(player.selectedPiece, location[0], location[1]);
            player.Turns += 1;
            player.selectedPiece = null;
            if (piece_play != null)
            {
                available.RemovePiece(piece_play);
            }
            return piece_play;
        }

        //Tell the player to play a piece quickly on the board
        static void QuickPlayPiece(ref Player player, ref Board board, ref AvailablePieces available)
        {
            int[] location = player.ai.PlayPiece(board, player.selectedPiece, available);
            board.SetPiece(player.selectedPiece, location[0], location[1]);
            player.Turns += 1;
            player.selectedPiece = null;
        }

        //Tell the player to randomly play a piece on the board 
        static void RandomPlayPiece(ref Player player, ref Board board, ref AvailablePieces available)
        {
            int[] location = player.ai.RandomPlayPiece(board, player.selectedPiece, available);
            board.SetPiece(player.selectedPiece, location[0], location[1]);
            player.Turns += 1;
            player.selectedPiece = null;
        }

        //Tell the player to give the opposing player a piece (MinMax)
        static void GivePiece(ref Player picker, ref Player receiver, ref Piece piece)
        {
            receiver.selectedPiece = piece;
            picker.Turns += 1;
            ChangeTurns(ref picker, ref receiver);
        }

        //Tell the player to give the opposing player a piece quickly
        static void QuickGivePiece(ref Player picker, ref Player receiver, ref Board board, ref AvailablePieces available)
        {
            Piece p = picker.ai.PickPiece(board, available);
            receiver.selectedPiece = p;
            picker.Turns += 1;
            available.RemovePiece(p);
            ChangeTurns(ref picker, ref receiver);
        }

        //Tell the player to randomly give the opposing player a piece
        static void RandomGivePiece(ref Player picker, ref Player receiver, ref Board board, ref AvailablePieces available)
        {
            Piece p = picker.ai.RandomPickPiece(board, available);
            receiver.selectedPiece = p;
            picker.Turns += 1;
            available.RemovePiece(p);
            ChangeTurns(ref picker, ref receiver);
        }

        //Switch Turns
        static void ChangeTurns(ref Player player1, ref Player player2)
        {
            player1.active = !player1.active;
            player2.active = !player2.active;
        }

        //Record a win and loss on the respective players to update their statistics
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

        //Don't record any statistics for either player other than games played
        static void GameTie(ref Player winner, ref Player loser)
        {
            winner.ai.GamesPlayed += 1;
            loser.ai.GamesPlayed += 1;
        }

        //old seletion algorithm
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

        //new selection algorithm
        static void NewSelection()
        {
            foreach (DNA d in Population) { d.EvaluateFitness(); }
            List<DNA> selectionList = Population.OrderByDescending(x => x.Fitness).Take((int)Math.Floor(Convert.ToDecimal(Population.Count * 3 / 4))).ToList();
            Population.Clear();

            double fitnessSum = 0;
            foreach (DNA d in selectionList) { fitnessSum += d.Fitness; }

            while (Population.Count < PopulationSize)
            {
                double fit1 = fitnessSum * rnd.NextDouble();
                double fit2 = fitnessSum * rnd.NextDouble();
                double sum1 = 0;
                double sum2 = 0;
                int index1 = 0;
                int index2 = 0;
                while (sum1 < fit1)
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
                    DNA child = DNA.NewCrossover(selectionList[index1], selectionList[index2]);
                    Population.Add(child);
                }
            }
        }

        //old mutation algorithm
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

        //new mutation algorithm
        static void NewMutate()
        {
            foreach (DNA d in Population)
            {
                if (d.NewMutate())
                {
                    Console.WriteLine("DNA Mutated");
                }
            }
        }

        //save the progress for the overall evolution after each generation has finished (so that we don't have to run the program for days at a time)
        static void Save(int iteration)
        {
            if (string.IsNullOrEmpty(EvolutionFilePath)) { return; }
            foreach(DNA d in Population)
            {
                d.Save(iteration, EvolutionFilePath);
            }
            Console.WriteLine("Genetic Population Saved");
        }

        //save the progress for the current generation (so that we don't have to run the program for days at a time)
        //called after one player finished playing every other player
        static void SaveProgress(List<DNA> localPop)
        {
            using (StreamWriter sw = new StreamWriter(new FileStream(ProgressFilePath, FileMode.Create)))
            {
                foreach (DNA d in Population)
                {
                    sw.WriteLine(d.PrintProgress(CurrentGeneration));
                }
            }

            using (StreamWriter sw = new StreamWriter(new FileStream(ProgressFilePath, FileMode.Append)))
            {
                foreach (DNA d in localPop)
                {
                    sw.WriteLine(d.PrintProgress(CurrentGeneration));
                }
            }
            
            Console.WriteLine("Progress Saved");
        }

        //used for loading the current progress
        static void ReadEvolutionInfo()
        {
            string[] info;
            using (StreamReader sr = new StreamReader(File.OpenRead(EvolutionFilePath)))
            {
                info = sr.ReadLine().Split(',');
            }
            PopulationSize = Convert.ToInt32(info[1]);
            QuickIterationCount = Convert.ToInt32(info[3]);
            SlowIterationCount = Convert.ToInt32(info[5]);
        }

        //used for loading the current progress
        static void ReadProgressInfo()
        {
            Population.Clear();
            List<string> pop = new List<string>();
            using (StreamReader sr = new StreamReader(File.OpenRead(ProgressFilePath)))
            {
                while (!sr.EndOfStream)
                {
                    pop.Add(sr.ReadLine());
                }
            }
            CurrentGeneration = Convert.ToInt32(pop[0].Split(',')[0]);
            foreach(string s in pop)
            {
                DNA d = new DNA(s);
                Population.Add(d);
            }
        }

        //creates the file that stores the current progress
        static void CreateEvolutionFile()
        {
            using (StreamWriter sw = new StreamWriter(new FileStream(EvolutionFilePath, FileMode.Create)))
            {
                sw.Write("Population Size: ,");
                sw.Write(PopulationSize + ",");
                sw.Write("Quick Iteration Count: ,");
                sw.Write(QuickIterationCount + ",");
                sw.Write("Slow Iteration Count: ,");
                sw.Write(SlowIterationCount);
                sw.WriteLine();
                sw.Write("Generation,");
                sw.Write("RowSum,");
                sw.Write("ColumnSum,");
                sw.Write("DiagonalSum,");
                sw.Write("RowSpotsRemaining,");
                sw.Write("ColumnSpotsRemaining,");
                sw.Write("DiagonalSpotsRemaining,");
                sw.Write("RemainingCount,");
                sw.Write("RowCommonPiecesRemaining,");
                sw.Write("ColumnCommonPiecesRemaining,");
                sw.Write("DiagonalCommonPiecesRemaining");
                sw.WriteLine();
            }
        }
    }
}
