using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace tthu3.Controllers
{
    [Authorize] // require authentication for all actions in this controller
    public class PhongTroController : Controller
    {
        // Dữ liệu mẫu (Trong thực tế sẽ lấy từ SQL Server)
        private static List<PhongTro> danhSachPhong = new List<PhongTro>
        {
            new PhongTro { MaPhong="P101", TenPhong="Phòng 101", GiaPhong=3000000, DienTich=25, DaChoThue=false },
            new PhongTro { MaPhong="P102", TenPhong="Phòng 102", GiaPhong=3500000, DienTich=30, DaChoThue=true }
        };

        // All authenticated users can see the list
        public IActionResult Index()
        {
            return View(danhSachPhong);
        }

        // Only Admin can access Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(PhongTro phong)
        {
            danhSachPhong.Add(phong);
            return RedirectToAction("Index");
        }
    }
}