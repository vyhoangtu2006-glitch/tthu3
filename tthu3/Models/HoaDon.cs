using System;

    namespace tthu3.Models
{
    public class HoaDon
    {
        // Mã hóa đơn (tùy chọn)
        public string MaHoaDon { get; set; }

        // Chỉ số điện
        public int SoDienCu { get; set; }
        public int SoDienMoi { get; set; }

        // Chỉ số nước
        public int SoNuocCu { get; set; }
        public int SoNuocMoi { get; set; }

        // Tiêu thụ (tính toán, luôn >= 0)
        public int SoDienTieuThu => Math.Max(0, SoDienMoi - SoDienCu);
        public int SoNuocTieuThu => Math.Max(0, SoNuocMoi - SoNuocCu);

        /// <summary>
        /// Tính tổng tiền phải trả = giá phòng + (điện tiêu thụ * đơn giá điện) + (nước tiêu thụ * đơn giá nước).
        /// donGiaDien và donGiaNuoc có giá trị mặc định (VND / đơn vị) — thay đổi khi cần.
        /// </summary>
        public decimal TinhTongTien(decimal giaPhong, decimal donGiaDien = 3000m, decimal donGiaNuoc = 20000m)
        {
            if (SoDienMoi < SoDienCu)
            {
                throw new ArgumentException("Số điện mới phải lớn hơn hoặc bằng số điện cũ.", nameof(SoDienMoi));
            }

            if (SoNuocMoi < SoNuocCu)
            {
                throw new ArgumentException("Số nước mới phải lớn hơn hoặc bằng số nước cũ.", nameof(SoNuocMoi));
            }

            decimal tienDien = (decimal)SoDienTieuThu * donGiaDien;
            decimal tienNuoc = (decimal)SoNuocTieuThu * donGiaNuoc;

            return giaPhong + tienDien + tienNuoc;
        }

        /// <summary>
        /// Tiện lợi: tính tổng tiền dựa trên đối tượng PhongTro (sử dụng PhongTro.GiaPhong).
        /// </summary>
        public decimal TinhTongTien(PhongTro phong, decimal donGiaDien = 3000m, decimal donGiaNuoc = 20000m)
        {
            if (phong == null) throw new ArgumentNullException(nameof(phong));
            return TinhTongTien(phong.GiaPhong, donGiaDien, donGiaNuoc);
        }
    }
}
