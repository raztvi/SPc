
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Controls;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SecurePC.Helpers
{
    public static class FaceRecognitionHelper
    {

        /// <summary>
        /// Rotate image by its orientation
        /// </summary>
        /// <param name="imagePath">image path</param>
        /// <returns>image for rendering</returns>
        public static BitmapImage LoadImageAppliedOrientation(string imagePath)
        {
            var im = new BitmapImage();
            im.BeginInit();
            im.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            im.Rotation = GetImageOrientation(imagePath);
            im.EndInit();
            return im;
        }

        /// <summary>
        /// Get image orientation flag.
        /// </summary>
        /// <param name="imagePath">image path</param>
        /// <returns></returns>
        public static Rotation GetImageOrientation(string imagePath)
        {
            using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                // See WIC Photo metadata policies for orientation query 
                // https://msdn.microsoft.com/en-us/library/windows/desktop/ee872007(v=vs.85).aspx
                const string query = "System.Photo.Orientation";
                var metadata = (BitmapMetadata)(BitmapFrame.Create(fs).Metadata);

                if (metadata != null && metadata.ContainsQuery(query))
                {
                    var orientationFlag = metadata.GetQuery(query);
                    if (orientationFlag != null)
                    {
                        switch ((ushort)orientationFlag)
                        {
                            case 6:
                                return Rotation.Rotate90;
                            case 3:
                                return Rotation.Rotate180;
                            case 8:
                                return Rotation.Rotate270;
                        }
                    }
                }
            }
            return Rotation.Rotate0;
        }

        /// <summary>
        /// Calculate the rendering face rectangle
        /// </summary>
        /// <param name="faces">Detected face from service</param>
        /// <param name="maxSize">Image rendering size</param>
        /// <param name="imageInfo">Image width and height</param>
        /// <returns>Face structure for rendering</returns>
        public static IEnumerable<Face> CalculateFaceRectangleForRendering(IEnumerable<Microsoft.ProjectOxford.Face.Contract.Face> faces, int maxSize, Tuple<int, int> imageInfo)
        {
            var imageWidth = imageInfo.Item1;
            var imageHeight = imageInfo.Item2;
            float ratio = (float)imageWidth / imageHeight;
            int uiWidth = 0;
            int uiHeight = 0;
            if (ratio > 1.0)
            {
                uiWidth = maxSize;
                uiHeight = (int)(maxSize / ratio);
            }
            else
            {
                uiHeight = maxSize;
                uiWidth = (int)(ratio * uiHeight);
            }

            int uiXOffset = (maxSize - uiWidth) / 2;
            int uiYOffset = (maxSize - uiHeight) / 2;
            float scale = (float)uiWidth / imageWidth;

            foreach (var face in faces)
            {
                yield return new Face()
                {
                    FaceId = face.FaceId.ToString(),
                    Left = (int)((face.FaceRectangle.Left * scale) + uiXOffset),
                    Top = (int)((face.FaceRectangle.Top * scale) + uiYOffset),
                    Height = (int)(face.FaceRectangle.Height * scale),
                    Width = (int)(face.FaceRectangle.Width * scale),
                };
            }
        }

        /// <summary>
        /// Get image basic information for further rendering usage
        /// </summary>
        /// <param name="imageFile">image file</param>
        /// <returns>Image width and height</returns>
        public static Tuple<int, int> GetImageInfoForRendering(BitmapImage imageFile)
        {
            try
            {
                return new Tuple<int, int>(imageFile.PixelWidth, imageFile.PixelHeight);
            }
            catch
            {
                return new Tuple<int, int>(0, 0);
            }
        }

        /// <summary>
        /// Append detected face to UI binding collection
        /// </summary>
        /// <param name="collections">UI binding collection</param>
        /// <param name="imagePath">Original image path, used for rendering face region</param>
        /// <param name="face">Face structure returned from service</param>
        public static void UpdateFace(ObservableCollection<Face> collections, string imagePath, Microsoft.ProjectOxford.Face.Contract.AddPersistedFaceResult face)
        {
            var renderingImage = LoadImageAppliedOrientation(imagePath);
            collections.Add(new Face()
            {
                ImageFile = renderingImage,
                FaceId = face.PersistedFaceId.ToString(),
            });
        }

        /// <summary>
        /// Append detected face to UI binding collection
        /// </summary>
        /// <param name="collections">UI binding collection</param>
        /// <param name="imagePath">Original image path, used for rendering face region</param>
        /// <param name="face">Face structure returned from service</param>
        public static void UpdateFace(ObservableCollection<Face> collections, string imagePath, Microsoft.ProjectOxford.Face.Contract.Face face)
        {
            var renderingImage = LoadImageAppliedOrientation(imagePath);
            collections.Add(new Face()
            {
                ImageFile = renderingImage,
                Left = face.FaceRectangle.Left,
                Top = face.FaceRectangle.Top,
                Width = face.FaceRectangle.Width,
                Height = face.FaceRectangle.Height,
                FaceId = face.FaceId.ToString(),
            });
        }

        /// <summary>
        /// Logging helper function
        /// </summary>
        /// <param name="log">log output instance</param>
        /// <param name="newMessage">message to append</param>
        /// <returns>log string</returns>
        public static string AppendLine(this string log, string newMessage)
        {
            return string.Format("{0}[{3}]: {2}{1}", log, Environment.NewLine, newMessage, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
        }

        
    }
       
}
