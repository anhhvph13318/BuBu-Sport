using DATN_ACV_DEV.Model_DTO.Invoice;

namespace DATN_ACV_DEV.Model_DTO.Invoice_DTO
{
    public class EditInvoiceResponse : CreateInvoiceResponse
    {
        public Guid Id { get; set; }
    }
}
