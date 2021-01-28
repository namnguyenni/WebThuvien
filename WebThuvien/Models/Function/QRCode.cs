using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using ZXing;
using ZXing.QrCode;
namespace WebThuvien.Models.Function
{
    public class QRCode
    {
        private string ReadQRCode()
        {
            string barcodeText = "";
            string imagePath = "~/Content/Images/QrCode.jpg";
            string barcodePath = HttpContext.Current.Server.MapPath(imagePath);
            var barcodeReader = new BarcodeReader();

            var result = barcodeReader.Decode(new Bitmap(barcodePath));
            if (result != null)
            {
                barcodeText = result.Text;
            }
            return barcodeText;
        }


        public string GenerateQRCode(string qrcodeText)
        {
            string folderPath = "~/Content/Images/";
            string imagePath = "~/Content/Images/"+ qrcodeText + ".png";
            // If the directory doesn't exist then create it.
            if (!Directory.Exists(HttpContext.Current.Server.MapPath(folderPath)))
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(folderPath));
            }

            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.CODE_128;
            barcodeWriter.Options = new ZXing.Common.EncodingOptions() { Width = 250, Height = 60 };
            var result = barcodeWriter.Write(qrcodeText);

            string barcodePath = HttpContext.Current.Server.MapPath(imagePath);
            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return imagePath;
        }

    }
}