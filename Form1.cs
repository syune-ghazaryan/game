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

namespace Racing
{
    public partial class Form1 : Form
    {
        private Random _ran = new Random();
        private int row = 0;
        private int col = 0;
        int proportion = 40;
        int health = 0;
        private GameMatrix GM = null;
        private Object GMLocker = new Object();
        private int heroCurrentPosition;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Width = 240;
            pictureBox1.Height = 240;
            this.Height = 400;
            this.Width = 400;

            // Matrix element will be represented on UI as rectangle with proportion x proportion pixesls.
            row = pictureBox1.Width / proportion;
            col = pictureBox1.Height / proportion;
            heroCurrentPosition = 0;
            GM = new GameMatrix(row, col);
            GM.Matrix[row - 1, 0] = 1;


        }

        /*
         * This is the function to update the top line of the matrix with those values
         */
        private void AddNewAliens()
        {

            // Get random number of the columnt where the alien should be added
            Random ran = new Random();
            int column = ran.Next() % col;

            // Get random type of the alien that should be added. Value of alien is 2 or 3. type should be 2 or 3. Only 2 or 3.
            int type = 2 + ran.Next() % 2;

            GM.Matrix[0, column] = type;
        }

        private void ShiftMatrixContent()
        {
            // Iterate over the rows
            for (int i = row - 1; i >= 0; --i)
            {
                // Iterate over the columns 
                for (int j = 0; j < col; ++j)
                {
                    // Check if the value of matrix[i, j] is alian, move it down. Value of alian is 2 or 3

                    if (GM.Matrix[i, j] == 2 || GM.Matrix[i, j] == 3)
                    {
                        // Move the  current alien to the next row
                        // Moving is equal to assign to next row the current row, and current set to 0
                        if (i != row - 1)
                        {
                            // Move to next row, if row existed
                            GM.Matrix[i + 1, j] = GM.Matrix[i, j];
                        }

                        // In any case set 0 the current element.
                        GM.Matrix[i, j] = 0;
                    }
                } 
            }
        }

        private void DrawMatrixLocal()
        {
            Graphics g = pictureBox1.CreateGraphics();
            g.Clear(Color.DodgerBlue);

            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    int x = j * proportion;
                    int y = i * proportion;

                    switch (GM.Matrix[i, j])
                    {
                        case 1:
                            Pen pen = new Pen(Color.Purple);
                            g.DrawRectangle(pen, new Rectangle(x, y, proportion, proportion));
                            break;
                        case 2:
                            pen = new Pen(Color.Red);
                            g.DrawRectangle(pen, new Rectangle(x, y, proportion, proportion));
                            break;
                        case 3:
                            pen = new Pen(Color.Green);
                            g.DrawRectangle(pen, new Rectangle(x, y, proportion, proportion));
                            break;

                    }

                }
            }
        }

    
        private void CheckMatrix()
        {
            lock (GMLocker)
            {
                for (int j = 0; j < col; ++j)
                {
                    if ( GM.Matrix[row - 2, j] == 2 &&  GM.Matrix[row - 1, j] == 1)
                    {
                        
                        GM.Matrix[row - 1, j] = 1;

                        if (health > 0)
                        {
                            health -= 10;
                            pbHealth.Value -= 10;

                        }
                        else
                        {
                            GameOver();
                        }
                    }

                    if ( GM.Matrix[row - 2, j] == 3 &&  GM.Matrix[row - 1, j] == 1)
                    {
                        GM.Matrix[row - 1, j] = 1;
                        if (health < 100)
                        {
                            health += 10;
                            pbHealth.Value += 10;
                            lblPercent.Text = health.ToString();
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) // FIXME: ForThread name is not informative
        {
            pbHealth.Value = health = 100;
            lblPercent.Text = health.ToString();

            Thread shiftMatrixThread = new Thread(() => ShitMatrix());
            Thread drawMatrixThread = new Thread(() => DrawMatrix());

            // To let threads close when the main applicaiton is closing
            shiftMatrixThread.IsBackground = true;
            drawMatrixThread.IsBackground = true;

            // Start the threads
            shiftMatrixThread.Start();
            drawMatrixThread.Start();
        }

        private void ShitMatrix()
        {
            while (true)
            {
                lock (GMLocker)
                {
                    CheckMatrix();
                    ShiftMatrixContent();
                    AddNewAliens();

                    Thread.Sleep(1000);
                }
                // CHeck here the content of the temp after each loop
            }
        }

        private void DrawMatrix()
        {
            while (true)
            {
                lock (GMLocker)
                {
                    DrawMatrixLocal();
                }

                Thread.Sleep(1000);
            }
        }

        private void GameOver()
        {
            MessageBox.Show("GM");
            this.Close();
        }
        private void UpdateHeroCurrentPosition()
        {
            lock (GMLocker)
            {
                for (int i = 0; i < col; ++i)
                {
                    if (GM.Matrix[row - 1, i] == 1)
                    {
                        heroCurrentPosition = i;
                    }
                }
            }
        }


        // This function is needed to override to have ability to handle the arrows press events handling


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            UpdateHeroCurrentPosition(); // FIXME: name is not informative
            lock (GMLocker)
            {

                int shift = 0;
                shift = (keyData == Keys.Left && heroCurrentPosition != 0) ? -1 : shift;
                shift = (keyData == Keys.Right && heroCurrentPosition != col - 1) ? 1 : shift;

                GM.Matrix[row - 1, heroCurrentPosition] = 0;
                GM.Matrix[row - 1, heroCurrentPosition + shift] = 1;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }
    }
}



