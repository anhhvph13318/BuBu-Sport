using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Invoice_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Invoice
{
    [Route("api/DetailInvoice")]
    [ApiController]
    public class DetailInvoiceController : ControllerBase, IBaseController<DetailInvoiceRequest, DetailInvoiceResponse>
    {
        private readonly DBContext _context;

        private DetailInvoiceRequest _request;
        private BaseResponse<DetailInvoiceResponse> _res;
        private DetailInvoiceResponse _response;
        private string _apiCode = "DetailInvoice";
        public DetailInvoiceController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DetailInvoiceResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new DetailInvoiceResponse();
        }
        public void AccessDatabase()
        {
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            try
            {
                var invoice = _context.TbInvoices.Where(p => p.Id == _request.Id && p.IsDelete == false).FirstOrDefault();
                if (invoice != null)
                {
                    _response.Id = invoice.Id;
                    _response.Code = invoice.Code;
                    _response.InputDate = invoice.InputDate;
                    var lstInvoiceProduct = _context.TbInvoiceDetails.Where(i => i.IdInvoice == invoice.Id)
                        .Select(s => new DetailInvoiceProduct
                        {
                            NameProduct = s.ProductName,
                            Quantity = s.Quantity,
                            Price = s.Price,
                            Unit = s.Unit,
                            SupplierId = s.SupplierId
                        });
                    var supplier = _context.TbSuppliers.Where(c => lstInvoiceProduct.Select(a => a.SupplierId).Contains(c.Id)).Distinct();
                    lstInvoiceProduct.ToList().ForEach(p =>
                    {
                        p.NameSupplier = supplier.Where(s => s.Id == p.SupplierId).Select(e => e.Name).FirstOrDefault() ?? "";
                    });
                    _response.InvoidProducts = lstInvoiceProduct.ToList();
                }
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
        }

        public void PreValidation()
        {

        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<DetailInvoiceResponse> Process(DetailInvoiceRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                //PreValidation();
                GenerateObjects();
                //PostValidation();
                AccessDatabase();
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
            return _res; ;
        }
    }
}
