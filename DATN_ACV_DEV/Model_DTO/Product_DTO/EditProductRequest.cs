using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.Product_DTO
{
    public class EditProductRequest : CreateProductRequest
    {
        public Guid ID { get; set; }
    }
}
