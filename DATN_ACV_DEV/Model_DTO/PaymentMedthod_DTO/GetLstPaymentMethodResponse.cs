namespace DATN_ACV_DEV.Model_DTO.PaymentMedthod_DTO
{
    public class GetLstPaymentMethodResponse 
    {
        public GetLstPaymentMethodResponse()
        {
            getLstPaymentMethod = new List<PaymentMethodDTO>();
        }
        public List<PaymentMethodDTO> getLstPaymentMethod { get; set; }
    }
    public class PaymentMethodDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? CartNumber { get; set; }
        public string? Type { get; set; }
    }
}
