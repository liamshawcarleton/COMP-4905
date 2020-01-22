using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto
{
    public class ItemSelectedEventArgs: EventArgs
    {
        public Piece SelectedPiece { get; set; }
    }
}
