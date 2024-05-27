namespace DATN_ACV_DEV.Model_DTO.Invoice_DTO
{
    public class DetailInvoiceResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public DateTime InputDate { get; set; }
        public List<DetailInvoiceProduct> InvoidProducts { get; set; }

    }
    public class DetailInvoiceProduct
    {
        public string? NameProduct { get; set; }
        public int? Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal Price { get; set; }
        public Guid? SupplierId { get; set; }
        public string NameSupplier { get; set; }

    }
}
