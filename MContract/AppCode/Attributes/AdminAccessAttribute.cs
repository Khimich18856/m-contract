using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace MContract.AppCode
{
	public sealed class AdminAccessAttribute : FilterAttribute, IActionFilter
	{
		public void OnActionExecuted(ActionExecutedContext filterContext)
		{ }

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var currentUserId = SM.CurrentUserId;
			if (currentUserId == 0)
			{
				var routeDictionary = new RouteValueDictionary
				{
					{"action", "Forbidden"},
					{"controller", "Errors"}
				};

				filterContext.Result = new RedirectToRouteResult(routeDictionary);
			}
		}
	}
}