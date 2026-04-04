using System;

namespace tthu3.Models
{
    public class ThanhToan
    {
        public string MaThanhToan { get; set; }
        public string MaHopDong { get; set; }     // nếu thanh toán liên quan hợp đồng
        public string MaHoaDon { get; set; }      // nếu thanh toán cho hóa đơn điện/nước
        public string MaPhong { get; set; }       // tham chiếu nhanh
        public string TenantId { get; set; }      // liên kết với UserAccount.TenantId (nếu có)
        public decimal SoTien { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public string Loai { get; set; }          // "Tiền phòng", "Điện", "Nước", "Khác"
        public string GhiChu { get; set; }
    }
}