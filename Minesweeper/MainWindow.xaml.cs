using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int GridSize = 10; // Grid size (10x10)
        private const int MineCount = 10; // Number of mines

        private Button[,] buttons; // Button grid
        private int[,] mineField; // Minefield grid: 0 = empty, 1 = mine
        private bool[,] revealed; // Revealed grid: true = revealed, false = hidden

        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void StartNewGame()
        {
            // Initialize the grids
            buttons = new Button[GridSize, GridSize];
            mineField = new int[GridSize, GridSize];
            revealed = new bool[GridSize, GridSize];

            // Clear the GameGrid
            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();

            // Create the grid rows and columns
            for (int i = 0; i < GridSize; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Create the buttons and add them to the grid
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Button button = new Button();
                    button.Height = 40;
                    button.Width = 40;
                    button.Click += Button_Click;
                    button.Tag = new Tuple<int, int>(row, col); // Store row and column in the Tag
                    buttons[row, col] = button;
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    GameGrid.Children.Add(button);
                }
            }

            // Place mines randomly
            PlaceMines();

            // Update the status text
            StatusText.Text = "Game started! Avoid the mines.";
        }

        private void PlaceMines()
        {
            Random random = new Random();
            int minesPlaced = 0;

            while (minesPlaced < MineCount)
            {
                int row = random.Next(GridSize);
                int col = random.Next(GridSize);

                // Place a mine if the spot is empty
                if (mineField[row, col] == 0)
                {
                    mineField[row, col] = 1;
                    minesPlaced++;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var position = (Tuple<int, int>)button.Tag;
            int row = position.Item1;
            int col = position.Item2;

            // Check if the clicked cell is a mine
            if (mineField[row, col] == 1)
            {
                // Game over: reveal all mines
                RevealAllMines();
                StatusText.Text = "Game Over! You hit a mine.";
                return;
            }

            // If the cell is not a mine, reveal the cell
            RevealCell(row, col);

            // Check for win condition
            if (CheckForWin())
            {
                StatusText.Text = "Congratulations! You've won!";
            }
        }

        private void RevealCell(int row, int col)
        {
            if (row < 0 || col < 0 || row >= GridSize || col >= GridSize || revealed[row, col])
                return;

            // Reveal this cell
            revealed[row, col] = true;
            int mineCount = CountAdjacentMines(row, col);
            buttons[row, col].Content = mineCount == 0 ? "" : mineCount.ToString();
            buttons[row, col].IsEnabled = false;

            // If no adjacent mines, reveal neighboring cells
            if (mineCount == 0)
            {
                RevealCell(row - 1, col);
                RevealCell(row + 1, col);
                RevealCell(row, col - 1);
                RevealCell(row, col + 1);
                RevealCell(row - 1, col - 1);
                RevealCell(row - 1, col + 1);
                RevealCell(row + 1, col - 1);
                RevealCell(row + 1, col + 1);
            }
        }

        private int CountAdjacentMines(int row, int col)
        {
            int count = 0;

            // Check all adjacent cells
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int r = row + i;
                    int c = col + j;

                    if (r >= 0 && r < GridSize && c >= 0 && c < GridSize)
                    {
                        if (mineField[r, c] == 1)
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        private void RevealAllMines()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (mineField[row, col] == 1)
                    {
                        buttons[row, col].Content = "M";
                        buttons[row, col].Background = Brushes.Red;
                    }
                }
            }
        }

        private bool CheckForWin()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (mineField[row, col] == 0 && !revealed[row, col])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }
    }
}