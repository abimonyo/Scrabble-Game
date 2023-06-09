using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Scrabble
{
    public partial class Form1 : Form
    {
        HashSet<string> dictionary = new HashSet<string>();
        string[] lines = File.ReadAllLines("dictionary.txt");

        TcpClient tcpClient;
        BinaryFormatter formatter;
        NetworkStream stream;

        String myName, opponentName;
        Panel player1Panel, player2Panel;
        Label player2Score = new Label(), player1Score = new Label();
        int myScore=0, opponentScore=0;
        Button btnSubmit, draggedButton, targetButton, btnReset;
        bool isMyTurn = false;

        List<MoveHistory> moveHistories = new List<MoveHistory>();  // Saves the tiles moved by user to board
        List<MoveHistory> sendMoveHistory = new List<MoveHistory>();  
        List<MoveHistory> filledBoardHistory = new List<MoveHistory>();  // Saves the history of filled board
        List<BoardBackup> boardHistory = new List<BoardBackup>(); // Backup for the board tile which is occupied.
        private List<Letter> userTiles = new List<Letter>();
        private Button[,] board = new Button[15, 15];   // 15x15 game board
        private Button[,] userTilesBoard = new Button[1, 7];  // 7 tiles available to user
        private const int ButtonSize = 37;
        private const int ButtonSpacing = 4;

        Color DLColor = ColorTranslator.FromHtml("#03befc");
        Color DWColor = ColorTranslator.FromHtml("#fc8e2d");
        Color TLColor = ColorTranslator.FromHtml("#0f38bd");
        Color TWColor = ColorTranslator.FromHtml("#eb0551");
        Color TilesColor = ColorTranslator.FromHtml("#f0e054");
               
        public Form1(TcpClient client, string name)
        {
            tcpClient = client;
            stream = tcpClient.GetStream();
            formatter = new BinaryFormatter();
            myName = name;
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            SetNames();
            PlayerGUI();
            AssignTurn();
            LoadGameUI();
            LoadDictionary();
        }
        private void SetNames()
        {
            formatter.Serialize(stream, myName);
            opponentName = (string)formatter.Deserialize(stream);
        }
        private void PlayerGUI()
        {
            btnSubmit = new Button();
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

            btnReset = new Button();
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
            player1Panel = new Panel();
            player1Panel.BorderStyle = BorderStyle.FixedSingle;
            player1Panel.Left = 680;
            player1Panel.Top = 40;
            player1Panel.Width = 250;
            player1Panel.Height = 100;
            panel1.Controls.Add(player1Panel);

            Label player1 = new Label();
            player1.Text = myName;
            player1.Font = new Font(player1.Font.FontFamily, 15, FontStyle.Bold);
            player1Panel.Controls.Add(player1);

            player1Score.Text = "0";
            player1Score.Top = 25;
            player1Score.Font = new Font(player1.Font, FontStyle.Bold);
            player1Panel.Controls.Add(player1Score);

            PictureBox player1Image = new PictureBox();
            player1Image.Dock = DockStyle.Right;
            player1Image.Height = 80;
            player1Image.Width = 100;
            player1Image.SizeMode = PictureBoxSizeMode.StretchImage;
            player1Image.Image = Image.FromFile("player1.jpg");  // Replace "path_to_your_image.jpg" with the actual path to your image file
            player1Panel.Controls.Add(player1Image);

            // Panel For Player 2

            player2Panel = new Panel();
            player2Panel.BorderStyle = BorderStyle.FixedSingle;
            player2Panel.Left = 680;
            player2Panel.Top = 220;
            player2Panel.Width = 250;
            player2Panel.Height = 100;
            panel1.Controls.Add(player2Panel);


            Label player2 = new Label();
            player2.Text = opponentName;
            player2.Font = new Font(player1.Font, FontStyle.Bold);
            player2Panel.Controls.Add(player2);

            player2Score.Text = "0";
            player2Score.Top = 25;
            player2Score.Font = new Font(player1.Font, FontStyle.Bold);
            player2Panel.Controls.Add(player2Score);

            PictureBox player2Image = new PictureBox();
            player2Image.Dock = DockStyle.Right;
            player2Image.Height = 80;
            player2Image.Width = 100;
            player2Image.SizeMode = PictureBoxSizeMode.StretchImage;
            player2Image.Image = Image.FromFile("player2.jpg");  // Replace "path_to_your_image.jpg" with the actual path to your image file
            player2Panel.Controls.Add(player2Image);

            player2Score.Text = "0";
            player2Score.Top = 25;
            player2Score.Font = new Font(player1.Font, FontStyle.Bold);
            player2Panel.Controls.Add(player2Score);
        }
        private void AssignTurn()
        {

            int turn = (int)formatter.Deserialize(stream);
            if (turn == 1)
            {
                isMyTurn = false;
                player1Panel.BackColor = Color.YellowGreen;
                player2Panel.BackColor = Color.White;
            }
            else
            {

                Thread t1 = new Thread(listenOpponent);
                t1.Start();
                t1.IsBackground = true;
            }
            string jsonString = (string)formatter.Deserialize(stream);
            userTiles = JsonConvert.DeserializeObject<List<Letter>>(jsonString);
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

            //UserTiles Board
            for (int i = 0; i < 7; i++)
            {
                Button button = new Button();
                button.Top = 635;
                button.Width = ButtonSize + 7;
                button.Height = ButtonSize + 7;
                button.Left = i * (ButtonSize + 7 + ButtonSpacing) + 165;
                button.BackColor = TilesColor;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.Text = userTiles[i].Name.ToString() + " \n   " + userTiles[i].Weight.ToString();
                button.Name = i.ToString();
                userTilesBoard[0, i] = button;

                userTilesBoard[0, i].MouseDown += SourceButton_MouseDown;
                button.Font = new Font(button.Font, FontStyle.Bold);
                panel1.Controls.Add(button);
            }
        }
        private void LoadDictionary()
        {
            foreach (string word in lines)
            {
                dictionary.Add(word.ToLower());
            }
        }
        private void listenOpponent()
        {
                if (isMyTurn)
                {
                    return;
                }
                else
                {
                    btnSubmit.Enabled = false;
                    player1Panel.BackColor = Color.White;
                    player2Panel.BackColor = Color.YellowGreen;
                    string jsonString = (string)formatter.Deserialize(stream);
                    moveHistories = JsonConvert.DeserializeObject<List<MoveHistory>>(jsonString);
                    opponentScore += (int)formatter.Deserialize(stream);

                updateUI();
                    player1Panel.BackColor = Color.YellowGreen;
                    player2Panel.BackColor = Color.White;
                    btnSubmit.Enabled = true;
                    AddMovesToBoardHistory(moveHistories);
                    moveHistories.Clear();
                }
        }
        private void updateUI()
        {
            foreach (var i in moveHistories)
            {
                board[i.rowIndex, i.columnIndex].Text = i.letter.ToString();
                board[i.rowIndex, i.columnIndex].Tag = 1;
                board[i.rowIndex, i.columnIndex].ForeColor = Color.Black;
                board[i.rowIndex, i.columnIndex].BackColor = TilesColor;
            }
            isMyTurn = true;
            player2Score.Text = opponentScore.ToString();
        }
          
        private void SourceButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draggedButton = (Button)sender;
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
            if (int.Parse(targetButton.Tag.ToString()) == 0)
            {
                if (!(targetButton.Text != "DW" && targetButton.Text != "DL" && targetButton.Text != "TL" && targetButton.Text != "TW" && targetButton.Text != "★" && targetButton.Text != ""))
                {
                    Button button = (Button)sender;
                    String[] splitted = targetButton.Name.Split(',');
                    draggedButton.Text = draggedButton.Text.Split(' ')[0];
                    moveHistories.Add(new MoveHistory()
                    {
                        rowIndex = 0,
                        columnIndex = int.Parse(draggedButton.Name.ToString()),
                        letter = char.Parse(draggedButton.Text)

                    });

                    boardHistory.Add(new BoardBackup()
                    {
                        row = int.Parse(splitted[0]),
                        col = int.Parse(splitted[1]),
                        text = targetButton.Text,
                        color = targetButton.BackColor
                    });
                    string buttonText = draggedButton.Text.Split(' ')[0];
                    button.Text = buttonText;
                    button.ForeColor = Color.Black;
                    button.BackColor = TilesColor;
                    button.FlatAppearance.BorderColor = Color.Red;
                    button.FlatAppearance.BorderSize = 1;
                    RemoveButtonFromLayout(draggedButton);
                    sendMoveHistory.Add(new MoveHistory()
                    {
                        rowIndex = int.Parse(splitted[0]),
                        columnIndex = int.Parse(splitted[1]),
                        letter = char.Parse(draggedButton.Text)
                    });
                }
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
            sendMoveHistory.Clear();
        }
        bool IsWordValid(string word)
        {
            return dictionary.Contains(word.ToLower());
        }
        private string GetWordFromBoard()
        {
            StringBuilder sb = new StringBuilder();

            int ind = filledBoardHistory.Count-1;
            int rowInd = filledBoardHistory[ind].rowIndex;
            int colInd = filledBoardHistory[ind].columnIndex;
            // from bottom to top row wise
            while (board[rowInd, colInd].Text != "" && board[rowInd, colInd].Text != "DW" && board[rowInd, colInd].Text != "DL" && board[rowInd, colInd].Text != "TL" && board[rowInd, colInd].Text != "TW" && board[rowInd, colInd].Text != "★")
            {
                sb.Insert(0,board[rowInd, colInd].Text);
                rowInd--;
            }
            if (IsWordValid(sb.ToString()))
                return sb.ToString();
            sb = new StringBuilder();
            rowInd = filledBoardHistory[ind].rowIndex;
            colInd = filledBoardHistory[ind].columnIndex;
            // from top to bottom row wise
            while (board[rowInd, colInd].Text != "" && board[rowInd, colInd].Text != "DW" && board[rowInd, colInd].Text != "DL" && board[rowInd, colInd].Text != "TL" && board[rowInd, colInd].Text != "TW" && board[rowInd, colInd].Text != "★")
            {
                sb.Append(board[rowInd, colInd].Text.ToString());
                rowInd++;
            }
            if (IsWordValid(sb.ToString()))
                return sb.ToString();
            sb = new StringBuilder();
            rowInd = filledBoardHistory[ind].rowIndex;
            colInd = filledBoardHistory[ind].columnIndex;
            // from left to right col wise
            while (board[rowInd, colInd].Text != "" && board[rowInd, colInd].Text != "DW" && board[rowInd, colInd].Text != "DL" && board[rowInd, colInd].Text != "TL" && board[rowInd, colInd].Text != "TW" && board[rowInd, colInd].Text != "★")
            {
                sb.Insert(0,board[rowInd, colInd].Text);
               
                colInd--;
            }
            if (IsWordValid(sb.ToString()))
                return sb.ToString();
            sb = new StringBuilder();
            rowInd = filledBoardHistory[ind].rowIndex;
            colInd = filledBoardHistory[ind].columnIndex;
            // from right to left col wise
            while (board[rowInd, colInd].Text != "" && board[rowInd, colInd].Text != "DW" && board[rowInd, colInd].Text != "DL" && board[rowInd, colInd].Text != "TL" && board[rowInd, colInd].Text != "TW" && board[rowInd, colInd].Text != "★")
            {
                sb.Append(board[rowInd, colInd].Text);
                colInd++;
            }

            return sb.ToString();
        }
        private void btnSubmit_Click(object sender, EventArgs e)
        {

            bool flag = isGameBeginWithStar();
            if (flag)
            {
                AddMovesToBoardHistory(sendMoveHistory);
                string word = GetWordFromBoard();
                if (IsWordValid(word))
                {
                    myScore += CalculateWordWeight(word);
                    MessageBox.Show(word + " is Valid word!");

                    player1Score.Text = myScore.ToString();
                    foreach (var i in boardHistory)
                    {
                        board[i.row, i.col].Tag = 1;
                    }
                    string json = JsonConvert.SerializeObject(sendMoveHistory);

                    formatter.Serialize(stream, json);
                    formatter.Serialize(stream, myScore);
                    json = (string)formatter.Deserialize(stream);
                    List<Letter> letter;
                    letter = JsonConvert.DeserializeObject<List<Letter>>(json);

                    sendMoveHistory.Clear();
                    RefillUserTiles(letter);
                    letter.Clear();
                    AddMovesToBoardHistory(moveHistories);
                    boardHistory.Clear();
                    moveHistories.Clear();
                    isMyTurn = false;
                    Thread t2 = new Thread(listenOpponent);
                    t2.IsBackground = true;
                    t2.Start();
                }
                else
                {
                    MessageBox.Show(word + "is  Invalid word!");
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
        private void RefillUserTiles(List<Letter> letter)
        {
            foreach (var i in moveHistories)
            {
                for (int j = 0; j < userTiles.Count; j++)
                {
                    if (userTiles[j].Name == i.letter)
                    {
                        userTiles.Remove(userTiles[j]);
                    }
                }
            }
            foreach (var i in letter)
            {
                userTiles.Add(i);
            }
            for(int i = 0; i < 7; i++)
            {
                userTilesBoard[0, i].Text = userTiles[i].Name.ToString() + " \n   " + userTiles[i].Weight.ToString();
                userTilesBoard[0, i].Show();
            }
        }
        private int CalculateWordWeight(string word)
        {
            int wordWeight = 0;
            foreach (char letter in word)
            {
                foreach (var scr in userTiles)
                {
                    if (letter == scr.Name)
                        wordWeight += scr.Weight;
                }
            }
            return wordWeight;
        }
        private void AddMovesToBoardHistory(List<MoveHistory> moves)
        {
            foreach(var move in moves)
            {
                filledBoardHistory.Add(move);
            }
        }
    }
}