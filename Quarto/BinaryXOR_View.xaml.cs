using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Quarto
{
    /// <summary>
    /// Interaction logic for BinaryXOR_View.xaml
    /// </summary>
    public partial class BinaryXOR_View : Window
    {
        Board board;
        public BinaryXOR_View(Board b)
        {
            InitializeComponent();
            this.board = b;
        }

        private void btnCalc_Click(object sender, RoutedEventArgs e)
        {
            int x = Convert.ToInt32(txt1.Text);
            int y = Convert.ToInt32(txt2.Text);

            StringBuilder sb = new StringBuilder();

            if ((bool)chkRow.IsChecked)
            {
                int[] r = EvaluationFunctions.CommonProperties(EvaluationFunctions.EvaluationDirection.Row, board, x, y);
                sb.AppendLine("Row: " + string.Join("",r));
            }

            if ((bool)chkColumn.IsChecked)
            {
                int[] r = EvaluationFunctions.CommonProperties(EvaluationFunctions.EvaluationDirection.Column, board, x, y);
                sb.AppendLine("Column: " + string.Join("", r));
            }

            if ((bool)chkDiagonal.IsChecked)
            {
                int[] r = EvaluationFunctions.CommonProperties(EvaluationFunctions.EvaluationDirection.Diagonal, board, x, y);
                sb.AppendLine("Diagonal: " + string.Join("", r));
            }

            MessageBox.Show(sb.ToString());
        }
    }
}
