using System;

namespace tthu3.Models
{
    public class ChiPhi
    {
        public string MaChiPhi { get; set; }
        public string MaHopDong { get; set; }      // liên kết tới hợp đồng
        public string TenChiPhi { get; set; }      // Ví dụ: "Sửa chữa", "Internet"
        public decimal SoTien { get; set; }
        public DateTime NgayPhatSinh { get; set; }
        public string GhiChu { get; set; }
    }
}