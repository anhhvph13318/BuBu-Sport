using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/CreateImage")]
    [ApiController]
    public class CreateImageController : ControllerBase, IBaseController<CreateImageRequest, CreateImageResponse>
    {
        private readonly DBContext _context;
        private CreateImageRequest _request;
        private BaseResponse<CreateImageResponse> _res;
        private CreateImageResponse _response;
        private string _apiCode = "CreateImage";
        private TbImage _Image;
        public CreateImageController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateImageResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateImageResponse();
        }

        public void AccessDatabase()
        {
            _context.Add(_Image);
            _context.SaveChanges();
            _response.ID = _Image.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            _Image = new TbImage()
            {
                Id = Guid.NewGuid(),
                Url = _request.Url,
                Type = _request.Type,
                ProductId = _request.ProductId,
                //Default
                InAcitve = false,
                CreateDate = DateTime.Now,
                CreateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"),
            };
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateImageResponse> Process(CreateImageRequest request)
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
