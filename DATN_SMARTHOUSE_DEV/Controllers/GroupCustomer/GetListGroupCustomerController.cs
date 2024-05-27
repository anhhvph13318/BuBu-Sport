using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.GroupCustomer_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.GroupCustomer
{
    [Route("api/GetListGroupCustomer")] 
    [ApiController]
    public class GetListGroupCustomerController : ControllerBase, IBaseController<GetListGroupCustomerRequest, GetListGroupCustomerResponse>
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private GetListGroupCustomerRequest _request;
        private BaseResponse<GetListGroupCustomerResponse> _res;
        private GetListGroupCustomerResponse _response;
        private string _apiCode = "GetListGroupCustomer";
        public GetListGroupCustomerController(DBContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _res = new BaseResponse<GetListGroupCustomerResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetListGroupCustomerResponse();
        }
        public void AccessDatabase()
        {
            List<GroupCustomerDTO> LstGroupCustomer = new List<GroupCustomerDTO>();
            var Model = _context.TbGroupCustomers.Where(c => c.IsDelete == false 
            && (!string.IsNullOrEmpty(_request.Name) ? c.Name.Contains(_request.Name) : true));
            _response.TotalCount = Model.Count();
            var query = _mapper.Map<List<GroupCustomerDTO>>(Model);
            if (_request.Limit == null)
            {
                LstGroupCustomer = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            }
            else
            {
                LstGroupCustomer = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            }
            _response.LstGroupCustomer = LstGroupCustomer;
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
        public BaseResponse<GetListGroupCustomerResponse> Process(GetListGroupCustomerRequest request)
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
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
            return _res;
        }
    }
}
