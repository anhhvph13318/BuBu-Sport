namespace GUI.Models.DTOs.Order_DTO
{
    public static class OrderUtil
    {
        public static IDictionary<int, string> OrderStepProcessing = new Dictionary<int, string>()
        {
            { 0, "Chờ xác nhận" },
            { 1, "Chuẩn bị hàng" },
            { 2, "Đang giao hàng" },
            { 7, "Hoàn thành" }
        };
    }
}
