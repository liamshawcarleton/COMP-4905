using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto
{
    public static class AvailablePieces
    {
        public static Piece[] RemainingPieces = new Piece[16];
        private static void Setup()
        {
            //large round hollow bw
            RemainingPieces[0] = new Piece(true, true, false, true, new Bitmap(Properties.Resources.large_round_hollow_bw));
            //large round hollow wb
            RemainingPieces[1] = new Piece(true, true, false, false, new Bitmap(Properties.Resources.large_round_hollow_wb));
            //large round solid b
            RemainingPieces[2] = new Piece(true, true, true, true, new Bitmap(Properties.Resources.large_round_solid_b));
            //large round solid w
            RemainingPieces[3] = new Piece(true, true, true, false, new Bitmap(Properties.Resources.large_round_solid_w));
            //large square hollow bw
            RemainingPieces[4] = new Piece(true, false, false, true, new Bitmap(Properties.Resources.large_square_hollow_bw));
            //large square hollow wb
            RemainingPieces[5] = new Piece(true, false, false, false, new Bitmap(Properties.Resources.large_square_hollow_wb));
            //large square solid b
            RemainingPieces[6] = new Piece(true, false, true, true, new Bitmap(Properties.Resources.large_square_solid_b));
            //large square solid w
            RemainingPieces[7] = new Piece(true, false, true, false, new Bitmap(Properties.Resources.large_square_solid_w));
            //small round hollow bw
            RemainingPieces[8] = new Piece(false, true, false, true, new Bitmap(Properties.Resources.small_round_hollow_bw));
            //small round hollow wb
            RemainingPieces[9] = new Piece(false, true, false, false, new Bitmap(Properties.Resources.small_round_hollow_wb));
            //small round solid b
            RemainingPieces[10] = new Piece(false, true, true, true, new Bitmap(Properties.Resources.small_round_solid_b));
            //small round solid w
            RemainingPieces[11] = new Piece(false, true, true, false, new Bitmap(Properties.Resources.small_round_solid_w));
            //small square hollow bw
            RemainingPieces[12] = new Piece(false, false, false, true, new Bitmap(Properties.Resources.small_square_hollow_bw));
            //small square hollow wb
            RemainingPieces[13] = new Piece(false, false, false, false, new Bitmap(Properties.Resources.small_square_hollow_wb));
            //small square solid b
            RemainingPieces[14] = new Piece(false, false, true, true, new Bitmap(Properties.Resources.small_square_solid_b));
            //small square solid w
            RemainingPieces[15] = new Piece(false, false, true, false, new Bitmap(Properties.Resources.small_square_solid_w));
        }
        static AvailablePieces()
        {
            Setup();
        }
    }
}
