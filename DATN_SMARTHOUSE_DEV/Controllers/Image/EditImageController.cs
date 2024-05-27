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
    [Route("api/EditImage")]
    [ApiController]
    public class EditImageController : ControllerBase, IBaseController<EditImageRequest, EditImageResponse>
    {
        private readonly DBContext _context;
        private EditImageRequest _request;
        private BaseResponse<EditImageResponse> _res;
        private EditImageResponse _response;
        private string _apiCode = "EditImage";
        private TbImage _Image;
        public EditImageController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<EditImageResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new EditImageResponse();
        }

        public void AccessDatabase()
        {
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
            try
            {
                _Image = _context.TbImages.Where(x => x.Id == _request.ID && x.InAcitve == true).FirstOrDefault();
                if (_Image != null)
                {
                    _Image.Url = _request.Url;
                    _Image.Type = _request.Type;
                    _Image.InAcitve = _request.InAcitve.HasValue ? _request.InAcitve : _Image.InAcitve;
                }
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<EditImageResponse> Process(EditImageRequest request)
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
