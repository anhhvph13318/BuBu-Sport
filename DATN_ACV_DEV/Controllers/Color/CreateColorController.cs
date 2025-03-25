using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Color_DTO;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Color
{
    [Route("api/CreateColor")]
    [ApiController]
    public class CreateColorController : ControllerBase, IBaseController<CreateColorRequest, CreateColorResponse>
    {
        private readonly DBContext _context;
        private CreateColorRequest _request;
        private BaseResponse<CreateColorResponse> _res;
        private CreateImageRequest _requestImage = new CreateImageRequest();
        private CreateColorResponse _response;
        private string _apiCode = "CreateColor";
        private TbColor _Color;
        public CreateColorController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateColorResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateColorResponse();
        }

        public void AccessDatabase()
        {
            _context.Add(_Color);
            _context.SaveChanges();
            _response.ID = _Color.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            _Color = new TbColor()
            {
                Id = Guid.NewGuid(),
                Name = _request.Name,
                Status = 1,
                //Default
                CreateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"), // Tạm thời gán guid khởi tạo.
                CreateDate = DateTime.Now, // Ngày hiện tại 
            };  
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateColorResponse> Process(CreateColorRequest request)
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
