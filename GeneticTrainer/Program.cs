using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarto;

namespace GeneticTrainer
{
    class Program
    {
        public static List<DNA> Population = new List<DNA>();
        public static int PopulationSize { get; set; }
        public static int IterationCount { get; set; }
        public static int GamesPerIteration { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("Population Size: ");
            PopulationSize = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Iteration Count: ");
            IterationCount = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Games Per Iteration: ");
            GamesPerIteration = Convert.ToInt32(Console.ReadLine());
            GeneratePopulation();
            MainLoop();
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
            Random rnd = new Random();
            List<DNA> localPopulation = new List<DNA>(Population);
            for (int i = 0; i < IterationCount; i++)
            {
                Population.Clear();
                while(localPopulation.Count > 0)
                {
                    int index1 = rnd.Next(localPopulation.Count);
                    int index2 = rnd.Next(localPopulation.Count);
                    DNA player1 = localPopulation[index1];
                    DNA player2 = localPopulation[index2];
                    Play(ref player1, ref player2);
                    if (player1.GamesPlayed == GamesPerIteration) { Population.Add(player1); localPopulation.Remove(player1); }
                    if (player2.GamesPlayed == GamesPerIteration) { Population.Add(player2); localPopulation.Remove(player2); }
                }
            }
        }

        static void Play(ref DNA player1, ref DNA player2)
        {

        }
    }
}
