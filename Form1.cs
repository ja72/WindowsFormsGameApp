using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JA.Game;

namespace JA
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DxGame game;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            game = new DxGame(pictureBox1);

            game.Update += (s, ev) =>
            {
                if (game.IsPaused)
                {
                    toolStripLabel1.Text = $"Score: {game.Score} | Press SPACE to unpause | press ESC to quit.";
                }
                else
                {
                    toolStripLabel1.Text = $"Score: {game.Score} | Press SPACE to pause | press ESC to quit.";
                }
            };
        }
    }
}
