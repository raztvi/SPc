
using SecurePC.Helpers;
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
using WebEye.Controls.Wpf;

namespace SecurePC.Controls
{
    /// <summary>
    /// Interaction logic for WebcamPage.xaml
    /// </summary>
    public partial class WebcamPage : Page
    {
        private WebCameraId _captureDevice;

        public WebcamPage()
        {
            InitializeComponent();
            Clear();
            CheckCaptureDevices();
            ((MainWindow)Application.Current.MainWindow).SetKeyboadsMouseFunctionality(false);
        }

        /// <summary>
        /// Get available capture devices
        /// </summary>
        public void CheckCaptureDevices(){
            var captureDevices = webCameraControl.GetVideoCaptureDevices();
            if(captureDevices != null && captureDevices.Count() > 0){
                _captureDevice = captureDevices.First();
            }
        }

        /// <summary>
        /// Unlock button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void unlockBtn_Click(object sender, RoutedEventArgs e)
        {
            unlockBtn.Visibility = Visibility.Collapsed;
            _progressBar.Visibility = Visibility.Visible;

            if (_captureDevice != null)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;

                //-- stat webcam
                webCameraControl.StartCapture(_captureDevice);
                //-- wait for webcam to start
                System.Threading.Thread.Sleep(4000);
                
                //-- get the photo taken and save it
                var img = webCameraControl.GetCurrentImage();
                SaveImage(img);            

                //-- stop webcam
                webCameraControl.StopCapture();

                //-- navigate to check auth page
                mainWindow.SetLogsVisibility(true);  
                mainWindow.NavigateTo(typeof(CheckAuth));
            }
            else
            {
                MessageBox.Show("There is no webcam available.", "Warning", MessageBoxButton.OK);
            }

        }

        /// <summary>
        /// Save image helper
        /// </summary>
        /// <param name="image"></param>
        private void SaveImage(Bitmap image)
        {
            var webcamImagePath = System.IO.Path.GetFullPath(ImageNameHelper.GetNextName());

            image.Save(webcamImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Dispose();
        }

        private void Clear()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
    }
}
