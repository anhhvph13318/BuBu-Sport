using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Policy_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/GetListPolicy")]
    [ApiController]
    public class GetListPolicyController : ControllerBase, IBaseController<GetListPolicyRequest, GetListPolicyResponse>
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private GetListPolicyRequest _request;
        private BaseResponse<GetListPolicyResponse> _res;
        private GetListPolicyResponse _response;
        private string _apiCode = "GetListPolicy";
        public GetListPolicyController(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _res = new BaseResponse<GetListPolicyResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetListPolicyResponse();
        }

        public void AccessDatabase()
        {
            List<PolicyDTO> LstPolicy = new List<PolicyDTO>();
            var Model = _context.TbPolicies.Where(c => c.EndDate >= DateTime.Now
                        && (!string.IsNullOrEmpty(_request.Name) ? c.Name.Contains(_request.Name) : true)
                        && (!string.IsNullOrEmpty(_request.Status) ? c.Status == _request.Status : true)
                        && (_request.Type.HasValue ? c.Type == _request.Type : true)
                        && (_request.FromDate.HasValue ? c.CreateDate >= _request.FromDate : true)
                        && (_request.ToDate.HasValue ? c.CreateDate <= _request.ToDate : true)
                        ).OrderByDescending(d => d.CreateDate);
            var Image = _context.TbImages.Where(c => Model.Select(a => a.ImageId).Contains(c.Id)).Distinct();
            _response.TotalCount = Model.Count();
            Model.ToList().ForEach(c =>
            {
                c.Image = Image.Where(i => i.Id == c.ImageId).FirstOrDefault();
            });
            var query = _mapper.Map<List<PolicyDTO>>(Model);
            if (_request.Limit == null)
            {
                LstPolicy = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            }
            else
            {
                LstPolicy = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            }
            _response.ListPolicy = LstPolicy;
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
        public BaseResponse<GetListPolicyResponse> Process(GetListPolicyRequest request)
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
