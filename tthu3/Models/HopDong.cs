using System;

namespace tthu3.Models
{
    public class HopDong
    {
        public string MaHopDong { get; set; }
        public string MaPhong { get; set; }           // liên kết đến PhongTro.MaPhong
        public string TenantCCCD { get; set; }       // liên kết đến NguoiThue.CCCD
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public decimal GiaThueThang { get; set; }    // tiền phòng hàng tháng (có thể khác với PhongTro.GiaPhong)
        public decimal TienCoc { get; set; }
        public string TrangThai { get; set; }        // Ví dụ: "Đang thuê", "Đã kết thúc"
    }
}