using GUI.Models.DTOs.Product_DTO;

namespace GUI.Models.DTOs.Product_DTO.Views
{
    public class IndexOject
    {
        public GetListProductResponse Data { get; set; } = new();
        public ProductModel Model { get; set; } = new();
        public string Search { get; set; } = string.Empty;
    }
}
