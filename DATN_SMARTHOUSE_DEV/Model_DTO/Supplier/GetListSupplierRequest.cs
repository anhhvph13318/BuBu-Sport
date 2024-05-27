using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Supplier
{
    public class GetListSupplierRequest:BaseRequest
    {
        public string? PhoneNumber { get; set; }

        public string? ProvideProducst { get; set; }

        public string? Name { get; set; }

        public string? Adress { get; set; }
    }
}
