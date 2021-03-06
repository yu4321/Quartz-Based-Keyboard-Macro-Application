﻿using QuartzBaseMacroProgramWPF.Utils;
using QuartzBaseMacroProgramWPF.ViewModel;
using System.Windows;

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
            GlobalVars.SetSetting();
            GlobalVars.SetQuartzJobs();
            TrayService.ShowMSG($"작동을 시작합니다. 현재 작업 {GlobalVars.scheduler.GetTriggers().Count}개.");
            if (Properties.Settings.Default.InstantStart)
            {
                GlobalVars.scheduler.Start();
            }
            TrayService.GetInstance().TurnOn();
            this.Close();
        }
    }
}