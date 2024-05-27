using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Invoice_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Invoice
{
    [Route("api/EditInvoice")]
    [ApiController]
    public class EditInvoiceController : ControllerBase, IBaseController<EditInvoiceRequest, EditInvoiceResponse>
    {
        private readonly DBContext _context;

        private EditInvoiceRequest _request;
        private BaseResponse<EditInvoiceResponse> _res;
        private EditInvoiceResponse _response;
        private string _apiCode = "EditInvoice";
        private TbInvoice _Invoice;
        public EditInvoiceController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<EditInvoiceResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new EditInvoiceResponse();
        }
        public void AccessDatabase()
        {
            _context.SaveChanges();
            _response.Id = _Invoice.Id;
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
                _Invoice = _context.TbInvoices.Where(p => p.Id == _request.Id).FirstOrDefault();
                if (_Invoice != null)
                {
                    _Invoice.Code = _request.Code ?? _Invoice.Code;
                    _Invoice.InputDate = _request.InputDate ?? _Invoice.InputDate;
                    _Invoice.UpdateBy = _request.AdminId ?? Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993");//Guid của 1 tài khoản có trong DB
                    _Invoice.UpdateDate = DateTime.Now; // Ngày hiện tại 
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
        public BaseResponse<EditInvoiceResponse> Process(EditInvoiceRequest request)
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
