using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Invoice
{
    public class CreateInvoiceRequest : BaseRequest
    {       
        public DateTime InputDate { get; set; }
        public string Code { get; set; }
        public List<SupplierProductInvoice> InvoiceProducts { get; set; }
    }
    public class SupplierProductInvoice
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public Guid SupplierId { get; set; }
        public int Quantity { get; set; }
        public decimal price { get; set; }
        public string? Unit {  get; set; }
    }
}
