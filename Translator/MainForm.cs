using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Translator
{
    public partial class MainForm : Form
    {
        private IntPtr thisWindow;
        private Main main;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            thisWindow = Main.FindWindow(null, "Main Form");
            main = new Main(thisWindow);
            main.RegisterHotKeys();
        }

        private void buttonToggle_Click(object sender, EventArgs e)
        {
            string temp = labelLeft.Text;
            labelLeft.Text = labelRight.Text;
            labelRight.Text = temp;

            textBoxLeft.Text = textBoxRight.Text = String.Empty;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                SendKeys.Send("^(c)");
                main.TranslateText(out string inp, out string outp, labelLeft.Text);
                textBoxLeft.Text = inp;
                textBoxRight.Text = outp;
            }
            base.WndProc(ref m);
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            main.UnRegisterHotKeys();
            this.Close();
        }
    }

}
