using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace YzmManager.Service
{
    /// <summary>
    /// 验证码识别服务
    /// </summary>
    public class YzmService
    {
        [DllImport("WmCode.dll")]
        public static extern bool LoadWmFromFile(string FilePath, string Password);

        [DllImport("WmCode.dll")]
        public static extern bool LoadWmFromBuffer(byte[] FileBuffer, int FileBufLen, string Password);

        [DllImport("WmCode.dll")]
        public static extern bool GetImageFromFile(string FilePath, StringBuilder Vcode);

        [DllImport("WmCode.dll")]
        public static extern bool GetImageFromBuffer(byte[] FileBuffer, int ImgBufLen, StringBuilder Vcode);

        [DllImport("WmCode.dll")]
        public static extern bool SetWmOption(int OptionIndex, int OptionValue);

        /// <summary>
        /// 识别验证码(线上图片)
        /// </summary>
        /// <param name="imgUrl">图片地址</param>
        /// <returns></returns>
        public string Recognize(string imgUrl)
        {
            if (string.IsNullOrEmpty(imgUrl))
            {
                return "fail";
            }
            var imageDir = HttpContext.Current.Server.MapPath("~/App_Data/");
            var libPath = HttpContext.Current.Server.MapPath("~/App_Data/Alibaba.dat");
            var result = LoadWmFromFile(libPath, "yanggl");
            if (!result)
            {
                return "fail";
            }
            SetWmOption(6, 80);

            #region ===下载图片===
            var fileSuffix = ".jpg";
            var fileName = Guid.NewGuid().ToString().Replace("-", "");
            var imgPath = imageDir + fileName + fileSuffix;
            var wc = new WebClient();
            wc.DownloadFile(imgUrl, imgPath);
            #endregion

            var sbResult = new StringBuilder('\0', 256);
            var success = GetImageFromFile(imgPath, sbResult);
            File.Delete(imgPath);
            return success ? sbResult.ToString() : "fail";
        }

        /// <summary>
        /// 识别验证码（本地图片）
        /// </summary>
        /// <param name="imgPath">图片地址</param>
        /// <returns></returns>
        public string RecognizeLocal(string imgPath)
        {
            if (string.IsNullOrEmpty(imgPath))
            {
                return "fail";
            }

            var libPath = HttpContext.Current.Server.MapPath("~/App_Data/Alibaba.dat");
            var result = LoadWmFromFile(libPath, "yanggl");
            if (!result)
            {
                return "fail";
            }
            SetWmOption(6, 80);
            var sbResult = new StringBuilder('\0', 256);
            var success = GetImageFromFile(imgPath, sbResult);
            //File.Delete(imgPath);
            return success ? sbResult.ToString() : "fail";
        }
    }
}