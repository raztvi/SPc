using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Controls;
using SecurePC.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CheckAuth.xaml
    /// </summary>
    public partial class CheckAuth : Page
    {
        #region Fields

        /// <summary>
        /// Face detection result container for image on the left
        /// </summary>
        private ObservableCollection<Face> _leftResultCollection = new ObservableCollection<Face>();

        /// <summary>
        /// Face detection result container for image on the right
        /// </summary>
        private ObservableCollection<Face> _rightResultCollection = new ObservableCollection<Face>();

        #endregion

        #region Properties

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
        /// Gets face detection results for image on the left
        /// </summary>
        public ObservableCollection<Face> LeftResultCollection
        {
            get
            {
                return _leftResultCollection;
            }
        }

        #endregion

        public CheckAuth()
        {
            InitializeComponent();
            ((MainWindow)Application.Current.MainWindow).SetKeyboadsMouseFunctionality(false);
            ValidateAuth();
        }

        /// <summary>
        /// Validate authentication
        /// </summary>
        private async void ValidateAuth()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            var validate = await VerifyAuth(mainWindow);

            if(validate)
            {
                mainWindow.SetKeyboadsMouseFunctionality(true);
                mainWindow.NavigateTo(typeof(AuthSuccessPage));
            }
            else
            {
                mainWindow.NavigateTo(typeof(AuthFailPage));
            }
        }

        /// <summary>
        /// Check if authentication was successful
        /// </summary>
        /// <returns></returns>
        private async Task<bool> VerifyAuth(MainWindow mainWindowInstance)
        {
           // return true;

            // User already picked one image
            var pickedImagePath = System.IO.Path.GetFullPath(ImageNameHelper.GetLatestWebcamImage());
            var renderingImage = FaceRecognitionHelper.LoadImageAppliedOrientation(pickedImagePath);
            var imageInfo = FaceRecognitionHelper.GetImageInfoForRendering(renderingImage);

            mainWindowInstance.Log(string.Format("Request: Detecting in {0}", pickedImagePath));
            var sw = Stopwatch.StartNew();

            LeftResultCollection.Clear();

            var detectedFaces = await MicrosoftApiHelper.DetectFaces(pickedImagePath, mainWindowInstance, imageInfo);
            for (var i = 0; i < detectedFaces.Count; i++)
            {
                LeftResultCollection.Add(detectedFaces[i]);
            }        

            //    FaceVerifyResult = string.Empty;

            var pickedImagePath2 = System.IO.Path.GetFullPath(Constants.OwnerImagePath);
            var renderingImage2 = FaceRecognitionHelper.LoadImageAppliedOrientation(pickedImagePath2);
            var imageInfo2 = FaceRecognitionHelper.GetImageInfoForRendering(renderingImage2);

            mainWindowInstance.Log(string.Format("Request: Detecting in {0}", pickedImagePath2));
            var sw2 = Stopwatch.StartNew();

            // Clear last time detection results
            RightResultCollection.Clear();

            detectedFaces = await MicrosoftApiHelper.DetectFaces(pickedImagePath2, mainWindowInstance, imageInfo2);
            for (var i = 0; i < detectedFaces.Count; i++)
            {
                RightResultCollection.Add(detectedFaces[i]);
            }

            if (LeftResultCollection.Count == 1 && RightResultCollection.Count == 1)
            {               
                var faceId1 = LeftResultCollection[0].FaceId;
                var faceId2 = RightResultCollection[0].FaceId;

                mainWindowInstance.Log(string.Format("Request: Verifying face {0} and {1}", faceId1, faceId2));

                // Call verify REST API with two face id
                try
                {
                    var faceServiceClient = new FaceServiceClient(Constants.SubscriptionKey, Constants.ApiEndpoint);
                    var res = await faceServiceClient.VerifyAsync(Guid.Parse(faceId1), Guid.Parse(faceId2));

                    mainWindowInstance.Log(string.Format("Response: Confidence = {0:0.00}, Face {1} and {2} {3} to the same person", res.Confidence, faceId1, faceId2, res.IsIdentical ? "belong" : "not belong"));

                    return res.IsIdentical;

                }
                catch (FaceAPIException ex)
                {
                    mainWindowInstance.Log(string.Format("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage));
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Verification accepts two faces as input, please pick images with only one detectable face in it.", "Warning", MessageBoxButton.OK);

                mainWindowInstance.NavigateTo(typeof(WebcamPage));
                return false;
            }

            GC.Collect();
        }

    }
}
