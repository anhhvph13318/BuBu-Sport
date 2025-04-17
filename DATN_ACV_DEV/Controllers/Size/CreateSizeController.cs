using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Color_DTO;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Size_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Size
{
    [Route("api/CreateSize")]
    [ApiController]
    public class CreateSizeController : ControllerBase, IBaseController<CreateSizeRequest, CreateSizeResponse>
    {
        private readonly DBContext _context;
        private CreateSizeRequest _request;
        private BaseResponse<CreateSizeResponse> _res;
        private CreateImageRequest _requestImage = new CreateImageRequest();
        private CreateSizeResponse _response;
        private string _apiCode = "CreateSize";
        private TbSize _Size;
        
        public CreateSizeController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateSizeResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateSizeResponse();
        }
        public void AccessDatabase()
        {
            _context.Add(_Size);
            _context.SaveChanges();
            _response.ID = _Size.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            _Size = new TbSize()
            {
                Id = Guid.NewGuid(),
                SizeName = _request.SizeName,
                FootLength = _request.FootLength,
                Quantity = _request.Quantity != null ? _request.Quantity : null,
                
            };
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateSizeResponse> Process(CreateSizeRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
                //PreValidation();
                GenerateObjects();
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
