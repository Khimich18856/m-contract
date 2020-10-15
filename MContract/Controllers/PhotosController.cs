using MContract.AppCode;
using MContract.DAL;
using MContract.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace MContract.Controllers
{
    public class PhotosController
    {
        public static string GetFileDirectoryForPhotos(Photo photo)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "DContent\\user-photos\\" + (photo.UserId / 1000) + "\\" + photo.UserId + "\\" + (photo.AdId != null ? "Ads\\" + photo.AdId + "\\" : "Logos\\");
        }

        public static List<Photo> AddPhoto(System.Drawing.Image inputImage, Photo photo)
        {
            var result = new List<Photo>();
            string fileDirectoryForPhotos = GetFileDirectoryForPhotos(photo);

            if (!Directory.Exists(fileDirectoryForPhotos))
                Directory.CreateDirectory(fileDirectoryForPhotos);

            photo.GroupId = Guid.NewGuid();

            try
            {
                result.Add(ResizeAndSavePhotoAndPhotoInfo(inputImage, 200, 200, photo));
                result.Add(SavePhoto(inputImage, photo));
            }
            catch (Exception ex)
            {
                LogsDAL.AddError("in PhotosController.AddPhotoAndGenerateResizedPhotos(): " + ex.ToString());
            }
            return result;
        }

        private static Photo ResizeAndSavePhotoAndPhotoInfo(System.Drawing.Image inputImage, int newWidth, int newHeight, Photo photo)
        {
            System.Drawing.Image resizedImage = GetResizedPhoto(inputImage, newWidth, newHeight);
            var resizedPhoto = new Photo()
            {
                GroupId = photo.GroupId,
                UserId = photo.UserId,
                AdId = photo.AdId,
                IsMain = photo.IsMain,
                ModerateResult = photo.ModerateResult,
                Added = photo.Added,
                Changed = photo.Changed,
                PhotoType = photo.PhotoType
            };
            return SavePhoto(resizedImage, resizedPhoto);
        }

        public static System.Drawing.Image GetResizedPhoto(System.Drawing.Image inputImage, int maxWidth, int maxHeight)
        {
            decimal divisor;
            decimal divisorWidth = 0;
            decimal divisorHeight = 0;
            if (inputImage.Width > maxWidth)
                divisorWidth = inputImage.Width / maxWidth;

            if (maxHeight > 0 && inputImage.Height > maxHeight)
                divisorHeight = inputImage.Height / maxHeight;

            divisor = divisorWidth > divisorHeight ? divisorWidth : divisorHeight;

            System.Drawing.Image smallImage;
            if (divisor > 0)
            {
                int newWidth = Convert.ToInt32(inputImage.Width / divisor);
                int newHeight = Convert.ToInt32(inputImage.Height / divisor);
                Bitmap bmpOut = new Bitmap(newWidth, newHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                // try testing with following options:
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                g.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                g.DrawImage(inputImage, 0, 0, newWidth, newHeight);

                smallImage = (System.Drawing.Image)bmpOut;
            }
            else
                smallImage = inputImage;

            return smallImage;
        }

        public static Photo SavePhoto(System.Drawing.Image photoImage, Photo photo)
        {
            var directory = GetFileDirectoryForPhotos(photo);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            /*Photo photo = new Photo();
            photo.UserId = userId;
            //photo.ImageType = "jpeg";
            //photo.ModerateResult = moderateResult;
            //photo.IsMain = setMain;
            //photo.Width = photo.Width;
            //photo.Height = photo.Height;
            //photo.NearestWidth = nearestWidth;*/
            #region Get unique fileName
            string fileName = String.Empty;
            while (true)
            {
                string _fileName = Guid.NewGuid().ToString().Replace("-", "").Remove(10) + ".jpg";
                if (!System.IO.File.Exists(directory + _fileName))
                {
                    fileName = _fileName;
                    break;
                }
            }

            if (fileName == String.Empty)
            {
                LogsDAL.AddError("in PhotosController.SavePhoto(): filePathAndName = String.Empty");
                //return false;
            }
            #endregion
            SaveJPGWithCompressionSetting(photoImage, directory + fileName, 70L);
            photo.FileNameWithExtension = fileName;
            photo.Width = photoImage.Width;
            photo.Height = photoImage.Height;
            photo.Id = PhotosDAL.AddPhoto(photo);
            return photo;
        }
        
        public static bool DeletePhoto(Photo photo)
        {
            var group = PhotosDAL.GetPhotoGroup(photo.GroupId);
            foreach (var photoToRemove in group)
            {
                if (System.IO.File.Exists(photoToRemove.FullPath))
                {
                    System.IO.File.Delete(photoToRemove.FullPath);
                }
                else
                {
                    LogsDAL.AddError("in PhotosController.DeletePhoto(): System.IO.File.Exists(photo.FullPath) == false");
                    return false;
                }
            }
            PhotosDAL.DeletePhotoGroup(photo.GroupId);
            return true;
        }

        private static void SaveJPGWithCompressionSetting(System.Drawing.Image image, string directoryAndFileName, long lCompression)
        {
            EncoderParameters eps = new EncoderParameters(1);
            eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, lCompression);
            ImageCodecInfo ici = GetEncoderInfo("image/jpeg");

            try
            {
                image.Save(directoryAndFileName, ici, eps);
            }
            catch
            {
                //обработка исключения: generic GDI+ error
                using (var img = new Bitmap(image))
                {
                    img.Save(directoryAndFileName, ImageFormat.Jpeg);
                }
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public static List<Photo> CopyAdPhotos(List<Photo> photos, int adId)
        {
            if (!photos.Any())
            {
                return new List<Photo>();
            }
            foreach (var group in photos.GroupBy(p => p.GroupId).Select(g => g.ToList()).ToList())
            {
                var groupId = Guid.NewGuid();
                foreach (var photo in group)
                {
                    var oldFullPath = photo.FullPath.Replace("/", "\\");
                    if (!System.IO.File.Exists(oldFullPath))
                    {
                        continue;
                    }
                    photo.AdId = adId;
                    var directory = PhotosController.GetFileDirectoryForPhotos(photo);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    photo.GroupId = groupId;
                    string fileName = String.Empty;
                    #region Get unique fileName
                    while (true)
                    {
                        string _fileName = Guid.NewGuid().ToString().Replace("-", "").Remove(10) + ".jpg";
                        if (!System.IO.File.Exists(directory + _fileName))
                        {
                            fileName = _fileName;
                            break;
                        }
                    }

                    if (fileName == String.Empty)
                    {
                        LogsDAL.AddError("in PhotosController.SavePhoto(): filePathAndName = String.Empty");
                        //return false;
                    }
                    #endregion
                    photo.FileNameWithExtension = fileName;
                    System.IO.File.Copy(oldFullPath, photo.FullPath.Replace("/", "\\"));
                    photo.Added = DateTime.Now.ToUniversalTime();
                    photo.Changed = photo.Added;
                    PhotosDAL.AddPhoto(photo);
                }
            }
            return photos;
        }
    }
}