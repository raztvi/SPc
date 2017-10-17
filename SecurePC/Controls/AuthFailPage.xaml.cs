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
using System.Runtime.InteropServices;
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
    /// Interaction logic for AuthFailPage.xaml
    /// </summary>
    public partial class AuthFailPage : Page, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Description dependency property
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(AuthFailPage));

        /// <summary>
        /// Face detection results in list container
        /// </summary>
        private ObservableCollection<Face> _detectedFaces = new ObservableCollection<Face>();

        /// <summary>
        /// Face detection results in text string
        /// </summary>
        private string _detectedResultsInText;

        /// <summary>
        /// Face detection results container
        /// </summary>
        private ObservableCollection<Face> _resultCollection = new ObservableCollection<Face>();

        /// <summary>
        /// Image used for rendering and detecting
        /// </summary>
        private ImageSource _selectedFile;

        #endregion Fields

        #region Constructors

        public AuthFailPage()
        {
            InitializeComponent();
            ((MainWindow)Application.Current.MainWindow).SetKeyboadsMouseFunctionality(false);
            SetImageAndDetect();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets description
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
        /// Gets face detection results
        /// </summary>
        public ObservableCollection<Face> DetectedFaces
        {
            get
            {
                return _detectedFaces;
            }
        }

        /// <summary>
        /// Gets or sets face detection results in text string
        /// </summary>
        public string DetectedResultsInText
        {
            get
            {
                return _detectedResultsInText;
            }

            set
            {
                _detectedResultsInText = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DetectedResultsInText"));
                }
            }
        }

        /// <summary>
        /// Gets constant maximum image size for rendering detection result
        /// </summary>
        public int MaxImageSize
        {
            get
            {
                return 300;
            }
        }

        /// <summary>
        /// Gets face detection results
        /// </summary>
        public ObservableCollection<Face> ResultCollection
        {
            get
            {
                return _resultCollection;
            }
        }

        /// <summary>
        /// Gets or sets image for rendering and detecting
        /// </summary>
        public ImageSource SelectedFile
        {
            get
            {
                return _selectedFile;
            }

            set
            {
                _selectedFile = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedFile"));
                }
            }
        }

        #endregion Properties

        #region Methods

        private async void SetImageAndDetect()
        {
            var mainWindowInstance = (MainWindow)Application.Current.MainWindow;

            var pickedImagePath = ImageNameHelper.GetLatestWebcamImage();
            var renderingImage = FaceRecognitionHelper.LoadImageAppliedOrientation(pickedImagePath);
            var imageInfo = FaceRecognitionHelper.GetImageInfoForRendering(renderingImage);
            
            SelectedFile = renderingImage;

            ResultCollection.Clear();
            DetectedFaces.Clear();
            DetectedResultsInText = string.Format("Detecting...");

            mainWindowInstance.Log(string.Format("Request: Detecting {0}", pickedImagePath));
            var sw = Stopwatch.StartNew();



            // Call detection REST API
            using (var fStream = File.OpenRead(pickedImagePath))
            {
                try
                {
                    var faceAttributeTypes = new FaceAttributeType[] {
                        FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses,
                        FaceAttributeType.HeadPose, FaceAttributeType.FacialHair, FaceAttributeType.Emotion, FaceAttributeType.Hair,
                        FaceAttributeType.Makeup, FaceAttributeType.Occlusion, FaceAttributeType.Accessories, FaceAttributeType.Noise,
                        FaceAttributeType.Exposure, FaceAttributeType.Blur };

                    var faceServiceClient = new FaceServiceClient(Constants.SubscriptionKey, Constants.ApiEndpoint);
                    var faces = await faceServiceClient.DetectAsync(fStream, false, true, faceAttributeTypes);

                    mainWindowInstance.Log(string.Format("Response: Success. Detected {0} face(s) in {1}", faces.Length, pickedImagePath));
                    DetectedResultsInText = string.Format("Info: {0} face(s) has been detected", faces.Length);

                    foreach (var face in faces)
                    {
                        DetectedFaces.Add(new Face()
                        {
                            ImageFile = SelectedFile,
                            Left = face.FaceRectangle.Left,
                            Top = face.FaceRectangle.Top,
                            Width = face.FaceRectangle.Width,
                            Height = face.FaceRectangle.Height,
                            FaceId = face.FaceId.ToString(),
                            Age = string.Format("{0:#} years old", face.FaceAttributes.Age),
                            Gender = face.FaceAttributes.Gender,
                            HeadPose = string.Format("Pitch: {0}, Roll: {1}, Yaw: {2}", Math.Round(face.FaceAttributes.HeadPose.Pitch, 2), Math.Round(face.FaceAttributes.HeadPose.Roll, 2), Math.Round(face.FaceAttributes.HeadPose.Yaw, 2)),
                            FacialHair = string.Format("FacialHair: {0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No"),
                            Glasses = string.Format("GlassesType: {0}", face.FaceAttributes.Glasses.ToString()),
                            Emotion = $"{GetEmotion(face.FaceAttributes.Emotion)}",
                            Hair = string.Format("Hair: {0}", GetHair(face.FaceAttributes.Hair)),
                            Makeup = string.Format("Makeup: {0}", ((face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No")),
                            EyeOcclusion = string.Format("EyeOccluded: {0}", ((face.FaceAttributes.Occlusion.EyeOccluded) ? "Yes" : "No")),
                            ForeheadOcclusion = string.Format("ForeheadOccluded: {0}", (face.FaceAttributes.Occlusion.ForeheadOccluded ? "Yes" : "No")),
                            MouthOcclusion = string.Format("MouthOccluded: {0}", (face.FaceAttributes.Occlusion.MouthOccluded ? "Yes" : "No")),
                            Accessories = $"{GetAccessories(face.FaceAttributes.Accessories)}",
                            Blur = string.Format("Blur: {0}", face.FaceAttributes.Blur.BlurLevel.ToString()),
                            Exposure = string.Format("{0}", face.FaceAttributes.Exposure.ExposureLevel.ToString()),
                            Noise = string.Format("Noise: {0}", face.FaceAttributes.Noise.NoiseLevel.ToString()),
                        });
                    }

                    // Convert detection result into UI binding object for rendering
                    foreach (var face in FaceRecognitionHelper.CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                    {
                        ResultCollection.Add(face);
                    }
                }
                catch (FaceAPIException ex)
                {
                    mainWindowInstance.Log(string.Format("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage));
                    GC.Collect();
                    return;
                }

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }
        }

        private string GetHair(Microsoft.ProjectOxford.Face.Contract.Hair hair)
        {
            if (hair.HairColor.Length == 0)
            {
                if (hair.Invisible)
                    return "Invisible";
                else
                    return "Bald";
            }
            else
            {
                Microsoft.ProjectOxford.Face.Contract.HairColorType returnColor = Microsoft.ProjectOxford.Face.Contract.HairColorType.Unknown;
                double maxConfidence = 0.0f;

                for (int i = 0; i < hair.HairColor.Length; ++i)
                {
                    if (hair.HairColor[i].Confidence > maxConfidence)
                    {
                        maxConfidence = hair.HairColor[i].Confidence;
                        returnColor = hair.HairColor[i].Color;
                    }
                }

                return returnColor.ToString();
            }
        }

        private string GetAccessories(Microsoft.ProjectOxford.Face.Contract.Accessory[] accessories)
        {
            if (accessories.Length == 0)
            {
                return "NoAccessories";
            }

            string[] accessoryArray = new string[accessories.Length];

            for (int i = 0; i < accessories.Length; ++i)
            {
                accessoryArray[i] = accessories[i].Type.ToString();
            }

            return "Accessories: " + String.Join(",", accessoryArray);
        }

        private string GetEmotion(Microsoft.ProjectOxford.Common.Contract.EmotionScores emotion)
        {
            string emotionType = string.Empty;
            double emotionValue = 0.0;
            if (emotion.Anger > emotionValue)
            {
                emotionValue = emotion.Anger;
                emotionType = "Anger";
            }
            if (emotion.Contempt > emotionValue)
            {
                emotionValue = emotion.Contempt;
                emotionType = "Contempt";
            }
            if (emotion.Disgust > emotionValue)
            {
                emotionValue = emotion.Disgust;
                emotionType = "Disgust";
            }
            if (emotion.Fear > emotionValue)
            {
                emotionValue = emotion.Fear;
                emotionType = "Fear";
            }
            if (emotion.Happiness > emotionValue)
            {
                emotionValue = emotion.Happiness;
                emotionType = "Happiness";
            }
            if (emotion.Neutral > emotionValue)
            {
                emotionValue = emotion.Neutral;
                emotionType = "Neutral";
            }
            if (emotion.Sadness > emotionValue)
            {
                emotionValue = emotion.Sadness;
                emotionType = "Sadness";
            }
            if (emotion.Surprise > emotionValue)
            {
                emotionValue = emotion.Surprise;
                emotionType = "Surprise";
            }
            return $"{emotionType}";
        }

        #endregion Methods

        #region Button Handlers

        private void tryAgainBtn_Click(object sender, RoutedEventArgs e)
        {
            //-- go back to Webcam page
            ((MainWindow)Application.Current.MainWindow).NavigateTo(typeof(WebcamPage));
        }

        #endregion
    }
}
