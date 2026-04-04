using System;

namespace tthu3.Models
{
    public class HopDongViewModel
    {
        private string _maHopDong = string.Empty;
        private string _maPhong = string.Empty;
        private string _tenantCCCD = string.Empty;
        private DateTime _ngayBatDau;
        private DateTime? _ngayKetThuc;
        private decimal _giaThueThang;
        private decimal _tienCoc;
        private string _trangThai = "Đang thuê";

        // Preserve original HopDong object for controllers/views that expect it
        public HopDong HopDong { get; set; }

        // Backwards-compatible flattened properties (read/write).
        // If HopDong is provided, getters read from it; setters store locally.
        public string MaHopDong
        {
            get => HopDong?.MaHopDong ?? _maHopDong;
            set => _maHopDong = value;
        }

        public string MaPhong
        {
            get => HopDong?.MaPhong ?? _maPhong;
            set => _maPhong = value;
        }

        public string TenantCCCD
        {
            get => HopDong?.TenantCCCD ?? _tenantCCCD;
            set => _tenantCCCD = value;
        }

        public DateTime NgayBatDau
        {
            get => HopDong?.NgayBatDau ?? _ngayBatDau;
            set => _ngayBatDau = value;
        }

        public DateTime? NgayKetThuc
        {
            get => HopDong?.NgayKetThuc ?? _ngayKetThuc;
            set => _ngayKetThuc = value;
        }

        public decimal GiaThueThang
        {
            get => HopDong?.GiaThueThang ?? _giaThueThang;
            set => _giaThueThang = value;
        }

        public decimal TienCoc
        {
            get => HopDong?.TienCoc ?? _tienCoc;
            set => _tienCoc = value;
        }

        public string TrangThai
        {
            get => HopDong?.TrangThai ?? _trangThai;
            set => _trangThai = value;
        }

        // Additional tenant display fields
        public string TenantName { get; set; } = "-";
        public string TenantPhone { get; set; } = "-";
    }
}
