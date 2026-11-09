using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TicTacToe
{
    // This is the main class for the Tic-Tac-Toe game.
    // It inherits from Form so that it can display a Windows window.
    public class TicTacToeForm : Form
    {
        // Variables used to keep track of the game.
        private readonly Button[] squares = new Button[9];
        private readonly char[] board = new char[9];

        private readonly Label turnLabel = new Label();
        private readonly Label scoreLabel = new Label();

        private char currentPlayer = 'X';
        private bool gameOver = false;

        // These variables store the number of wins for each player.
        private int xWins = 0;
        private int oWins = 0;

        // This is where the scoreboard will be saved.
        private readonly string scoreFile = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "TicTacToe",
            "scores.txt");

        // These are the eight possible winning combinations.
        private readonly int[][] winningLines =
        {
            new[] { 0, 1, 2 },
            new[] { 3, 4, 5 },
            new[] { 6, 7, 8 },

            new[] { 0, 3, 6 },
            new[] { 1, 4, 7 },
            new[] { 2, 5, 8 },

            new[] { 0, 4, 8 },
            new[] { 2, 4, 6 }
        };

        // The constructor builds the game window.
        public TicTacToeForm()
        {
            Text = "Tic-Tac-Toe";
            Size = new Size(450, 560);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            BuildInterface();
            LoadScores();
            UpdateScoreLabel();
            UpdateTurnLabel();
        }

        // This function creates the labels, buttons, and board.
        private void BuildInterface()
        {
            // Display the game title.
            Label titleLabel = new Label();
            titleLabel.Text = "TIC-TAC-TOE";
            titleLabel.Font = new Font("Arial", 22, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(25, 15);
            titleLabel.Size = new Size(385, 45);
            Controls.Add(titleLabel);

            // Display whose turn it is.
            turnLabel.Font = new Font("Arial", 13, FontStyle.Bold);
            turnLabel.TextAlign = ContentAlignment.MiddleCenter;
            turnLabel.Location = new Point(25, 70);
            turnLabel.Size = new Size(385, 35);
            Controls.Add(turnLabel);

            // Display the scoreboard.
            scoreLabel.Font = new Font("Arial", 11);
            scoreLabel.TextAlign = ContentAlignment.MiddleCenter;
            scoreLabel.Location = new Point(25, 110);
            scoreLabel.Size = new Size(385, 30);
            Controls.Add(scoreLabel);

            // Create the 3-by-3 game board.
            TableLayoutPanel boardPanel = new TableLayoutPanel();
            boardPanel.Location = new Point(40, 155);
            boardPanel.Size = new Size(350, 350);
            boardPanel.RowCount = 3;
            boardPanel.ColumnCount = 3;
            boardPanel.CellBorderStyle =
                TableLayoutPanelCellBorderStyle.Single;

            // Give every row and column an equal amount of space.
            for (int i = 0; i < 3; i++)
            {
                boardPanel.RowStyles.Add(
                    new RowStyle(SizeType.Percent, 33.33F));

                boardPanel.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 33.33F));
            }

            // Create all nine clickable squares.
            for (int i = 0; i < 9; i++)
            {
                Button square = new Button();

                square.Dock = DockStyle.Fill;
                square.Font = new Font("Arial", 36, FontStyle.Bold);
                square.Text = "";
                square.Tag = i;

                // Save the square in the array for later use.
                squares[i] = square;

                // Connect the button to the function that handles moves.
                int squareIndex = i;
                square.Click += (sender, e) => MakeMove(squareIndex);

                // Add the square to the board.
                boardPanel.Controls.Add(square, i % 3, i / 3);
            }

            Controls.Add(boardPanel);

            // Create a button to start another round.
            Button newGameButton = new Button();
            newGameButton.Text = "New Game";
            newGameButton.Font = new Font("Arial", 12, FontStyle.Bold);
            newGameButton.Location = new Point(125, 510);
            newGameButton.Size = new Size(180, 35);
            newGameButton.Click += (sender, e) => NewGame();
            Controls.Add(newGameButton);
        }

        // This function runs whenever a player clicks a square.
        private void MakeMove(int index)
        {
            // Do not allow moves after the game has ended.
            if (gameOver)
            {
                return;
            }

            // Do not allow a player to overwrite an occupied square.
            if (board[index] != '\0')
            {
                return;
            }

            // Record the current player's move.
            board[index] = currentPlayer;
            squares[index].Text = currentPlayer.ToString();

            // Check whether the current player has won.
            if (CheckWinner())
            {
                gameOver = true;

                // Increase the winner's score.
                if (currentPlayer == 'X')
                {
                    xWins++;
                }
                else
                {
                    oWins++;
                }

                turnLabel.Text = "Player " + currentPlayer + " wins!";
                UpdateScoreLabel();
                SaveScores();

                MessageBox.Show(
                    "Congratulations! Player " + currentPlayer + " wins!",
                    "Game Over");

                return;
            }

            // If the board is full, the game is a tie.
            if (IsBoardFull())
            {
                gameOver = true;
                turnLabel.Text = "It's a tie!";

                MessageBox.Show(
                    "The game ended in a tie!",
                    "Game Over");

                return;
            }

            // Switch to the other player.
            if (currentPlayer == 'X')
            {
                currentPlayer = 'O';
            }
            else
            {
                currentPlayer = 'X';
            }

            UpdateTurnLabel();
        }

        // This function checks all eight possible winning combinations.
        private bool CheckWinner()
        {
            // Loop through each possible winning line.
            foreach (int[] line in winningLines)
            {
                int first = line[0];
                int second = line[1];
                int third = line[2];

                // All three squares must contain the same player's mark.
                if (board[first] != '\0' &&
                    board[first] == board[second] &&
                    board[second] == board[third])
                {
                    return true;
                }
            }

            // No winning combination was found.
            return false;
        }

        // This function checks whether every square is occupied.
        private bool IsBoardFull()
        {
            // Look at every square on the board.
            foreach (char square in board)
            {
                // The null character means the square is empty.
                if (square == '\0')
                {
                    return false;
                }
            }

            return true;
        }

        // This function resets the board for another round.
        private void NewGame()
        {
            // Clear the game board and all nine buttons.
            for (int i = 0; i < 9; i++)
            {
                board[i] = '\0';
                squares[i].Text = "";
                squares[i].Enabled = true;
            }

            // Player X always starts a new round.
            currentPlayer = 'X';
            gameOver = false;

            UpdateTurnLabel();
        }

        // Update the label showing whose turn it is.
        private void UpdateTurnLabel()
        {
            turnLabel.Text = "Player " + currentPlayer + "'s turn";
        }

        // Update the scoreboard displayed above the board.
        private void UpdateScoreLabel()
        {
            scoreLabel.Text =
                "Player X Wins: " + xWins +
                "     Player O Wins: " + oWins;
        }

        // Save both players' scores to a text file.
        private void SaveScores()
        {
            try
            {
                // Create the folder if it does not already exist.
                Directory.CreateDirectory(
                    Path.GetDirectoryName(scoreFile)!);

                // Write both scores to the file.
                File.WriteAllLines(
                    scoreFile,
                    new string[]
                    {
                        xWins.ToString(),
                        oWins.ToString()
                    });
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "The scores could not be saved.",
                    "File Error");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "The program does not have permission to save scores.",
                    "File Error");
            }
        }

        // Load the previous scores when the program starts.
        private void LoadScores()
        {
            try
            {
                // Only read the file if it already exists.
                if (File.Exists(scoreFile))
                {
                    string[] savedScores = File.ReadAllLines(scoreFile);

                    // Make sure the file contains both scores.
                    if (savedScores.Length >= 2)
                    {
                        int.TryParse(savedScores[0], out xWins);
                        int.TryParse(savedScores[1], out oWins);
                    }
                }
            }
            catch (IOException)
            {
                // Start with the current scores if reading fails.
                xWins = 0;
                oWins = 0;
            }
            catch (UnauthorizedAccessException)
            {
                xWins = 0;
                oWins = 0;
            }
        }
    }

    // The Program class starts the application.
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Initialize Windows Forms settings.
            ApplicationConfiguration.Initialize();

            // Open the Tic-Tac-Toe game window.
            Application.Run(new TicTacToeForm());
        }
    }
}