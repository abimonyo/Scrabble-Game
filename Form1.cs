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

        List<MoveHistory> moveHistories = new List<MoveHistory>();
        List<BoardBackup> boardHistory = new List<BoardBackup>();
        private Button[,] board = new Button[15, 15];
        private Button[,] wordsLayout = new Button[1, 7];
        private const int ButtonSize = 37;
        private const int ButtonSpacing = 4;
        Color DLColor = ColorTranslator.FromHtml("#03befc");
        Color DWColor = ColorTranslator.FromHtml("#fc8e2d");
        Color TLColor = ColorTranslator.FromHtml("#0f38bd"); 
        Color TWColor = ColorTranslator.FromHtml("#eb0551");
        Color TilesColor = ColorTranslator.FromHtml("#f0e054");
        private Button draggedButton; // Stores the button being dragged
        private Button targetButton; // Stores the button being dragged
        private bool isDragging; // Indicates whether dragging is in progress


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
                dictionary.Add(word.ToLower()); // Store words in lowercase for case-insensitive matching
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
                    button.Left = col * (ButtonSize+ButtonSpacing);
                    button.Top = row * (ButtonSize + ButtonSpacing);
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.ForeColor = Color.White;
                    button.Text = "";
                    button.Font = new Font(button.Font, FontStyle.Bold);
                    button.Name =row + "," + col;
                    panel1.Controls.Add(button);
                    button.BackColor = Color.LightGray;
                    board[row, col] = button;
                    board[row, col].AllowDrop = true;
                    board[row,col].DragEnter += TargetButton_DragEnter;
                    board[row,col].DragDrop += TargetButton_DragDrop;
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
                        button.BackColor=DLColor;
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
            board[7, 7].Font = new Font(board[7, 7].Font.FontFamily, 18, FontStyle.Regular);

            //bottom buttons
            for (int i = 0; i < 7; i++)
            {

                Button button = new Button();
                button.Top = 635;
                button.Width = ButtonSize;
                button.Height = ButtonSize;
                button.Left = i * (ButtonSize + ButtonSpacing) + 165;
                button.BackColor = TilesColor;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.Text = i.ToString();
                button.Name = i.ToString();
                wordsLayout[0, i] = button;
                
               /* wordsLayout[0, i].DragEnter += SourceButton_DragEnter;
                wordsLayout[0, i].DragDrop += SourceButton_DragDrop;*/
                wordsLayout[0 ,i].MouseDown += SourceButton_MouseDown;
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
            btnSubmit.Text ="Submit";
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

            Label player1Score = new Label();
            player1Score.Text = "00";
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

            Label player2Score = new Label();
            player2Score.Text = "00";
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
                    letter = draggedButton.Text
                });
                isDragging = true;
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
        private void RemoveButtonFromLayout(Button button)
        {
            /*String[] splitted = button.Name.Split(',');
            int col = int.Parse(splitted[1]);*/
            wordsLayout[0, int.Parse(button.Name)].Hide();
           
        }
        private void btnReset_Click(object sender,EventArgs args)
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
                wordsLayout[0, move.columnIndex].Show();
            }



        }
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        bool IsWordValid(string word)
        {
            return dictionary.Contains(word.ToLower());
        }
    }
  

}
