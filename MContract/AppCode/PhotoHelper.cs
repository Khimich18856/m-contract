using MContract.Controllers;
using MContract.DAL;
using MContract.Models;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace MContract.AppCode
{
	public static class PhotoHelper
    {
        public static string NoLogoImageUrl = C.SiteUrl + "/ico/nonePhoto.svg";
        public static string UploadPhoto(HttpRequestBase Request, int userId, int? adId = null)
        {
            var result = "";
            if (SM.CurrentUserIsNull)
                return "Для загрузки файлов необходимо авторизоваться на сайте";
            foreach (string upload in Request.Files)
            {
                var uploadFile = Request.Files[upload];
                if (uploadFile == null || uploadFile.ContentLength == 0)
                    continue;

                string filename = uploadFile.FileName;
                var extension = Path.GetExtension(filename);
                if (extension != null)
                    extension = extension.Replace(".", "");

                var deniedExtensions = new List<string>() { "exe", "js" };
                if (deniedExtensions.Contains(extension))
                    return "Запрещено загружать файлы с расширением " + extension;

                var isImage = new List<string>() { "jpg", "jpeg", "png" }.Contains(extension);
                #region обработка изображения
                //var resizedPhoto = PhotosController.GetResizedPhoto(uploadFile.InputStream, 130, 130);

                System.Drawing.Image inputImage = new System.Drawing.Bitmap(uploadFile.InputStream);
                var photo = new Photo()
                {
                    UserId = userId,
                    AdId = adId,
                    IsMain = false,
                    ModerateResult = ModerateResults.NotChecked,
                    Added = DateTime.Now.ToUniversalTime(),
                    Changed = DateTime.Now.ToUniversalTime(),
                    Width = inputImage.Width,
                    Height = inputImage.Height,
                    PhotoType = (adId != null ? PhotoTypes.AdPhoto : PhotoTypes.CompanyLogo)
                };

                var folder = PhotosController.GetFileDirectoryForPhotos(photo);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                try
                {
                    //uploadFile.SaveAs(photo.FullPath);
                    var photos = PhotosController.AddPhoto(inputImage, photo);
                    if (photos.Any())
                    {
                        result += "ok," + photos.FirstOrDefault().Id + "," + photo.Url + "|";
                    }
                }
                catch (Exception ex)
                {
                    result = "error";
                    LogsDAL.AddError("in UserController.UploadPhoto(): userId = " + photo.UserId + ", exception: " + ex);
                    return result;
                }
                #endregion
                /*if (falseisImage)
                {
                    #region обработка изображения
                    var resizedPhoto = PhotosController.GetResizedPhoto(uploadFile.InputStream, 130, 130);

                    var fileName = "main-" + Guid.NewGuid() + extension;

                    var photo = new Photo()
                    {
                        UserId = SM.CurrentUserId,
                        FileNameWithExtension = fileName,
                        IsMain = true,
                        ModerateResult = ModerateResults.NotChecked,
                        Added = DateTime.Now,
                        Changed = DateTime.Now,
                        Width = resizedPhoto.Width,
                        Height = resizedPhoto.Height
                    };

                    bool result = true;
                    try
                    {
                        //uploadFile.SaveAs(photo.FullPath);
                        resizedPhoto.Save(photo.FullPath);
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        LogsDAL.AddError("in UserController.UploadPhoto(): userId = " + photo.UserId + ", exception: " + ex);
                    }
                    #endregion
                }
                else
                {
                    var newFileFullPath = folder + "/" + filename;
                    if (System.IO.File.Exists(newFileFullPath))
                        return "На сервере уже есть файл с таким названием. Пожалуйста, переименуйте файл и попробуйте снова";

                    uploadFile.SaveAs(folder + "/" + filename);

                    var file = new MyFile()
                    {
                        NameWithExtension = filename,
                        Extension = extension,
                        ContainingFolderPath = folder,
                        IsAttachToComment = true,
                        UploadedUserId = SM.CurrentUserId,
                        UploadedDate = DateTime.Now,
                        SizeKb = uploadFile.ContentLength / 1000
                    };

                    var newFileId = FilesDAL.AddFile(file);

                    return "ok," + newFileId + "," + filename;
                }*/
            }

            return result;
        }
        public static bool DeletePhoto(int photoId)
        {
            var photo = PhotosDAL.GetPhoto(photoId);
            if (photo == null)
                return false;
            return PhotosController.DeletePhoto(photo);
        }
        public static bool MakePhotoMain(int photoId)
        {
            var photo = PhotosDAL.GetPhoto(photoId);
            if (photo == null)
                return false;
            var allPhotos = PhotosDAL.GetPhotos(photo.AdId.Value);
            if (!allPhotos.Any())
                return false;
            foreach (var photoNotMain in allPhotos.GroupBy(p => p.GroupId).Select(g => g.FirstOrDefault()).ToList())
            {
                PhotosDAL.UpdateIsMainGroup(photoNotMain.GroupId, false);
            }
            PhotosDAL.UpdateIsMainGroup(photo.GroupId, true);
            return true;
        }
        public static Image Crop(this Image image, int x1, int y1, int x2, int y2)
		{
			Bitmap cropBmp = null;
			var selection = new Rectangle(x1, y1, x2, y2);
            using (Bitmap bmp = image as Bitmap)
			{
				//var bmp = image as Bitmap;

				// Check if it is a bitmap:
				if (bmp == null)
					throw new ArgumentException("No valid bitmap");

				// Crop the image:
				
				try
				{
					cropBmp = bmp.Clone(selection, bmp.PixelFormat);
				}
				catch (Exception)
				{

				}
			}

			// Release the resources:
			image.Dispose();

			return cropBmp;
		}

		public static Stream ToStream(this Image image, ImageFormat format)
		{
			var stream = new System.IO.MemoryStream();
			image.Save(stream, format);
			stream.Position = 0;
			return stream;
		}
	}
}