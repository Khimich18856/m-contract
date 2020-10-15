using MContract.AppCode;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class UnregisteredUser
	{
		public int Id { get; set; }
		public string CompanyName { get; set; }
		public string ContactName { get; set; }
		public string Email { get; set; }
		public TypesOfOwnership TypeOfOwnership { get; set; }
		public int CityId { get; set; }
		public string INN { get; set; }
		public string OGRN { get; set; }
		public string PhoneNumber { get; set; }
		public string PhoneNumberCity { get; set; }
		public DateTime Created { get; set; }

		#region Поля СБИС
		public string SbisCompanyName { get; set; }
		public int? SbisTypeOfOwnershipId { get; set; }
		public string SbisOGRN { get; set; }
		public DateTime? SbisWorksFrom { get; set; }

		#endregion

		//вычисляемые поля:
		public string SbisTypeOfOwnershipStr
		{
			get
			{
				if (!SbisTypeOfOwnershipId.HasValue)
					return String.Empty;

				return UserHelper.GetTypeOfOwnershipString((TypesOfOwnership)SbisTypeOfOwnershipId.Value);
			}
		}

		public string TypeOfOwnershipStr
		{
			get
			{
				return UserHelper.GetTypeOfOwnershipString(TypeOfOwnership);
			}
		}

		public Town Town { get; set; }

		private string _companyNameWithTypeOfOwnership;
		public string CompanyNameWithTypeOfOwnership
		{
			get
			{
				if (_companyNameWithTypeOfOwnership == null)
					_companyNameWithTypeOfOwnership = TypeOfOwnershipStr + " \"" + CompanyName + "\"";

				return _companyNameWithTypeOfOwnership;
			}
			set
			{
				_companyNameWithTypeOfOwnership = value;
			}
		}
	}
}