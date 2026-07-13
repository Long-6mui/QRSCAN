using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QRSCAN.Areas.Admin.Controllers
{
    [Area("Admin")] 
    [Authorize]     
    public class BaseAdminController : Controller
    {
    }
}