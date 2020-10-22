using MContract.AppCode;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class Photo
	{
		public int Id { get; set; }
		public Guid GroupId { get; set; }
		public int? UserId { get; set; }
		public int? AdId { get; set; }
		public string FileNameWithExtension { get; set; }
		public bool IsMain { get; set; }
		public ModerateResults ModerateResult { get; set; }
		public DateTime Added { get; set; }
		public DateTime Changed { get; set; }
		public int? Width { get; set; }
		public int? Height { get; set; }
		public PhotoTypes PhotoType { get; set; }

		// вычисляемые поля

		public User User { get; set; }
		public Ad Ad { get; set; }
		private string RelativePath
		{
			get
			{
				if (IsNoLogoPlaceholder)
					
					return "ico/nonePhoto.svg";
				else
					return "DContent/user-photos/" + (UserId / 1000) + "/" + UserId + "/" + (AdId != null ? "Ads/" + AdId + "/" : "Logos/") + FileNameWithExtension;
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

		public string FullPath
		{
			get
			{
				return AppDomain.CurrentDomain.BaseDirectory + RelativePath;
			}
		}

		public int? HigherDimension
		{
			get
			{
				if (Width != null && Height != null)
					return Width > Height ? Width.Value : Height.Value;
				else
					return null;
			}
		}

		//для страницы модерации
		public bool Accepted { get; set; }
		public int? Rotate { get; set; }
		public string Take { get; set; }

		public bool IsNoLogoPlaceholder { get; set; }
	}
}