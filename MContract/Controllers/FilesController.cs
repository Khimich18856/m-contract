using MContract.AppCode;
using MContract.DAL;
using MContract.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MContract.Controllers
{
    public class FilesController : Controller
    {
        public static string GetFileDirectoryForFiles(UserFile file)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "DContent\\user-files\\" + (file.UserId / 1000) + "\\" + file.UserId + "\\" + file.MessageId + "\\";
        }

        [HttpPost]
        public string UploadFiles(int respondentId)
        {
            var result = "";

            if (Request == null || Request.Files == null || Request.Files.Count == 0)
                return "Не выбрано ни одного файла";

            if (SM.CurrentUserIsNull)
                return "Для загрузки файлов необходимо авторизоваться на сайте";

            var files = new List<UserFile>();
            var userId = SM.CurrentUserId;

            var message = new Message
            {
                SenderId = userId,
                RecipientId = respondentId,
                Text = "Прикрепленные файлы:",
                Files = new List<UserFile>()
            };
            var messageId = UserHelper.AddMessage(message);
            message.Id = messageId;

            foreach (string upload in Request.Files)
            {
                var uploadFile = Request.Files[upload];
                if (uploadFile == null || uploadFile.ContentLength == 0)
                    continue;

                var byteCount = uploadFile.ContentLength;
                if (byteCount > 50 * 1024 * 1024) {
                    result += "error,0,0";
                    continue;
                }

                string filename = uploadFile.FileName;
                var extension = Path.GetExtension(filename);
                if (extension != null)
                {
                    extension = extension.Replace(".", "");
                    filename = filename.Substring(0, filename.IndexOf(extension) - 1);
                }

                var deniedExtensions = new List<string>() { "exe", "js" };
                if (deniedExtensions.Contains(extension))
                    return "Запрещено загружать файлы с расширением " + extension;

                //var isImage = new List<string>() { "jpg", "jpeg", "png" }.Contains(extension);

                var file = new UserFile()
                {
                    UserId = userId,
                    MessageId = 0,
                    Name = filename,
                    Extension = extension,
                    ModerateResult = ModerateResults.NotChecked,
                    Added = DateTime.Now.ToUniversalTime(),
                    Changed = DateTime.Now.ToUniversalTime()
                };
                message.Files.Add(file);
                file.MessageId = messageId;

                try
                {
                    var folder = GetFileDirectoryForFiles(file);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    uploadFile.SaveAs(file.FullPath);
                    files.Add(file);
                    result += $"ok,{file.NameWithExtension},{file.Url}|";
                }
                catch (Exception ex)
                {
                    result = "error";
                    LogsDAL.AddError("in UserController.UploadFile(): userId = " + file.UserId + ", exception: " + ex);
                    return result;
                }
            }

            SM.AddMessageToCurrentDialogs(message);

            FilesDAL.AddFiles(files);

            return result;
        }

        [HttpPost]
        public static bool DeleteFile(int fileId)
        {
            var file = FilesDAL.GetFile(fileId);
            if (file == null)
                return false;
            System.IO.File.Delete(file.FullPath);
            return FilesDAL.DeleteFile(fileId);
        }
    }
}