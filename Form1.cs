using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scrabble
{
    public partial class Form1 : Form
    {
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
        private bool isDragging; // Indicates whether dragging is in progress


        public Form1()
        {
            InitializeComponent();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

            LoadGameUI();
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
                    button.Name = "Button_" + row + "_" + col;
                    panel1.Controls.Add(button);
                    button.BackColor = Color.LightGray;
                    board[row, col] = button;
                    board[row, col].AllowDrop = true;
                    board[row,col].DragEnter += TargetButton_DragEnter;
                    board[row,col].DragDrop += TargetButton_DragDrop;
                    

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
                wordsLayout[0, i] = button;
                wordsLayout[0 ,i].MouseDown += SourceButton_MouseDown;
             

                panel1.Controls.Add(button);
                

            }
            Button btnSubmit = new Button();
            btnSubmit.Top = 635;
            btnSubmit.Width = 110;
            btnSubmit.Height = 37;
            btnSubmit.Left = 500;
            btnSubmit.BackColor = Color.LightGreen;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Text ="Submit";
            panel1.Controls.Add(btnSubmit);

            Button btnReset = new Button();
            btnReset.Top = 635;
            btnReset.Width = 110;
            btnReset.Height = 37;
            btnReset.Left = 15;
            btnReset.BackColor = Color.LightGreen;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Text = "Reset";
            panel1.Controls.Add(btnReset);

        }

       

        private void SourceButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draggedButton = (Button)sender;
                isDragging = true;
                draggedButton.DoDragDrop(draggedButton.Text, DragDropEffects.Copy);
            }
        }
        private void TargetButton_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void TargetButton_DragDrop(object sender, DragEventArgs e)
        {
            Button targetButton = (Button)sender;
            string buttonText = (string)e.Data.GetData(DataFormats.Text);
            targetButton.Text = buttonText;
            RemoveButtonFromLayout(draggedButton);       
        }
        private void RemoveButtonFromLayout(Button button)
        {
            button.Parent.Controls.Remove(button);
        }
    }
  

}
