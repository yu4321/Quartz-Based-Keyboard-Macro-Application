using GalaSoft.MvvmLight.Threading;
using log4net;
using System.Windows;

namespace QuartzBaseMacroProgramWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly ILog Logger = QuartzBaseMacroProgramWPF.Utils.Logger.LogWriterMessage;

        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}