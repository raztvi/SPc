using SecurePC.Controls;
using SecurePC.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace SecurePC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<NavigationPage> NavigationPagesList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.NavigationPagesList = GetNavigationPages();

            Log("Application Log Text");
            SetKeyboadsMouseFunctionality(true);
            GoToLockscreen();
        }

        private List<NavigationPage> GetNavigationPages()
        {
            var pages = new List<NavigationPage>();
            
            pages.Add(new NavigationPage(){
                PageClass = typeof(LockPage),
                Title = "Lock",
            });

            pages.Add(new NavigationPage(){
                PageClass = typeof(AuthFailPage),
                Title = "AuthFailed",
            });

            pages.Add(new NavigationPage(){
                PageClass = typeof(AuthSuccessPage),
                Title = "AuthSucceed",
            });

            pages.Add(new NavigationPage(){
                PageClass = typeof(WebcamPage),
                Title = "Webcam",
            });

            pages.Add(new NavigationPage()
            {
                PageClass = typeof(CheckAuth),
                Title = "AuthCheck",
            });

            return pages;
        }

        public void NavigateTo(Type PageClass)
        {
            Page page = Activator.CreateInstance(PageClass) as Page;
            page.DataContext = this.DataContext;
            this._scenarioFrame.Navigate(page);
        }

        public void SetLogsVisibility(bool visible)
        {
            if (visible)
            {
                this._logTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                this._logTextBox.Visibility = Visibility.Collapsed;
            }
            
        }

        public void Log(string logMessage)
        {
            if (String.IsNullOrEmpty(logMessage) || logMessage == "\n")
            {
                _logTextBox.Text += "\n";
            }
            else
            {
                string timeStr = DateTime.Now.ToString("HH:mm:ss.ffffff");
                string messaage = "[" + timeStr + "]: " + logMessage + "\n";
                _logTextBox.Text += messaage;
            }
            _logTextBox.ScrollToEnd();
        }

        public void ClearLog()
        {
            _logTextBox.Text = "";
        }

        public void SetKeyboadsMouseFunctionality(bool isFunctional)
        {
            if (isFunctional)
            {
                this.Topmost = false;
                this.WindowState = WindowState.Normal;
                this.ShowInTaskbar = true;
                this.ResizeMode = ResizeMode.CanResize;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.MinHeight = 540;
                this.MinWidth = 300;
                this.Height = 700;
                this.Width = 900;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                this.Topmost = true;
                this.WindowStyle = WindowStyle.None;
                this.ShowInTaskbar = false;
                this.ResizeMode = ResizeMode.NoResize;
                this.WindowState = WindowState.Maximized;
            }
        }

        public void GoToLockscreen()
        {
            //-- get lock scenario
            NavigationPage navPage = NavigationPagesList.First(p => p.Title.ToLower().Equals("lock")) as NavigationPage;
            //-- open lock page
            NavigateTo(navPage.PageClass);
        }
    }
}
