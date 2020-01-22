using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto
{
    public class ItemPlacedEventArgs : EventArgs
    {
        public int x { get; set; }
        public int y { get; set; }
    }
}
