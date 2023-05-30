using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scrabble
{
    public partial class Form1 : Form
    {
        HashSet<string> dictionary = new HashSet<string>();

        // Load the word list from the file
        string[] lines = File.ReadAllLines("dictionary.txt");

        List<MoveHistory> moveHistories = new List<MoveHistory>();  // Saves the tiles moved by user to board
        List<BoardBackup> boardHistory = new List<BoardBackup>(); // Backup for the board tile which is occupied.
        private List<char> userTiles = new List<char>(); 
        private Button[,] board = new Button[15, 15];   // 15x15 game board
        private Button[,] userTilesBoard = new Button[1, 7];  // 7 tiles available to user
        private const int ButtonSize = 37;
        private const int ButtonSpacing = 4;
        Color DLColor = ColorTranslator.FromHtml("#03befc");
        Color DWColor = ColorTranslator.FromHtml("#fc8e2d");
        Color TLColor = ColorTranslator.FromHtml("#0f38bd");
        Color TWColor = ColorTranslator.FromHtml("#eb0551");
        Color TilesColor = ColorTranslator.FromHtml("#f0e054");
        private Button draggedButton; // Stores the button being dragged
        private Button targetButton; 
        private Dictionary<char, int> letterPoints = new Dictionary<char, int>()
        {
            { 'A', 1 }, { 'B', 3 }, { 'C', 3 }, { 'D', 2 }, { 'E', 1 }, { 'F', 4 }, { 'G', 2 },
            { 'H', 4 }, { 'I', 1 }, { 'J', 8 }, { 'K', 5 }, { 'L', 1 }, { 'M', 3 }, { 'N', 1 },
            { 'O', 1 }, { 'P', 3 }, { 'Q', 10 }, { 'R', 1 }, { 'S', 1 }, { 'T', 1 }, { 'U', 1 },
            { 'V', 4 }, { 'W', 4 }, { 'X', 8 }, { 'Y', 4 }, { 'Z', 10 }
        };
        Label player2Score = new Label();
        Label player1Score = new Label();
        Random random = new Random();
        char[] availableTiles = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M' };
        int Score;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            LoadGameUI();
            foreach (string word in lines)
            {
                dictionary.Add(word.ToLower());
            }
        }

        private void LoadGameUI()
        {

            for (int row = 0; row < 15; row++)
            {
                for (int col = 0; col < 15; col++)
                {
                    Button button = new Button();
                    button.Width = ButtonSize;
                    button.Height = ButtonSize;
                    button.Left = col * (ButtonSize + ButtonSpacing);
                    button.Top = row * (ButtonSize + ButtonSpacing);
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.ForeColor = Color.White;
                    button.Text = "";
                    button.Font = new Font(button.Font, FontStyle.Bold);
                    button.Name = row + "," + col;
                    button.Tag = 0;
                    panel1.Controls.Add(button);
                    button.BackColor = Color.LightGray;
                    board[row, col] = button;
                    board[row, col].AllowDrop = true;
                    board[row, col].DragEnter += TargetButton_DragEnter;
                    board[row, col].DragDrop += TargetButton_DragDrop;
                    //board[row, col].MouseDown += TargetButton_MouseDown;


                    // Code For Printing 'TW' on the board

                    if ((row == 0 || row == 7 || row == 14) && (col == 0 || col == 7 || col == 14))
                    {
                        button.Text = "TW";
                        button.BackColor = TWColor;
                    }

                    // Code For Printing 'DL' on the board

                    if ((row == 3 || row == 11) && (col == 0 || col == 7 || col == 14))
                    {
                        button.Text = "DL";
                        button.BackColor = DLColor;
                    }
                    else if ((row == 0 || row == 7 || row == 14) && (col == 3 || col == 11))
                    {
                        button.Text = "DL";
                        button.BackColor = DLColor;

                    }
                    else if ((row == 2 || row == 6 || row == 8 || row == 12) && (col == 6 || col == 8))
                    {
                        button.Text = "DL";
                        button.BackColor = DLColor;

                    }
                    else if ((row == 6 || row == 8) && (col == 2 || col == 6 || col == 8 || col == 12))
                    {
                        button.Text = "DL";
                        button.BackColor = DLColor;
                    }
                    else if ((row == 7) && (col == 3 || col == 11))
                    {
                        button.Text = "DL";
                        button.BackColor = DLColor;
                    }

                    // Code For Printing 'TL' in the board

                    if ((row == 1 || row == 13) && (col == 1 || col == 5 || col == 9 || col == 13))
                    {
                        button.Text = "TL";
                        button.BackColor = TLColor;

                    }
                    else if ((row == 2 || row == 12) && (col == 2 || col == 12))
                    {
                        button.Text = "TL";
                        button.BackColor = TLColor;

                    }
                    else if ((row == 3 || row == 11) && (col == 3 || col == 11))
                    {
                        button.Text = "TL";
                        button.BackColor = TLColor;
                    }
                    else if ((row == 4 || row == 10) && (col == 4 || col == 10))
                    {
                        button.Text = "TL";
                        button.BackColor = TLColor;
                    }
                    else if ((row == 5 || row == 9) && (col == 1 || col == 5 || col == 9 || col == 13))
                    {
                        button.Text = "TL";
                        button.BackColor = TLColor;
                    }
                    else if ((row == 7) && (col == 7))
                    {
                        button.Text = "TL";
                        button.BackColor = TLColor;
                    }
                }
            }
            // Code For Printing 'DW' on the board

            int DW = 13;
            for (int i = 1; i < 5; i++)
            {
                board[i, i].Text = "DW";
                board[i, i].BackColor = DWColor;
                if (DW > 9)
                {
                    board[i, DW].Text = "DW";
                    board[i, DW].BackColor = DWColor;
                    board[DW, i].Text = "DW";
                    board[DW, i].BackColor = DWColor;
                    board[DW, DW].Text = "DW";
                    board[DW, DW].BackColor = DWColor;
                    DW--;
                }
            }
            board[7, 7].Text = "★";
            board[7, 7].BackColor = DWColor;
            board[7, 7].ForeColor = Color.White;
            //board[7, 7].Font = new Font(board[7, 7].Font.FontFamily, 18, FontStyle.Regular);

            //UserTiles Board
            for (int i = 0; i < 7; i++)
            {
                int index = random.Next(availableTiles.Length);
                char tile = availableTiles[index];
                userTiles.Add(tile);
                availableTiles[index] = availableTiles[availableTiles.Length - 1];
                Array.Resize(ref availableTiles, availableTiles.Length - 1);


                Button button = new Button();
                button.Top = 635;
                button.Width = ButtonSize;
                button.Height = ButtonSize;
                button.Left = i * (ButtonSize + ButtonSpacing) + 165;
                button.BackColor = TilesColor;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.Text = tile.ToString();
                button.Name = i.ToString();
                userTilesBoard[0, i] = button;

                /*userTilesBoard[0, i].DragEnter += SourceButton_DragEnter;
                userTilesBoard[0, i].DragDrop += SourceButton_DragDrop;*/
                userTilesBoard[0, i].MouseDown += SourceButton_MouseDown;
                button.Font = new Font(button.Font, FontStyle.Bold);
                panel1.Controls.Add(button);
            }
            Button btnSubmit = new Button();
            btnSubmit.Top = 635;
            btnSubmit.Width = 110;
            btnSubmit.Height = 37;
            btnSubmit.Left = 500;
            Color greenColor = ColorTranslator.FromHtml("#5aad5c");
            btnSubmit.BackColor = greenColor;
            btnSubmit.ForeColor = Color.White;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Text = "Submit";
            btnSubmit.Font = new Font(btnSubmit.Font, FontStyle.Bold);
            btnSubmit.Click += btnSubmit_Click;

            panel1.Controls.Add(btnSubmit);

            Button btnReset = new Button();
            btnReset.Top = 635;
            btnReset.Width = 110;
            btnReset.Height = 37;
            btnReset.Left = 15;
            Color redColor = ColorTranslator.FromHtml("#e03d2b");
            btnReset.BackColor = redColor;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Text = "↓↓";
            btnReset.Font = new Font(btnReset.Font.FontFamily, 15, FontStyle.Regular);
            btnReset.ForeColor = Color.White;
            btnReset.Font = new Font(btnReset.Font, FontStyle.Bold);
            btnReset.Click += btnReset_Click;
            panel1.Controls.Add(btnReset);

            // Panel For Player 1
            Panel player1Panel = new Panel();
            player1Panel.BorderStyle = BorderStyle.FixedSingle;
            player1Panel.Left = 680;
            player1Panel.Top = 40;
            player1Panel.Width = 250;
            player1Panel.Height = 100;
            panel1.Controls.Add(player1Panel);

            Label player1 = new Label();
            player1.Text = "Abdullah";
            player1.Font = new Font(btnReset.Font, FontStyle.Bold);
            player1Panel.Controls.Add(player1);

            player1Score.Text = "0";
            player1Score.Top = 25;
            player1Score.Font = new Font(btnReset.Font, FontStyle.Bold);
            player1Panel.Controls.Add(player1Score);

            PictureBox player1Image = new PictureBox();
            player1Image.Dock = DockStyle.Right;
            player1Image.Height = 80;
            player1Image.Width = 100;
            player1Image.SizeMode = PictureBoxSizeMode.StretchImage;
            player1Image.Image = Image.FromFile("D:\\BSCS\\Semester 6\\Parallel Distributing Computing\\Labs\\Sir Umer\\Scrabbleproject\\player1.jpg");  // Replace "path_to_your_image.jpg" with the actual path to your image file
            player1Panel.Controls.Add(player1Image);

            // Panel For Player 2

            Panel player2Panel = new Panel();
            player2Panel.BorderStyle = BorderStyle.FixedSingle;
            player2Panel.Left = 680;
            player2Panel.Top = 220;
            player2Panel.Width = 250;
            player2Panel.Height = 100;
            panel1.Controls.Add(player2Panel);


            Label player2 = new Label();
            player2.Text = "Usama";
            player2.Font = new Font(btnReset.Font, FontStyle.Bold);
            player2Panel.Controls.Add(player2);

            player2Score.Text = "0";
            player2Score.Top = 25;
            player2Score.Font = new Font(btnReset.Font, FontStyle.Bold);
            player2Panel.Controls.Add(player2Score);

            PictureBox player2Image = new PictureBox();
            player2Image.Dock = DockStyle.Right;
            player2Image.Height = 80;
            player2Image.Width = 100;
            player2Image.SizeMode = PictureBoxSizeMode.StretchImage;
            player2Image.Image = Image.FromFile("D:\\BSCS\\Semester 6\\Parallel Distributing Computing\\Labs\\Sir Umer\\Scrabbleproject\\player2.jpg");  // Replace "path_to_your_image.jpg" with the actual path to your image file
            player2Panel.Controls.Add(player2Image);

            player2Score.Text = "00";
            player2Score.Top = 25;
            player2Score.Font = new Font(btnReset.Font, FontStyle.Bold);
            player2Panel.Controls.Add(player2Score);
        }



        private void SourceButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draggedButton = (Button)sender;
                moveHistories.Add(new MoveHistory()
                {
                    rowIndex = 0,
                    columnIndex = int.Parse(draggedButton.Name),
                    letter = char.Parse(draggedButton.Text)
                });
                draggedButton.DoDragDrop(draggedButton.Text, DragDropEffects.Copy);
            }
        }
        private void TargetButton_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                targetButton = (Button)sender;

                e.Effect = DragDropEffects.Copy;
            }
        }

        private void TargetButton_DragDrop(object sender, DragEventArgs e)
        {
            if (int.Parse(targetButton.Tag.ToString()) == 0) { 
                Button button = (Button)sender;
                String[] splitted = targetButton.Name.Split(',');
                boardHistory.Add(new BoardBackup()
                {
                    row = int.Parse(splitted[0]),
                    col = int.Parse(splitted[1]),
                    text = targetButton.Text,
                    color = targetButton.BackColor
                });
                string buttonText = (string)e.Data.GetData(DataFormats.Text);
                button.Text = buttonText;
                button.ForeColor = Color.Black;
                button.BackColor = TilesColor;

                RemoveButtonFromLayout(draggedButton);
            }
        }
        private void RemoveButtonFromLayout(Button button)
        {
            userTilesBoard[0, int.Parse(button.Name)].Hide();
        }
        private void btnReset_Click(object sender, EventArgs args)
        {
            foreach (var i in boardHistory)
            {
                board[i.row, i.col].Text = i.text;
                board[i.row, i.col].BackColor = i.color;
                board[i.row, i.col].ForeColor = Color.White;
                board[i.row, i.col].Show();

            }
            foreach (var move in moveHistories)
            {
                userTilesBoard[0, move.columnIndex].Show();
            }
            boardHistory.Clear();
            moveHistories.Clear();
        }
        bool IsWordValid(string word)
        {
            return dictionary.Contains(word.ToLower());
        }

        private string GetWordFromBoard()
        {
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < 15; row++)
            {
                for (int col = 0; col < 15; col++)
                {
                    if (!string.IsNullOrEmpty(board[row, col].Text))
                    {
                        if (board[row, col].Text != "DW" && board[row, col].Text != "DL" && board[row, col].Text != "TL" && board[row, col].Text != "TW" && board[row, col].Text != "★")
                            if(int.Parse(board[row, col].Tag.ToString())==0)
                                sb.Append(board[row, col].Text);
                    }
                }
            }
            return sb.ToString();
        }
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            bool flag=isGameBeginWithStar();
            if (flag)
            { 
                string word = GetWordFromBoard();
                if (IsWordValid(word))
                {

                    Score += CalculateWordWeight(word);
                    MessageBox.Show("Valid word!");
                
                    player2Score.Text = Score.ToString();
                    foreach(var i in boardHistory)
                    {

                        board[i.row, i.col].Tag = 1;
                    }

                    RefillUserTiles();
                    boardHistory.Clear();
                    moveHistories.Clear();
                }
                else
                {
                    MessageBox.Show("Invalid word!");
                }
            }
            else 
            {
                MessageBox.Show("First Word Must Start with Star");

            }
        }

        private bool isGameBeginWithStar()
        {
            if (board[7, 7].Text == "★")
            {
                return false;
            }
            else
                return true;
        }

        private void RefillUserTiles()
        {
            foreach(var i in moveHistories)
            {
                userTiles.Remove(i.letter);
            }
            for(int i = 0; i <= 7-userTiles.Count; i++)
            {
                userTiles.Add(availableTiles[random.Next(availableTiles.Length)]);
            }
            foreach(var i in moveHistories)
            {
                foreach(var j in userTiles)
                {
                    userTilesBoard[0, i.columnIndex].Text=j.ToString();
                    userTilesBoard[0, i.columnIndex].Show();

                }
            }
        }

        private int CalculateWordWeight(string word)
        {
            int wordWeight = 0;
            foreach (char letter in word)
            {
                int letterWeight = letterPoints[letter];
                wordWeight += letterWeight;
            }
            return wordWeight;
        }
    }
}