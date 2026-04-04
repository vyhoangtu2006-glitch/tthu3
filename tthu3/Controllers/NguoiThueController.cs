using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using tthu3.Models;

namespace tthu3.Controllers
{
    [Authorize]
    public class NguoiThueController : Controller
    {
        // Made public so other controllers can lookup tenants in this demo app.
        public static readonly List<NguoiThue> NguoiThues = new()
        {
            new NguoiThue { HoTen = "Nguyễn Văn A", SoDienThoai = "0912345678", CCCD = "012345678", DiaChi = "Quận 1" },
            new NguoiThue { HoTen = "Trần Thị B", SoDienThoai = "0987654321", CCCD = "987654321", DiaChi = "Quận 3" }
        };

        // Helper for lookup by CCCD (demo-only)
        public static NguoiThue GetByCCCD(string cccd) => NguoiThues.FirstOrDefault(t => t.CCCD == cccd);

        // List: all authenticated users can view; consider restricting for Tenant later
        public IActionResult Index()
        {
            return View(NguoiThues);
        }

        // Details
        public IActionResult Details(string id)
        {
            // Use phone or CCCD as identifier; here we use CCCD
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var item = NguoiThues.FirstOrDefault(t => t.CCCD == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // Only Admin creates tenants
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(NguoiThue model)
        {
            if (!ModelState.IsValid) return View(model);
            NguoiThues.Add(model);
            TempData["Message"] = "Thêm người thuê thành công.";
            return RedirectToAction(nameof(Index));
        }

        // Only Admin edits
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var item = NguoiThues.FirstOrDefault(t => t.CCCD == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id, NguoiThue model)
        {
            if (id != model.CCCD) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var existing = NguoiThues.FirstOrDefault(t => t.CCCD == id);
            if (existing == null) return NotFound();

            existing.HoTen = model.HoTen;
            existing.SoDienThoai = model.SoDienThoai;
            existing.DiaChi = model.DiaChi;

            TempData["Message"] = "Cập nhật người thuê thành công.";
            return RedirectToAction(nameof(Index));
        }

        // Only Admin deletes
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var item = NguoiThues.FirstOrDefault(t => t.CCCD == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(string id)
        {
            var item = NguoiThues.FirstOrDefault(t => t.CCCD == id);
            if (item != null) NguoiThues.Remove(item);
            TempData["Message"] = "Xóa người thuê thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}