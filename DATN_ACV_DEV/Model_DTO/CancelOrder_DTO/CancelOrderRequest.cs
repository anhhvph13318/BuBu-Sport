namespace DATN_ACV_DEV.Model_DTO.CancelOrder_DTO
{
    public class CancelOrderRequest
    {
        public Guid Id { get; set; }
        public string ReasonCancel { get; set; }
        public int? Action { get; set; }
        public List<string>? UrlImage { get; set; }
        public string? TypeImage { get; set; }
    }
}
