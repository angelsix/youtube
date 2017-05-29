using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication1
{
    public enum MarkType
    {
        Free,
        Nought,
        Cross
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MarkType[] mResults;
        private bool mPlayer1Turn;
        private bool mWinner;

        public MainWindow()
        {
            InitializeComponent();

            NewGame();
        }

        private void NewGame()
        {
            mResults = new MarkType[9];

            for (var i = 0; i < mResults.Length; i++)
                mResults[i] = MarkType.Free;

            mPlayer1Turn = true;

            Container.Children.Cast<Button>().ToList().ForEach(f =>
            {
                f.Content = string.Empty;
                f.Background = Brushes.White;
                f.Foreground = Brushes.Blue;
            });

            mWinner = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (mWinner)
            {
                NewGame();
                return;
            }

            var button = (Button)sender;

            var row = Grid.GetRow(button);
            var column = Grid.GetColumn(button);

            var index = column + (row * 3);

            if (mResults[index] != MarkType.Free)
                return;

            mResults[index] = mPlayer1Turn ? MarkType.Cross : MarkType.Nought;

            button.Content = mPlayer1Turn ? "X" : "O";

            if (!mPlayer1Turn)
                button.Foreground = Brushes.Red;


            mPlayer1Turn ^= true;

            CheckForWinner();
        }

        private void CheckForWinner()
        {
            if (mResults[0] != MarkType.Free && (mResults[0] & mResults[1] & mResults[2]) == mResults[0])
            {
                mWinner = true;

                // Turn the winning blocks green
                Button0_0.Background = Button1_0.Background = Button2_0.Background = Brushes.Green;
            }
            if (mResults[3] != MarkType.Free && (mResults[3] & mResults[4] & mResults[5]) == mResults[3])
            {
                mWinner = true;

                // Turn the winning blocks green
                Button0_1.Background = Button1_1.Background = Button2_1.Background = Brushes.Green;
            }
            if (mResults[6] != MarkType.Free && (mResults[6] & mResults[7] & mResults[8]) == mResults[6])
            {
                mWinner = true;

                // Turn the winning blocks green
                Button0_2.Background = Button1_2.Background = Button2_2.Background = Brushes.Green;
            }

            if (mResults[0] != MarkType.Free && (mResults[0] & mResults[3] & mResults[6]) == mResults[0])
            {
                mWinner = true;

                // Turn the winning blocks green
                Button0_0.Background = Button0_1.Background = Button0_2.Background = Brushes.Green;
            }
            if (mResults[1] != MarkType.Free && (mResults[1] & mResults[4] & mResults[7]) == mResults[1])
            {
                mWinner = true;

                // Turn the winning blocks green
                Button1_0.Background = Button1_1.Background = Button1_2.Background = Brushes.Green;
            }
            if (mResults[2] != MarkType.Free && (mResults[2] & mResults[5] & mResults[8]) == mResults[2])
            {
                mWinner = true;

                // Turn the winning blocks green
                Button2_0.Background = Button2_1.Background = Button2_2.Background = Brushes.Green;
            }

            if (mResults[0] != MarkType.Free && (mResults[0] & mResults[4] & mResults[8]) == mResults[0])
            {
                mWinner = true;

                // Turn the winning blocks green
                Button0_0.Background = Button1_1.Background = Button2_2.Background = Brushes.Green;
            }
            if (mResults[2] != MarkType.Free && (mResults[2] & mResults[4] & mResults[6]) == mResults[2])
            {
                mWinner = true;

                // Turn the winning blocks green
                Button2_0.Background = Button1_1.Background = Button0_2.Background = Brushes.Green;
            }

            if (!mResults.Any(f => f == MarkType.Free))
            {
                Container.Children.Cast<Button>().ToList().ForEach(f => f.Background = Brushes.Orange);
                mWinner = true;
            }
        }
    }
}
