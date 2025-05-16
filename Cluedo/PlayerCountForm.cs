using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cluedo
{
    public partial class PlayerCountForm : Form
    {
        public int PlayerCount { get; private set; } = 2;
        public PlayerCountForm()
        {
            InitializeComponent();
            this.Text = "Selecting the number of players";

            Label label = new Label
            {
                Text = "Selecting the number of players(min 2, max 6)",
                AutoSize = true,
                Location = new Point(10, 10)
            };
            Controls.Add(label);

            NumericUpDown numericUpDown = new NumericUpDown
            {
                Minimum = 2,
                Maximum = 6,
                Value = 2,
                Location = new Point(10, 40)
            };
            Controls.Add(numericUpDown);

            Button btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(10, 80)
            };
            btnOk.Click += (s, e) => PlayerCount = (int)numericUpDown.Value;
            Controls.Add(btnOk);

            this.AcceptButton = btnOk;
            this.ClientSize = new Size(200, 120);
        }
    


        private void PlayerCountForm_Load(object sender, EventArgs e)
        {

        }
    }
}
