using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using tthu3.Models;

namespace tthu3.Controllers
{
    [Authorize]
    public class HopDongController : Controller
    {
        private static readonly List<HopDong> _hopDongs = new()
        {
            new HopDong { MaHopDong = "HD001", MaPhong = "P101", TenantCCCD = "012345678", NgayBatDau = new DateTime(2024,1,1), GiaThueThang = 3000000, TienCoc = 3000000, TrangThai = "Đang thuê" }
        };

        // Build view models by joining tenant info
        private List<HopDongViewModel> BuildViewModels(IEnumerable<HopDong> list)
        {
            return list.Select(h =>
            {
                var tenant = NguoiThueController.GetByCCCD(h.TenantCCCD);
                return new HopDongViewModel
                {
                    HopDong = h,
                    TenantName = tenant?.HoTen ?? "-",
                    TenantPhone = tenant?.SoDienThoai ?? "-"
                };
            }).ToList();
        }

        // Index: show all or filter by tenantCCCD (query string "tenant")
        public IActionResult Index(string tenant = null)
        {
            var source = string.IsNullOrEmpty(tenant)
                ? _hopDongs
                : _hopDongs.Where(h => string.Equals(h.TenantCCCD, tenant, StringComparison.OrdinalIgnoreCase)).ToList();

            var vm = BuildViewModels(source);
            ViewBag.TenantFilter = tenant;
            return View(vm);
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var item = _hopDongs.FirstOrDefault(h => h.MaHopDong == id);
            if (item == null) return NotFound();

            var tenant = NguoiThueController.GetByCCCD(item.TenantCCCD);
            var vm = new HopDongViewModel
            {
                HopDong = item,
                TenantName = tenant?.HoTen ?? "-",
                TenantPhone = tenant?.SoDienThoai ?? "-"
            };

            return View(vm);
        }

        // Only Admin can create/edit/delete/end
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Provide a default model so required fields (e.g. NgayBatDau) are prefilled
            var model = new HopDong
            {
                NgayBatDau = DateTime.Today,
                TrangThai = "Đang thuê"
            };
            return View(model);
        }

        // Modified: accept tenantName and tenantPhone; upsert tenant into NguoiThue list after creating contract
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(HopDong model, string tenantName, string tenantPhone)
        {
            // Ensure a sensible start date if browser didn't post a value
            if (model.NgayBatDau == default) model.NgayBatDau = DateTime.Today;

            // Re-validate the model after we fixed any defaults
            ModelState.Remove(nameof(model.NgayBatDau));
            TryValidateModel(model);

            // TenantCCC is required for linking/upsert
            if (string.IsNullOrWhiteSpace(model.TenantCCCD))
            {
                ModelState.AddModelError(nameof(model.TenantCCCD), "CCCD người thuê là bắt buộc.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TenantName = tenantName;
                ViewBag.TenantPhone = tenantPhone;
                return View(model);
            }

            if (_hopDongs.Any(h => h.MaHopDong == model.MaHopDong))
            {
                ModelState.AddModelError(nameof(model.MaHopDong), "Mã hợp đồng đã tồn tại.");
                ViewBag.TenantName = tenantName;
                ViewBag.TenantPhone = tenantPhone;
                return View(model);
            }

            // Upsert tenant into NguoiThueController.NguoiThues (demo in-memory store)
            var existingTenant = NguoiThueController.NguoiThues.FirstOrDefault(t => t.CCCD == model.TenantCCCD);
            if (existingTenant == null)
            {
                var newTenant = new NguoiThue
                {
                    HoTen = string.IsNullOrWhiteSpace(tenantName) ? "Khách thuê" : tenantName,
                    SoDienThoai = tenantPhone ?? string.Empty,
                    CCCD = model.TenantCCCD,
                    DiaChi = string.Empty
                };
                NguoiThueController.NguoiThues.Add(newTenant);
            }
            else
            {
                // Update name/phone if provided
                if (!string.IsNullOrWhiteSpace(tenantName)) existingTenant.HoTen = tenantName;
                if (!string.IsNullOrWhiteSpace(tenantPhone)) existingTenant.SoDienThoai = tenantPhone;
            }

            model.TrangThai ??= "Đang thuê";
            _hopDongs.Add(model);
            TempData["Message"] = "Tạo hợp đồng thành công.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var item = _hopDongs.FirstOrDefault(h => h.MaHopDong == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id, HopDong model)
        {
            if (id != model.MaHopDong) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var existing = _hopDongs.FirstOrDefault(h => h.MaHopDong == id);
            if (existing == null) return NotFound();

            existing.MaPhong = model.MaPhong;
            existing.TenantCCCD = model.TenantCCCD;
            existing.NgayBatDau = model.NgayBatDau;
            existing.NgayKetThuc = model.NgayKetThuc;
            existing.GiaThueThang = model.GiaThueThang;
            existing.TienCoc = model.TienCoc;
            existing.TrangThai = model.TrangThai;

            TempData["Message"] = "Cập nhật hợp đồng thành công.";
            return RedirectToAction(nameof(Index));
        }

        // End contract (sets NgayKetThuc and updates TrangThai)
        [Authorize(Roles = "Admin")]
        public IActionResult End(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var item = _hopDongs.FirstOrDefault(h => h.MaHopDong == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("End")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult EndConfirmed(string id, DateTime? ngayKetThuc)
        {
            var existing = _hopDongs.FirstOrDefault(h => h.MaHopDong == id);
            if (existing == null) return NotFound();

            existing.NgayKetThuc = ngayKetThuc ?? DateTime.UtcNow;
            existing.TrangThai = "Đã kết thúc";

            TempData["Message"] = $"Hợp đồng {id} đã được kết thúc.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var item = _hopDongs.FirstOrDefault(h => h.MaHopDong == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(string id)
        {
            var item = _hopDongs.FirstOrDefault(h => h.MaHopDong == id);
            if (item != null) _hopDongs.Remove(item);
            TempData["Message"] = "Xóa hợp đồng thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}