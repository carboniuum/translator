using System;
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

            notifyIcon.BalloonTipTitle = "Translator app";
            notifyIcon.BalloonTipText = "App is hidden";
            notifyIcon.Text = "Translator app";
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            main.UnRegisterHotKeys();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000);
            }
            else
            {
                if (FormWindowState.Normal == this.WindowState)
                {
                    notifyIcon.Visible = false;
                }
            }
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
    }

}
