using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilsCore;
using UtilsCore.Result;
using YzmManager.Service;

namespace YzmManager.Controllers
{
    /// <summary>
    /// 验证码识别
    /// </summary>
    public class YzmController : Controller
    {
        // GET: Yzm

        /// <summary>
        /// 验证码识别
        /// </summary>
        /// <param name="base64">含验证码的页面全屏截图</param>
        /// <param name="x">验证码x</param>
        /// <param name="y">验证码y</param>
        /// <param name="width">验证码width</param>
        /// <param name="height">验证码height</param>
        /// <returns></returns>
        public JsonResult Index(string base64,int x=0,int y=0,int width=0,int height=0)
        {
            BaseResult<string> result=new BaseResult<string>(){Result = ""};
            if (string.IsNullOrEmpty(base64))
            {
                result.SetError("参数base64不能为空");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var fullPageBytes = Convert.FromBase64String(base64);
                var imageDir = Server.MapPath("~/App_Data/");
                const string fileSuffix = ".png";
                var fileName = Guid.NewGuid().ToString().Replace("-", "");
                var screenImagePath = imageDir + fileName + fileSuffix;
                var image = ImageHelper.BytesToImage(fullPageBytes);

                //计算出元素上、下、左、右 位置
                var left = x;
                var top = y;
                var right = width;
                var bottom = height;
                //定义截取矩形
                var cropArea = new Rectangle(left, top, right, bottom); //要截取的区域大小
                var screenImage = (Bitmap)image;
                //进行裁剪
                var bmpCrop = screenImage.Clone(cropArea, screenImage.PixelFormat);
                //保存成新文件
                bmpCrop.Save(screenImagePath);
                //释放对象
                screenImage.Dispose();
                bmpCrop.Dispose();

                //var screenImagePath= imageDir+"111.jpg";

                var yzm = new YzmService();
                result.Result= yzm.RecognizeLocal(screenImagePath);
                return Json(result,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
           
        }
    }
}