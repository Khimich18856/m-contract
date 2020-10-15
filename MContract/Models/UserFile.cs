using MContract.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class UserFile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MessageId { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public DateTime Added { get; set; }
        public DateTime? Changed { get; set; }
        public ModerateResults ModerateResult { get; set; }
        public string NameWithExtension
        {
            get
            {
                if (Name != null && Extension != null)
                    return $"{Name}.{Extension}";
                else
                    return "";
            }
        }
        private string RelativePath
        {
            get
            {
                return $"DContent/user-files/{UserId / 1000}/{UserId}/{MessageId}/{NameWithExtension}";
            }
        }

        public string FullPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + RelativePath;
            }
        }

        public string Url
        {
            get
            {
                var param = Changed > Added ? "?p=" + System.Text.RegularExpressions.Regex.Replace(Changed.ToString(), @"\D*", "") : "";

                return C.SiteUrl + RelativePath + param;
            }
        }
    }
}