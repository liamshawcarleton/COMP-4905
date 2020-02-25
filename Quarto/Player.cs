using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto
{
    public class Player
    {
        public bool active { get; set; } = false;
        public string name { get; private set; } = null;
        public Piece selectedPiece { get; set; } = null;
        public int Turns { get; set; } = 0;
        public DNA ai { get; set; } = null;
        public Player(string name, ref DNA ai)
        {
            this.name = name;
            this.Turns = 0;
            this.ai = ai;
        }
    }
}
