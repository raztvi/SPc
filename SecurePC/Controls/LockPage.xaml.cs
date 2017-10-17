using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecurePC.Controls
{
    /// <summary>
    /// Interaction logic for LockPage.xaml
    /// </summary>
    public partial class LockPage : Page
    {
        public LockPage()
        {
            InitializeComponent();
            ((MainWindow)Application.Current.MainWindow).SetLogsVisibility(false);
            ((MainWindow)Application.Current.MainWindow).SetKeyboadsMouseFunctionality(true);
        }

        /// <summary>
        /// Lock button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lockBtn_Click(object sender, RoutedEventArgs e)
        {
           ((MainWindow)Application.Current.MainWindow).NavigateTo(typeof(WebcamPage));
        }

        


    }
}
