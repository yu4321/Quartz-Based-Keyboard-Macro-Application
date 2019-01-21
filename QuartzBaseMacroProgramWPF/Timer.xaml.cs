using QuartzBaseMacroProgramWPF.Utils;
using System;
using System.Windows;

namespace QuartzBaseMacroProgramWPF
{
    /// <summary>
    /// Timer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Timer : Window
    {
        private System.Windows.Forms.Timer timer1;
        private int counter = Properties.Settings.Default.Timer;

        public Timer()
        {
            InitializeComponent();
            GlobalVars.istimerticking = true;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000; // 1 second
            timer1.Start();
            count.Text = counter.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            counter--;
            if (counter == 0)
            {
                timer1.Stop();
                GlobalVars.istimerticking = false;
                this.Close();
            }
            count.Text = counter.ToString();
        }
    }
}