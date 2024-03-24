using System.Web;
using System.Web.Mvc;

namespace DBSD.CW2._12882._14757._13372
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
