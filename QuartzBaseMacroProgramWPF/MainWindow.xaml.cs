using System.Windows;
using QuartzBaseMacroProgramWPF.ViewModel;
using System.IO;
using QuartzBasedMacroProgram.Utils;
using Newtonsoft.Json;
using QuartzBaseMacroProgramWPF.Model;
using QuartzBaseMacroProgramWPF.Utils;
using Quartz;
using Quartz.Impl;

namespace QuartzBaseMacroProgramWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            //KeyboardInputs.PressKey(0x5B);
            //System.Windows.Forms.SendKeys.Send("{CAPSLOCK}");
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            GlobalVars.SetQuartzJobs();
            GlobalVars.scheduler.Start();
            TrayService.GetInstance().TurnOn();
            this.Close();
            //GlobalVars.scheduler.TestRun();
        }

    }
}