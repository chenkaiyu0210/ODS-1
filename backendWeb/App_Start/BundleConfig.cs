using System.Web;
using System.Web.Optimization;

namespace backendWeb
{
    public class BundleConfig
    {
        // 如需統合的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region OLD
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            //// 準備好可進行生產時，請使用 https://modernizr.com 的建置工具，只挑選您需要的測試。
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));


            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));
            #endregion

            //Script
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/ThirdParty/sbAdmin2/vendor/jquery/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/ThirdParty/sbAdmin2/vendor/bootstrap/js/bootstrap.min.js",
                        "~/ThirdParty/sbAdmin2/vendor/bootstrap/js/bootstrap.bundle.min.js"));

            bundles.Add(new ScriptBundle("~/sbAdmin2/js").Include(
                     "~/ThirdParty/sbAdmin2/js/sb-admin-2.min.js"));

            bundles.Add(new ScriptBundle("~/sbAdmin2/vendorJs").Include(
                         "~/ThirdParty/sbAdmin2/vendor/chart.js/Chart.min.js",
                         "~/ThirdParty/sbAdmin2/vendor/datatables/jquery.dataTables.min.js",
                         "~/ThirdParty/sbAdmin2/vendor/datatables/dataTables.bootstrap4.min.js",
                         "~/ThirdParty/sbAdmin2/vendor/chart.js/Chart.min.js",
                         "~/ThirdParty/sbAdmin2/vendor/jquery-easing/jquery.easing.min.js"
                         ));

            //Css
            bundles.Add(new StyleBundle("~/sbAdmin2/css").Include(
                     "~/ThirdParty/sbAdmin2/css/sb-admin-2.min.css"));

            bundles.Add(new StyleBundle("~/sbAdmin2/vendorCss").Include(                   
                    "~/ThirdParty/sbAdmin2/vendor/fontawesome-free/css/all.min.css",
                    "~/ThirdParty/sbAdmin2/vendor/fontawesome-free/css/brands.min.css",
                    "~/ThirdParty/sbAdmin2/vendor/fontawesome-free/css/fontawesome.min.css",
                    "~/ThirdParty/sbAdmin2/vendor/fontawesome-free/css/regular.min.css",
                    "~/ThirdParty/sbAdmin2/vendor/fontawesome-free/css/solid.min.css",
                    "~/ThirdParty/sbAdmin2/vendor/fontawesome-free/css/svg-with-js.min.css",
                    "~/ThirdParty/sbAdmin2/vendor/fontawesome-free/css/v4-shims.min.css"
                    ));



        }
    }
}
