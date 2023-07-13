using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace HYTETestTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NonDetectPage nonPage;
        ControlPage conPage;
        UIHelper Helper;
        public MainWindow()
        {
            InitializeComponent();
            nonPage = new NonDetectPage();
            conPage = new ControlPage();
            Helper = UIHelper.Instance;
            Helper.RegistryControlPage(NavToControllPage);
            Helper.RegistryNonPage(NavToNonePage);
            Helper.StartProcess();
        }

        void NavToControllPage()
        {
            Dispatcher.Invoke(() =>
            {
                if (EffectFrame.Content != conPage)
                {
                    EffectFrame.Content = conPage;
                }
            });
        }

        void NavToNonePage()
        {
            Dispatcher.Invoke(() =>
            {
                if (EffectFrame.Content != nonPage)
                {
                    EffectFrame.Content = nonPage;
                }
            });
        }

        private void EffectFrame_Navigated(object sender, NavigationEventArgs e)
        {
            EffectFrame.NavigationService.RemoveBackEntry();
        }
    }
}
