using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Controls;
using SecurePC.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for AuthSuccessPage.xaml
    /// </summary>
    public partial class AuthSuccessPage : Page, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Description dependency property
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(AuthSuccessPage));

        /// <summary>
        /// Face detection result container for image on the left
        /// </summary>
        private ObservableCollection<Face> _leftResultCollection = new ObservableCollection<Face>();

        /// <summary>
        /// Face detection result container for image on the right
        /// </summary>
        private ObservableCollection<Face> _rightResultCollection = new ObservableCollection<Face>();
     
        /// <summary>
        /// Face to face verification result
        /// </summary>
        private string _faceVerifyResult;
        private bool shutdownState = false;

        #endregion Fields

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets max image size for UI rendering
        /// </summary>
        public int MaxImageSize
        {
            get
            {
                return Constants.MaxImageSize;
            }
        }

        /// <summary>
        /// Gets or sets description for UI rendering
        /// </summary>
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }

            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets face detection results for image on the left
        /// </summary>
        public ObservableCollection<Face> LeftResultCollection
        {
            get
            {
                return _leftResultCollection;
            }
        }


        /// <summary>
        /// Gets face detection results for image on the right
        /// </summary>
        public ObservableCollection<Face> RightResultCollection
        {
            get
            {
                return _rightResultCollection;
            }
        }

        /// <summary>
        /// Gets or sets selected face verification result
        /// </summary>
        public string FaceVerifyResult
        {
            get
            {
                return _faceVerifyResult;
            }

            set
            {
                _faceVerifyResult = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FaceVerifyResult"));
                }
            }
        }

        #endregion Properties

        public AuthSuccessPage()
        {
            InitializeComponent();

            FaceVerifyResult = String.Empty;

            SetImages();

          

            //if (shutdownState)
            //{
            //    App.Current.Shutdown();
            //}
        }

        private async void SetImages()
        {
            var mainWindowInstance = (MainWindow)Application.Current.MainWindow;

            await SetWebcamImage(mainWindowInstance);
            await SetModelImage(mainWindowInstance);
            await Verify(mainWindowInstance);
            
        }

        private async Task SetWebcamImage(MainWindow mainWindowInstance)
        {
            var pickedImagePath = System.IO.Path.GetFullPath(ImageNameHelper.GetLatestWebcamImage());
            var renderingImage = FaceRecognitionHelper.LoadImageAppliedOrientation(pickedImagePath);
            var imageInfo = FaceRecognitionHelper.GetImageInfoForRendering(renderingImage);

            LeftImageDisplay.Source = renderingImage;
            
            mainWindowInstance.Log(string.Format("Request: Detecting in {0}", pickedImagePath));
            var sw = Stopwatch.StartNew();

            LeftResultCollection.Clear();

            var detectedFaces = await MicrosoftApiHelper.DetectFaces(pickedImagePath, mainWindowInstance, imageInfo);
            for (var i = 0; i < detectedFaces.Count; i++)
            {
                LeftResultCollection.Add(detectedFaces[i]);
            }      
        }

        private async Task SetModelImage(MainWindow mainWindowInstance)
        {
            var pickedImagePath2 = System.IO.Path.GetFullPath(Constants.OwnerImagePath);
            var renderingImage2 = FaceRecognitionHelper.LoadImageAppliedOrientation(pickedImagePath2);
            var imageInfo2 = FaceRecognitionHelper.GetImageInfoForRendering(renderingImage2);

            RightImageDisplay.Source = renderingImage2;

            mainWindowInstance.Log(string.Format("Request: Detecting in {0}", pickedImagePath2));
            var sw2 = Stopwatch.StartNew();

            // Clear last time detection results
            RightResultCollection.Clear();

            var detectedFaces = await MicrosoftApiHelper.DetectFaces(pickedImagePath2, mainWindowInstance, imageInfo2);
            for (var i = 0; i < detectedFaces.Count; i++)
            {
                RightResultCollection.Add(detectedFaces[i]);
            }
        }

        private async Task Verify(MainWindow mainWindowInstance)
        {
            // Call face to face verification, verify REST API supports one face to one face verification only
            // Here, we handle single face image only
            if (LeftResultCollection.Count == 1 && RightResultCollection.Count == 1)
            {
                FaceVerifyResult = "Verifying...";
                var faceId1 = LeftResultCollection[0].FaceId;
                var faceId2 = RightResultCollection[0].FaceId;

                mainWindowInstance.Log(string.Format("Request: Verifying face {0} and {1}", faceId1, faceId2));

                // Call verify REST API with two face id
                try
                {
                    var faceServiceClient = new FaceServiceClient(Constants.SubscriptionKey, Constants.ApiEndpoint);
                    var res = await faceServiceClient.VerifyAsync(Guid.Parse(faceId1), Guid.Parse(faceId2));

                    // Verification result contains IsIdentical (true or false) and Confidence (in range 0.0 ~ 1.0),
                    // here we update verify result on UI by FaceVerifyResult binding
                    FaceVerifyResult = string.Format("Confidence = {0:0.00}{1}{2}", res.Confidence, Environment.NewLine, res.IsIdentical ? "Two faces belong to same person" : "two faces not belong to same person");
                    mainWindowInstance.Log(string.Format("Response: Success. Face {0} and {1} {2} to the same person", faceId1, faceId2, res.IsIdentical ? "belong" : "not belong"));
                    //if( res.IsIdentical)
                    //{
                    //    shutdownState = true;
                    //    mainWindowInstance.Log(string.Format("stae et rue"));

                    //}
                    
                }
                catch (FaceAPIException ex)
                {
                    mainWindowInstance.Log(string.Format("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage));

                    return;
                }
            }
            else
            {
                MessageBox.Show("Verification accepts two faces as input, please pick images with only one detectable face in it.", "Warning", MessageBoxButton.OK);
            }

            GC.Collect();
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            //-- go back to Webcam page
            ((MainWindow)Application.Current.MainWindow).NavigateTo(typeof(LockPage));
        }
    }
}
