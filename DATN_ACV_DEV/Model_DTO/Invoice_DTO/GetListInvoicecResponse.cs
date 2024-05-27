namespace DATN_ACV_DEV.Model_DTO.Invoice_DTO
{
    public class GetListInvoicecResponse
    {
        public GetListInvoicecResponse()
        {
            LstInvoice = new List<InvoiceDTO> { };

        }
        public List<InvoiceDTO> LstInvoice { get; set; }
        public int TotalCount { get; set; }
    }
    public class InvoiceDTO
    {
        public Guid Id { get; set; }    
        public string Code { get; set; }
        public DateTime InputDate { get; set; }

    }
}
