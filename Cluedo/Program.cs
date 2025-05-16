using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cluedo
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var playerCountForm = new PlayerCountForm())
            {
                if (playerCountForm.ShowDialog() == DialogResult.OK)
                {
                    int count = playerCountForm.PlayerCount;
                    Application.Run(new Form1(count));
                }
            }
        }
    }
}
