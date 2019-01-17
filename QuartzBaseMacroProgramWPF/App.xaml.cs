using System.Windows;
using GalaSoft.MvvmLight.Threading;
using log4net;

namespace QuartzBaseMacroProgramWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly ILog Logger = QuartzBasedMacroProgram.Utils.Logger.LogWriterMessage;
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
