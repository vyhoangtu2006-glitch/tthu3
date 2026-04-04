using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using tthu3.Models;

namespace tthu3.Controllers
{
    [Authorize]
    public class ThanhToanController : Controller
    {
        private static readonly List<ThanhToan> _thanhToans = new()
        {
            new ThanhToan { MaThanhToan = "TT001", MaHopDong = "HD001", MaPhong = "P101", TenantId = "T001", SoTien = 3000000, NgayThanhToan = DateTime.UtcNow.AddDays(-10), Loai = "Tiền phòng", GhiChu = "Thanh toán tháng 1" }
        };

        // Index: Admin sees all, Tenant sees only their own payments
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(_thanhToans);
            }

            // Tenant: filter by TenantId claim
            var tenantId = User.FindFirst("TenantId")?.Value;
            if (string.IsNullOrEmpty(tenantId))
            {
                return Forbid();
            }

            var mine = _thanhToans.Where(t => t.TenantId == tenantId).ToList();
            return View(mine);
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var item = _thanhToans.FirstOrDefault(t => t.MaThanhToan == id);
            if (item == null) return NotFound();

            // Tenant may only view their own record
            if (!User.IsInRole("Admin"))
            {
                var tenantId = User.FindFirst("TenantId")?.Value;
                if (tenantId == null || item.TenantId != tenantId) return Forbid();
            }

            return View(item);
        }

        // Admin creates payment records (or record created after tenant pays)
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(ThanhToan model)
        {
            if (!ModelState.IsValid) return View(model);
            if (_thanhToans.Any(t => t.MaThanhToan == model.MaThanhToan))
            {
                ModelState.AddModelError(nameof(model.MaThanhToan), "Mã thanh toán đã tồn tại.");
                return View(model);
            }

            _thanhToans.Add(model);
            TempData["Message"] = "Tạo bản ghi thanh toán thành công.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var item = _thanhToans.FirstOrDefault(t => t.MaThanhToan == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id, ThanhToan model)
        {
            if (id != model.MaThanhToan) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var existing = _thanhToans.FirstOrDefault(t => t.MaThanhToan == id);
            if (existing == null) return NotFound();

            existing.MaHopDong = model.MaHopDong;
            existing.MaHoaDon = model.MaHoaDon;
            existing.MaPhong = model.MaPhong;
            existing.TenantId = model.TenantId;
            existing.SoTien = model.SoTien;
            existing.NgayThanhToan = model.NgayThanhToan;
            existing.Loai = model.Loai;
            existing.GhiChu = model.GhiChu;

            TempData["Message"] = "Cập nhật thanh toán thành công.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var item = _thanhToans.FirstOrDefault(t => t.MaThanhToan == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(string id)
        {
            var item = _thanhToans.FirstOrDefault(t => t.MaThanhToan == id);
            if (item != null) _thanhToans.Remove(item);
            TempData["Message"] = "Xóa thanh toán thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}