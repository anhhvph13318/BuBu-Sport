using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Customer
{
    [Route("api/GetListCustomer")] 
    [ApiController]
    public class GetListCustomerController : ControllerBase, IBaseController<GetListCustomerRequest, GetListCustomerResponse>
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private GetListCustomerRequest _request;
        private BaseResponse<GetListCustomerResponse> _res;
        private GetListCustomerResponse _response;
        private string _apiCode = "GetListCustomer";
        public GetListCustomerController(DBContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _res = new BaseResponse<GetListCustomerResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null,
            };
            _response = new GetListCustomerResponse();
        }
        public void AccessDatabase()
        {
            List<CustomerDTO> LstCustomer = new List<CustomerDTO>();
            var model = _context.TbCustomers.Where(c => (!string.IsNullOrEmpty(_request.Name) ? c.Name.Contains(_request.Name) : true)
                        && (!string.IsNullOrEmpty(_request.Status) ? c.Status == _request.Status : true)
                        && (!string.IsNullOrEmpty(_request.Sex) ? c.Sex.ToString() == _request.Sex : true)
                        && (!string.IsNullOrEmpty(_request.Rank) ? c.Rank.ToString() == _request.Rank : true)
                        ).OrderByDescending(d => d.CreateDate);
            _response.TotalCount = model.Count();
            var query = _mapper.Map<List<CustomerDTO>>(model);
            if (_request.Limit == null)
            {
                LstCustomer = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            }
            else
            {
                LstCustomer = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            }
            _response.LstCustomer = LstCustomer;
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
        public BaseResponse<GetListCustomerResponse> Process(GetListCustomerRequest request)
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
