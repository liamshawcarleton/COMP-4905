using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto
{
    public class Player
    {
        public bool active { get; set; }
        public string name { get; private set; }
        public Piece selectedPiece { get; set; }
        public Player(string name)
        {
            this.name = name;
        }
    }
}
