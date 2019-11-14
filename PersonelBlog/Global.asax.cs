using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace PersonelBlog
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var lang = "en-EN"; // Default dil
            var cookie = Request.Cookies["SelectedLanguage"];
            if (cookie != null && cookie.Value != null)
            {
                lang = cookie.Value;
            }
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
        }

    }
}
