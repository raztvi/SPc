using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePC.Helpers
{
    public class ImageNameHelper
    {
        public static string GetNextName()
        {
            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            return Constants.WebcamImagePath + "webcam_" + timeStamp + ".jpg";
        }

        public static string GetLatestWebcamImage()
        {
            var webcamDirectory = new DirectoryInfo("\\Poza");
            var myFile = webcamDirectory.GetFiles()
             .OrderByDescending(f => f.LastWriteTime)
             .Select(f => f.Name)
             .First();

            return Constants.WebcamImagePath + myFile;
        }
    }
}
