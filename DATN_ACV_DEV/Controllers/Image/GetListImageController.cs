using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Invoice_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Image
{
    [Route("api/GetListImage")]
    [ApiController]
    public class GetListImageController : ControllerBase, IBaseController<GetListImageRequest, GetListImageResponse>
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private GetListImageRequest _request;
        private BaseResponse<GetListImageResponse> _res;
        private GetListImageResponse _response;
        private string _apiCode = "GetListImage";
        public GetListImageController(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _res = new BaseResponse<GetListImageResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null,
            };
            _response = new GetListImageResponse();
        }
        public void AccessDatabase()
        {
            List<GetListImageDTO> LstImage = new List<GetListImageDTO>();
            var model = _context.TbImages.Where(c => _request.ProductId.HasValue ? c.ProductId == _request.ProductId : true
                       && (!string.IsNullOrEmpty(_request.Url) ? c.Url.Contains(_request.Url) : true)
                       && (!string.IsNullOrEmpty(_request.Type) ? c.Type.Contains(_request.Type) : true)
                        ).OrderByDescending(d => d.CreateDate);
            _response.TotalCount = model.Count();
            var query = _mapper.Map<List<GetListImageDTO>>(model);
            if (_request.Limit == null)
            {
                LstImage = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            }
            else
            {
                LstImage = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            }
            _response.LstImage = LstImage;
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
        public BaseResponse<GetListImageResponse> Process(GetListImageRequest request)
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
