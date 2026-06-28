using Microsoft.AspNetCore.Mvc;

namespace QRSCAN.Controllers
{
    public class QRController : Controller
    {
        public IActionResult Scan(int maBan)
        {
            HttpContext.Session.SetInt32("MaBan", maBan);

            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Register", "Account");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}