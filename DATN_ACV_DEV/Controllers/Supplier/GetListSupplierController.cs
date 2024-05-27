using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.Supplier;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Supplier
{
    [Route("api/GetListSupplier")]
    [ApiController]
    public class GetListSupplierController : ControllerBase, IBaseController<GetListSupplierRequest, GetListSupplierResponse>
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private GetListSupplierRequest _request;
        private BaseResponse<GetListSupplierResponse> _res;
        private GetListSupplierResponse _response;
        private string _apiCode = "GetListSupplier";
        public GetListSupplierController(DBContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _res = new BaseResponse<GetListSupplierResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null,
            };
            _response = new GetListSupplierResponse();
        }
        public void AccessDatabase()
        {
            List<SupplierDTO> LstSupplier = new List<SupplierDTO>();
            var model = _context.TbSuppliers.Where(c => (!string.IsNullOrEmpty(_request.Name) ? c.Name.Contains(_request.Name) : true)
                        && (!string.IsNullOrEmpty(_request.PhoneNumber) ? c.PhoneNumber == _request.PhoneNumber : true)
                        && (!string.IsNullOrEmpty(_request.Adress) ? c.Adress.ToString() == _request.Adress : true)
                        && (!string.IsNullOrEmpty(_request.ProvideProducst) ? c.ProvideProducst.ToString() == _request.ProvideProducst : true)
                        ).OrderByDescending(d => d.CreateDate);
            _response.TotalCount = model.Count();
            var query = _mapper.Map<List<SupplierDTO>>(model);
            if (_request.Limit == null)
            {
                LstSupplier = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            }
            else
            {
                LstSupplier = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            }
            _response.LstSupplier = LstSupplier;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
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
        public BaseResponse<GetListSupplierResponse> Process(GetListSupplierRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                //PreValidation();
                //GenerateObjects();
                //PostValidation();
                AccessDatabase();
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
            return _res;
        }
    }
}
