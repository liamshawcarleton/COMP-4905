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

        public void PlayPiece(Board b, Piece p)
        {
            List<int[]> playableLocations = b.GetOpenSpots();
        }

        public Piece PickPiece()
        {
            return null;
        }

        private void RankMoves()
        {

        }
    }
}
