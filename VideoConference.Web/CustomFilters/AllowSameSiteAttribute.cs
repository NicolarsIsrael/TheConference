using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.CustomFilters
{
    public class AllowSameSiteAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var response = filterContext.HttpContext.Response;

            if (response != null)
            {
                response.Headers.Add("Set-Cookie", "HttpOnly;Secure;SameSite=Strict");
                //Add more headers...
            }

            base.OnActionExecuting(filterContext);
        }
    }

    //public class CheckIfCompanyIsSetup : ActionFilterAttribute, IActionFilter
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        var controllersUsingThisAttribute = ((StoreController)filterContext.Controller);
    //        var admin = controllersUsingThisAttribute.GetCompanyAdmin();
    //        if (admin == null)
    //        {
    //            filterContext.Result = new RedirectResult("/Store/SetUpCompany");
    //        }
    //        else
    //        {
    //            if (admin.Company == null)
    //                filterContext.Result = new RedirectResult("/Store/SetUpCompany");
    //        }



    //        base.OnActionExecuting(filterContext);
    //    }
    //}
}
