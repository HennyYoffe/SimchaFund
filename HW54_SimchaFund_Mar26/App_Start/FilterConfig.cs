using System.Web;
using System.Web.Mvc;

namespace HW54_SimchaFund_Mar26
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
