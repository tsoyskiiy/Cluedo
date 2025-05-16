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
    public partial class notepad : Form
    {
        public string PlayerName { get; private set; }
        public notepad(string playerName)
        {
            InitializeComponent();
            PlayerName = playerName;
            this.Text = $"Notepad - {PlayerName}";
        }

        public Dictionary<string, bool> GetCheckedState()
        {
            var state = new Dictionary<string, bool>();
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is CheckedListBox clb)
                {
                    foreach (var item in clb.Items)
                    {
                        int index = clb.Items.IndexOf(item);
                        string key = $"{clb.Name}_{item}";
                        state[key] = clb.GetItemChecked(index);
                    }
                }
            }
            return state;
        }
        public void RestoreCheckedState(Dictionary<string, bool> state)
        {
            if (state == null) return;

            foreach (var control in this.Controls)
            {
                if (control is CheckBox cb && state.ContainsKey(cb.Name))
                {
                    cb.Checked = state[cb.Name];
                }
            }
        }
    }
}
