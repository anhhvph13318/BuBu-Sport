using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Image
{
    [Route("api/DetailImage")]
    [ApiController]
    public class DetailImageController : ControllerBase, IBaseController<DetailImageRequest, DetailImageResponse>
    {
        private readonly DBContext _context;
        private DetailImageRequest _request;
        private BaseResponse<DetailImageResponse> _res;
        private DetailImageResponse _response;
        private string _apiCode = "DetailImage";
        private TbImage _Image;
        public DetailImageController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DetailImageResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new DetailImageResponse();
        }
        public void AccessDatabase()
        {
            _Image = _context.TbImages.Where(c => c.Id == _request.Id && c.InAcitve == false).FirstOrDefault();
            if (_Image != null)
            {

                _response.Id = _Image.Id;
                _response.Url = _Image.Url;
                _response.Type = _Image.Type;
                _response.InAcitve = _Image.InAcitve;
                _response.ProductId = _Image.ProductId;
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
        public BaseResponse<DetailImageResponse> Process(DetailImageRequest request)
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
