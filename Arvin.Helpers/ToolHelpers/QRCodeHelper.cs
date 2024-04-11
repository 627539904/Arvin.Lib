using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Arvin.Helpers
{
    public class QRCodeHelper
    {
        /// <summary>
        /// 二维码生成
        /// </summary>
        /// <param name="url">二维码链接地址</param>
        /// <param name="imgPath">生成的二维码图片存储地址</param>
        public static void QrCodeGenerate(string url, string imgPath)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(imgPath);
            }
        }
    }
}
