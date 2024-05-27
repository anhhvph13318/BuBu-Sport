using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Invoice;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Invoice
{
    [Route("api/CreateInvoice")]
    [ApiController]
    public class CreateInvoiceController : ControllerBase, IBaseController<CreateInvoiceRequest, CreateInvoiceResponse>
    {
        private readonly DBContext _context;
        private CreateInvoiceRequest _request;
        private BaseResponse<CreateInvoiceResponse> _res;
        private CreateInvoiceResponse _response;
        private string _apiCode = "CreateInvoice";
        private TbInvoice _Invoice;
        private List<TbInvoiceDetail> _InvoiceDetails;
        private TbProduct _Product;
        public CreateInvoiceController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateInvoiceResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateInvoiceResponse();
            _InvoiceDetails = new List<TbInvoiceDetail>();
        }
        public void AccessDatabase()
        {
            _context.Add(_Invoice);
            _context.AddRange(_InvoiceDetails);
            _context.SaveChanges();
            _response.id = _Invoice.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            _Invoice = new TbInvoice()
            {
                Id = Guid.NewGuid(),
                InputDate = _request.InputDate,
                Code = _request.Code,
                IsDelete = false,
                CreateBy = _request.AdminId ?? Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993")
            };
            foreach (var item in _request.InvoiceProducts)
            {
                _InvoiceDetails.Add(new TbInvoiceDetail()
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.price,
                    Unit = item.Unit,
                    SupplierId = item.SupplierId,
                    IdInvoice = _Invoice.Id
                });
                var product = _context.TbProducts.Where(c => c.Id == item.ProductId).FirstOrDefault();
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                }
            }
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateInvoiceResponse> Process(CreateInvoiceRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                //PreValidation(); // validate dữ liệu 
                GenerateObjects(); // Gán dữ liệu 
                AccessDatabase(); // Lưu xuống DB 

            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
            return _res;
        }
    }
}
