namespace DATN_ACV_DEV.Model_DTO.Supplier
{
    public class GetListSupplierResponse
    {
        public GetListSupplierResponse()
        {
            LstSupplier = new List<SupplierDTO> { };
        }
        public List<SupplierDTO> LstSupplier { get; set; }
        public int TotalCount { get; set; }
    }
    public class SupplierDTO
    {
        public Guid Id { get; set; }
        public string? PhoneNumber { get; set; }

        public string? ProvideProducst { get; set; }

        public string? Name { get; set; }

        public string? Adress { get; set; }
    }
}
