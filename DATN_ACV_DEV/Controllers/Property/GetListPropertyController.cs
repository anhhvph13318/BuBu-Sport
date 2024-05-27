using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using DATN_ACV_DEV.Model_DTO.Property_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Property
{
    [Route("api/GetListProperty")]
    [ApiController]
    public class GetListPropertyController : ControllerBase, IBaseController<GetListPropertyRequest, GetListPropertyResponse>
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private GetListPropertyRequest _request;
        private BaseResponse<GetListPropertyResponse> _res;
        private GetListPropertyResponse _response;
        private string _apiCode = "GetListProperty";
        public GetListPropertyController(DBContext context,IMapper mapper)
        {
            _context = context;
            _res = new BaseResponse<GetListPropertyResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetListPropertyResponse();
            _mapper = mapper;
        }
        public void AccessDatabase()
        {
            List<GetListPropertyDTO> lstProperty = new List<GetListPropertyDTO>();
            var model = _context.TbProperties.Where(c => c.Id != null
                                                    && (_request.ProductId.HasValue ? c.ProductId == _request.ProductId : true)
                                                    && (_request.CategoryId.HasValue ? c.CategoryId == _request.CategoryId : true)
                                                    && c.Active == true
                                                    && (!string.IsNullOrEmpty(_request.Name) ? _request.Name == c.Name : true))
                                                        .OrderByDescending(d => d.CreateDate);
            _response.TotalCount = model.Count();
            var query = _mapper.Map<List<GetListPropertyDTO>>(model);
            if (_request.Limit == null)
            {
                lstProperty = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            }
            else
            {
                lstProperty = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            }
            _response.Properties = lstProperty;
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
        public BaseResponse<GetListPropertyResponse> Process(GetListPropertyRequest request)
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
            catch (ACV_Exception ex0)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
                _res.Messages = ex0.Messages;
            }
            catch (Exception ex)
            {
                _res.Status = StatusCodes.Status500InternalServerError.ToString();
                _res.Messages.Add(Message.CreateErrorMessage(_apiCode, _res.Status, ex.Message, string.Empty));
            }
            return _res;
        }
    }
}
