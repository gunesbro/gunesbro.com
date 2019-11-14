using PersonelBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PersonelBlog
{
    public class UserAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Request.Cookies["userAccess"] != null)
            {
                return true;
            }
            else
            {
                httpContext.Response.Redirect("/Account/Login");
                return false;
            }
        }
    }
}