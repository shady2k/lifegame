using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace lifegame
{
    public partial class Form1 : Form
    {
        bool[,] world;
        int worldHeight;
        int worldWidth;

        int xStep;
        int yStep;

        int frameHeight;
        int frameWidth;

        int rectWidth;
        int rectHeight;

        double deathProbability;
        double birthProbability;
        double initBirthProbability;

        bool isTimerActive = false;

        private System.Timers.Timer worldTimer;
        Random r = new Random();

        public Form1()
        {
            InitializeComponent();
        }
        private void GenerateWorld()
        {
            GenerateWorld(Int32.Parse(udHeight.Value.ToString()), Int32.Parse(udWidth.Value.ToString()));
        }
        private void GenerateWorld(int worldHeight, int worldWidth)
        {
            world = new bool[worldWidth, worldHeight];
        }
        private void InitNewWorld()
        {
            deathProbability = Decimal.ToDouble(udDeathProbability.Value) / 100;
            birthProbability = Decimal.ToDouble(udBirthProbability.Value) / 100;
            initBirthProbability = Decimal.ToDouble(udInitBirthProbability.Value) / 100;
            GenerateWorld();
            ShowWorldUI();
        }
        private void CalculateWorldUI()
        {
            worldHeight = world.GetLength(1);
            worldWidth = world.GetLength(0);

            xStep = (pictureBox1.Width - 1) / worldWidth;
            yStep = (pictureBox1.Height - 1) / worldHeight;

            frameHeight = yStep * worldHeight;
            frameWidth = xStep * worldWidth;

            rectWidth = (frameWidth / worldWidth) - 3;
            rectHeight = (frameHeight / worldHeight) - 3;
        }
        private void ShowWorldUIBase()
        {
            Bitmap img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics graph = Graphics.FromImage(img))
            {
                graph.FillRectangle(Brushes.White, 0, 0, frameWidth, frameHeight);

                for (int i = 0; i <= worldWidth; i++)
                {
                    graph.DrawLine(Pens.Black, i * xStep, 0, i * xStep, frameHeight);
                }

                for (int i = 0; i <= worldHeight; i++)
                {
                    graph.DrawLine(Pens.Black, 0, i * yStep, frameWidth, i * yStep);
                }

                for (int i = 0; i < worldWidth; i++)
                {
                    for (int j = 0; j < worldHeight; j++)
                    {
                        graph.FillRectangle(Brushes.Black, i * xStep + 2, j * yStep + 2, rectWidth, rectHeight);
                        graph.FillRectangle(Brushes.White, i * xStep + 1, j * yStep + 1, rectWidth, rectHeight);
                    }
                }
            }
            pictureBox1.Image = img;
        }
        private void ShowWorldUIPartial()
        {
            try
            {
                Image img = pictureBox1.Image;
                using (Graphics graph = Graphics.FromImage(img))
                {
                    for (int i = 0; i < worldWidth; i++)
                    {
                        for (int j = 0; j < worldHeight; j++)
                        {
                            if (world[i, j])
                            {
                                graph.FillRectangle(Brushes.Black, i * xStep + 2, j * yStep + 2, rectWidth, rectHeight);
                            }
                            else
                            {
                                graph.FillRectangle(Brushes.White, i * xStep + 1, j * yStep + 1, rectWidth, rectHeight);
                            }
                        }
                    }
                }
                pictureBox1.Image = img;
            } catch
            {

            }
        }
        private void ShowWorldUI()
        {
            CalculateWorldUI();
            ShowWorldUIBase();
            ShowWorldUIPartial();
        }
        private void GenerateRandomPopulation()
        {
            int x = world.GetLength(0);
            int y = world.GetLength(1);

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    world[i, j] = (r.NextDouble() <= initBirthProbability);
                }
            }
        }
        private void GenerateEmptyPopulation()
        {
            int x = world.GetLength(0);
            int y = world.GetLength(1);

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    world[i, j] = false;
                }
            }
        }
        private void GenerateNextPopulation()
        {
            int x = world.GetLength(0);
            int y = world.GetLength(1);
            bool[,] newWorld = new bool[x, y];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (GetNeighborsCount(world, i, j) == 3)
                    {
                        if (!world[i, j]) newWorld[i, j] = true;
                    }
                }
            }

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (world[i, j])
                    {
                        int nc = GetNeighborsCount(world, i, j);
                        if (nc == 3 || nc == 2)
                        {
                            newWorld[i, j] = true;
                        }
                        else
                        {
                            newWorld[i, j] = false;
                        }
                    }
                }
            }

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    double rn;

                    rn = r.NextDouble();
                    if (rn <= birthProbability)
                    {
                        if (!world[i, j]) newWorld[i, j] = true;
                    }

                    rn = r.NextDouble();
                    if (rn <= deathProbability)
                    {
                        newWorld[i, j] = false;
                    }
                }
            }

            world = newWorld;
        }
        private bool GetCellWorldValue(bool[,] tworld, int x, int y)
        {
            if (x < 0)
            {
                x = worldWidth - 1;
            }
            if (x >= worldWidth)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = worldHeight - 1;
            }
            if (y >= worldHeight)
            {
                y = 0;
            }

            if (x >= 0 && x < worldWidth)
            {
                if(y >= 0 && y < worldHeight)
                {
                    return tworld[x, y];
                }
            }
            return false;
        }
        private int GetNeighborsCount(bool[,] tworld, int x, int y)
        {
            int res = 0;

            if (GetCellWorldValue(tworld, x - 1, y - 1)) res++;
            if (GetCellWorldValue(tworld, x, y - 1)) res++;
            if (GetCellWorldValue(tworld, x + 1, y - 1)) res++;
            if (GetCellWorldValue(tworld, x - 1, y)) res++;
            if (GetCellWorldValue(tworld, x + 1, y)) res++;
            if (GetCellWorldValue(tworld, x - 1, y + 1)) res++;
            if (GetCellWorldValue(tworld, x, y + 1)) res++;
            if (GetCellWorldValue(tworld, x + 1, y + 1)) res++;

            return res;
        }
        private void SetTimer()
        {
            worldTimer = new System.Timers.Timer(Decimal.ToDouble(udTickInterval.Value));
            worldTimer.Elapsed += OnTimedEvent;
            worldTimer.AutoReset = true;
            worldTimer.Enabled = true;
        }
        private void StopTimer()
        {
            if (worldTimer != null)
            {
                worldTimer.Stop();
            }
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            worldTimer.Stop();
            if (!isTimerActive) return;
            GenerateNextPopulation();
            ShowWorldUIPartial();
            if (worldTimer != null)
            {
                worldTimer.Start();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            GenerateRandomPopulation();
            ShowWorldUI();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitNewWorld();
        }

        private void udHeight_ValueChanged(object sender, EventArgs e)
        {
            InitNewWorld();
        }

        private void udWidth_ValueChanged(object sender, EventArgs e)
        {
            InitNewWorld();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            ShowWorldUI();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            GenerateNextPopulation();
            ShowWorldUIPartial();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Запуск")
            {
                isTimerActive = true;
                button3.Text = "Стоп";
                SetTimer();
            } else
            {
                isTimerActive = false;
                StopTimer();
                button3.Text = "Запуск";
            }
        }

        private void UdTickInterval_ValueChanged(object sender, EventArgs e)
        {
            if (worldTimer != null && worldTimer.Enabled)
            {
                StopTimer();
                SetTimer();
            }
        }

        private void UdDeathProbability_ValueChanged(object sender, EventArgs e)
        {
            deathProbability = Decimal.ToDouble(udDeathProbability.Value) / 100;
        }

        private void UdBirthProbability_ValueChanged(object sender, EventArgs e)
        {
            birthProbability = Decimal.ToDouble(udBirthProbability.Value) / 100;
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (world[e.X / xStep, e.Y / yStep])
            {
                world[e.X / xStep, e.Y / yStep] = false;
            }
            else
            {
                world[e.X / xStep, e.Y / yStep] = true;
            }
            ShowWorldUIPartial();
        }

        private void UdInitBirthProbability_ValueChanged(object sender, EventArgs e)
        {
            initBirthProbability = Decimal.ToDouble(udInitBirthProbability.Value) / 100;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            GenerateEmptyPopulation();
            ShowWorldUIPartial();
        }
    }
}
