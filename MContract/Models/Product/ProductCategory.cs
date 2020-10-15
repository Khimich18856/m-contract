using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class ProductCategory
	{
		public int Id { get; set; }

		public string Name { get; set; }

		/// <summary>
		/// Уровень вложенности, только 3 возможных значения: 1 - корневые категории, 2 - дочки корневых, 3 - дочки уровня 2
		/// </summary>
		public int Level { get; set; }

		public int ParentId { get; set; }

		// вычисляется, не хранится в БД
		public List<int> ChildrenId { get; set; } = new List<int>();
		public List<ProductCategory> ChildCategories { get; set; }

		public ProductCategory Clone()
		{
			var result = new ProductCategory()
			{
				Id = Id,
				Name = Name,
				Level = Level,
				ParentId = ParentId,
				ChildrenId = ChildrenId
			};

			return result;
		}
	}
}