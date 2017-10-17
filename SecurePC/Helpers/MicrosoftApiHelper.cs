using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SecurePC.Helpers
{
    public class MicrosoftApiHelper
    {
        /// <summary>
        /// Detect faces based on image,and return them
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="mainWindowInstance"></param>
        /// <param name="imageInfo"></param>
        /// <returns></returns>
        public static async Task<ObservableCollection<Face>> DetectFaces(string imagePath, MainWindow mainWindowInstance, Tuple<int, int> imageInfo)
        {
            var facesDetected = new ObservableCollection<Face>();

            // Call detection REST API, detect faces inside the image
            using (var fileStream = File.OpenRead(imagePath))
            {
                try
                {
                    var faceServiceClient = new FaceServiceClient(Constants.SubscriptionKey, Constants.ApiEndpoint);
                    var faces = await faceServiceClient.DetectAsync(fileStream);

                    // Handle REST API calling error
                    if (faces == null)
                    {
                        return facesDetected;
                    }

                    mainWindowInstance.Log(string.Format("Response: Success. Detected {0} face(s) in {1}", faces.Length, imagePath));

                    // Convert detection results into UI binding object for rendering
                    foreach (var face in FaceRecognitionHelper.CalculateFaceRectangleForRendering(faces, Constants.MaxImageSize, imageInfo))
                    {
                        // Detected faces are hosted in result container, will be used in the verification later
                        facesDetected.Add(face);
                    }
                }
                catch (FaceAPIException ex)
                {
                    mainWindowInstance.Log(string.Format("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage));
                    return facesDetected;
                }
            }

            return facesDetected;
        }

    }
}
