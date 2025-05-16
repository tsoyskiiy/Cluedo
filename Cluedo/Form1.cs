using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Cluedo
{
    public partial class Form1 : Form
    {
        notepad notePad;
        private Dictionary<PictureBox, int> originalTops = new Dictionary<PictureBox, int>();
        private const int cellSize = 22;
        private const int gridWidth = 24;
        private const int gridHeight = 25;
        private int remainingSteps = 0;
        private bool canMove = true;
        private Timer moveTimer;
        

        private int[,] map;
        private List<Player> players = new List<Player>();
        private int currentPlayerIndex = 0;
        private Random random = new Random();
        private int numberOfPlayers;
        private Dictionary<int, notepad> playerNotepads = new Dictionary<int, notepad>();
        private Dictionary<int, Dictionary<string, bool>> notepadStates = new Dictionary<int, Dictionary<string, bool>>();
        public Form1(int playerCount)
        {
            numberOfPlayers = playerCount;
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
           
            moveTimer = new Timer();
            moveTimer.Interval = 150;
            moveTimer.Tick += (s, e) =>
            {
                canMove = true;
                moveTimer.Stop();
            };
          

            InitializeMap();
            InitializePlayer();
            SetupUI();
            roomsPic.PreviewKeyDown += RoomsPic_PreviewKeyDown;
            roomsPic.TabStop = true; 
            roomsPic.Focus();
            RegisterCharacterIcon(pictureRose);
            RegisterCharacterIcon(pictureBrown);
            RegisterCharacterIcon(pictureGrey);
            RegisterCharacterIcon(pictureMurphy);
            RegisterCharacterIcon(pictureTaylor);
            RegisterCharacterIcon(pictureKnife);
            RegisterCharacterIcon(picturePipe);
            RegisterCharacterIcon(pictureRevolver);
            RegisterCharacterIcon(pictureRope);
            RegisterCharacterIcon(pictureWrench);
            RegisterCharacterIcon(pictureJones);
            RegisterCharacterIcon(pictureCandlestick);



        }

        public class Player
        {
            public int X, Y;
            public PictureBox Pic;
            public string Name;
        }
        private void PictureBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
        e.KeyCode == Keys.Left || e.KeyCode == Keys.Right ||
        e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }
        private void RegisterCharacterIcon(PictureBox pb)
        {
            originalTops[pb] = pb.Top;
            pb.PreviewKeyDown += PictureBox_PreviewKeyDown;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.MouseEnter += PictureBox_MouseEnter;
            pb.MouseLeave += PictureBox_MouseLeave;
            pb.Cursor = Cursors.Hand;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Top -= 5;
        }
        private void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Top = originalTops[pb];
        }

        private void AddPlayer(string name, Image img, int startX, int startY)
        {
            PictureBox pic = new PictureBox
            {
                Image = img,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(cellSize, cellSize),
                BackColor = Color.Transparent,
                Parent = roomsPic
            };
            roomsPic.Controls.Add(pic);
            pic.BringToFront();
            Player p = new Player
            {
                Name = name,
                X = startX,
                Y = startY,
                Pic = pic
            };
            players.Add(p);
            UpdatePlayerPosition(p);

        }
        private void UpdatePlayerPosition(Player p)
        {
            int offset = (cellSize - p.Pic.Width) / 2;
            p.Pic.Left = p.X * cellSize + offset;
            p.Pic.Top = p.Y * cellSize + offset;
        }

        private void InitializePlayer()
        {
            List<(string name, PictureBox pic, int x, int y)> allPlayers = new List<(string, PictureBox, int, int)>
    {
               ("Mille Rose", playerPic1, 10, 25),
               ("Mr.Murphy", playerPic2, 15, 25),
               ("Miss Grey", playerPic3, 1, 19),
               ("Mr.Brown", playerPic4, 24, 8),
               ("Mille Taylor", playerPic5, 1, 6),
               ("Dr.Jones", playerPic6, 17, 1)
    };
            for(int i = 0; i < numberOfPlayers; i++)
            {
                var (name, pic, x, y) = allPlayers[i];
                players.Add(new Player { Name = name, Pic = pic, X = x, Y = y });
            }
            for(int i = 0; i < numberOfPlayers; i++)
            {
                playerNotepads[i] = new notepad($"Player{i + 1}");
                notepadStates[i] = new Dictionary<string, bool>();
            }
            foreach (var p in players)
            {
                p.Pic.Parent = roomsPic;
                p.Pic.Size = new Size(16, 16);
                p.Pic.SizeMode = PictureBoxSizeMode.Zoom;
                p.Pic.BackColor = Color.Transparent;
                roomsPic.Controls.Add(p.Pic);
                p.Pic.BringToFront();
                UpdatePlayerPosition(p);
            }
            List<PictureBox> allPics = new List<PictureBox> { playerPic1, playerPic2, playerPic3, playerPic4, playerPic5, playerPic6 };

            for (int i = numberOfPlayers; i < allPics.Count; i++)
            {
                allPics[i].Visible = false;
            }

        }
        private void InitializeMap()
        {
            //empty = 0, wall = 1, door = 2, player = 3, room = 4
            map = new int[,] {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,4,4,4,4,4,4,4,0,1,1,4,4,4,4,1,1,1,1,4,4,4,4,1},
        {1,4,4,4,4,4,4,4,0,0,1,4,4,4,4,1,0,0,1,4,4,4,4,1},
        {1,4,4,4,4,4,4,4,0,0,1,4,4,4,4,1,0,0,1,4,4,4,4,1},
        {1,1,1,1,1,1,1,2,0,0,1,4,4,4,4,1,0,0,1,4,4,4,4,1},
        {1,1,0,0,0,0,0,0,0,0,2,4,4,4,4,1,0,0,1,4,4,4,4,1},
        {1,1,0,0,0,0,0,0,0,0,1,4,4,4,4,1,0,0,2,1,1,1,1,1},
        {1,1,1,1,1,1,1,0,0,0,1,1,2,2,1,1,0,0,0,0,0,0,0,0},
        {1,4,4,4,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {1,4,4,4,1,1,1,2,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0},
        {1,4,4,4,1,1,1,1,0,0,1,1,1,1,1,0,0,1,2,1,1,1,1,1},
        {1,1,1,1,2,1,1,0,0,0,1,1,1,1,1,0,0,1,4,4,4,4,4,4},
        {1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,1,4,4,4,4,4,4},
        {1,1,2,1,1,1,1,0,0,0,0,0,0,0,0,0,0,2,4,4,4,4,4,4},
        {1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,0,0,1,4,4,4,4,4,4},
        {1,4,4,4,1,1,1,0,0,0,1,1,1,1,1,0,0,1,1,1,1,1,1,1},
        {1,4,4,4,1,1,2,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1},
        {1,4,4,4,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,1,2,1,1,1,1,2,1,0,0,0,0,0,0,0},
        {1,0,0,0,0,0,0,0,0,1,4,4,4,4,4,4,1,0,0,1,2,1,1,1},
        {1,1,1,1,1,2,0,0,0,2,4,4,4,4,4,4,2,0,0,1,1,1,1,1},
        {1,4,4,4,1,1,1,0,0,1,4,4,4,4,4,4,1,0,0,1,1,1,1,1},
        {1,4,4,4,1,1,1,0,0,1,4,4,4,4,1,1,1,0,0,1,1,1,1,1},
        {1,4,4,4,1,1,1,0,0,1,4,1,4,4,1,1,1,0,0,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,0,0,0,1,1,1,1,1,1}
            };




        }


        private void RoomsPic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.IsInputKey = true;
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!canMove || remainingSteps <= 0) return;
            canMove = false;
            moveTimer.Start();

            var current = players[currentPlayerIndex];
            int dx = 0, dy = 0;
            if (e.KeyCode == Keys.Up) dy = -1;
            else if (e.KeyCode == Keys.Down) dy = 1;
            else if (e.KeyCode == Keys.Left) dx = -1;
            else if (e.KeyCode == Keys.Right) dx = 1;
            else return;

            int newX = current.X + dx;
            int newY = current.Y + dy;

            if (newX < 0 || newX >= gridWidth || newY < 0 || newY >= gridHeight)
                return;

            if (map[newY, newX] == 0 || map[newY, newX] == 2)
            {
                current.X = newX;
                current.Y = newY;
                UpdatePlayerPosition(current);
                remainingSteps--;
            }
        }
        private void SetupUI()
        {
            Button btnRoll = new Button
            {
                Text = "Roll",
                Location = new Point(10, gridHeight * cellSize + 10)
            };
            btnRoll.Click += btnRoll_Click;
            this.Controls.Add(btnRoll);

            Button btnNext = new Button
            {
                Text = "Next",
                Location = new Point(90, gridHeight * cellSize + 10)
            };
            btnNext.Click += btnNext_Click;
            this.Controls.Add(btnNext);
        }
            private void playerPic3_Click(object sender, EventArgs e)
        {

        }

     
      

        private void btnRoll_Click(object sender, EventArgs e)
        {
            remainingSteps = random.Next(1, 13);
            lblSteps.Text = $"Steps: {remainingSteps}";
            roomsPic.Focus();
            btnRoll.Enabled = false;
            btnNext.Enabled = true;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (playerNotepads.ContainsKey(currentPlayerIndex) && playerNotepads[currentPlayerIndex] != null)
            {
                try
                {
                    notepadStates[currentPlayerIndex] = playerNotepads[currentPlayerIndex].GetCheckedState();
                    playerNotepads[currentPlayerIndex].Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении блокнота игрока {currentPlayerIndex}: {ex.Message}");
                }
            }

            // Moving on to the next player
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            remainingSteps = 0;

            // Restore the new player's notebook
            if (playerNotepads.ContainsKey(currentPlayerIndex) && playerNotepads[currentPlayerIndex] != null)
            {
                var notepad = playerNotepads[currentPlayerIndex];

                try
                {
                    if (notepadStates.ContainsKey(currentPlayerIndex))
                    {
                        notepad.RestoreCheckedState(notepadStates[currentPlayerIndex]);
                    }
                    notepad.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when restoring player's notebook {currentPlayerIndex}: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show($"Player's notebook {currentPlayerIndex} не найден.");
            }

            this.ActiveControl = null;
            roomsPic.Focus();
            btnNext.Enabled = false;
            btnRoll.Enabled = true;
        }

        private void btnNode_Click(object sender, EventArgs e)
        {
            if (playerNotepads[currentPlayerIndex] != null)
                notepadStates[currentPlayerIndex] = playerNotepads[currentPlayerIndex].GetCheckedState();

            
            var notepad = playerNotepads[currentPlayerIndex];

            if (notepadStates[currentPlayerIndex] != null)
                notepad.RestoreCheckedState(notepadStates[currentPlayerIndex]);

            notepad.Show(); 
        }
    }
}

