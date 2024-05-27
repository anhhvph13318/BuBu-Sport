using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Supplier;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Supplier
{
    public class DetailSupplierController : ControllerBase, IBaseController<DetailSupplierRequest, DetailSupplierResponse>
    {
        private readonly DBContext _context;
        private DetailSupplierRequest _request;
        private BaseResponse<DetailSupplierResponse> _res;
        private DetailSupplierResponse _response;
        private string _apiCode = "DetailCustomer";
        private TbSupplier _Supplier;
        public DetailSupplierController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DetailSupplierResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _Supplier = new TbSupplier();
            _response = new DetailSupplierResponse();
        }
        public void AccessDatabase()
        {
            try
            {
                _Supplier = _context.TbSuppliers.Where(p => p.Id == _request.Id).FirstOrDefault();
                if (_Supplier != null)
                {
                    _response.Id = _Supplier.Id;
                    _response.PhoneNumber = _Supplier.PhoneNumber;
                    _response.ProvideProducst = _Supplier.ProvideProducst;
                    _response.Name = _Supplier.Name;
                    _response.Adress = _Supplier.Adress;
                }
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            throw new NotImplementedException();
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<DetailSupplierResponse> Process(DetailSupplierRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
                //PreValidation();
                //GenerateObjects();
                //PostValidation();
                AccessDatabase();
            }
            catch (ACV_Exception ex)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
                _res.Messages = ex.Messages;
            }
            catch (System.Exception ex)
            {
                _res.Status = StatusCodes.Status500InternalServerError.ToString();
                _res.Messages.Add(Message.CreateErrorMessage(_apiCode, _res.Status, ex.Message, string.Empty));
            }
            return _res;
        }
    }
}
