using System.Web;
using System.Web.Optimization;

namespace MContract
{
	public class BundleConfig
	{
		// Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/js/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/js/jquery.validate*"));

			// Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
			// готово к выпуску, используйте средство сборки по адресу https://modernizr.com, чтобы выбрать только необходимые тесты.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/js/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/js/bootstrap.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap.min.css"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/css/layout.css"));
		}
	}
}
