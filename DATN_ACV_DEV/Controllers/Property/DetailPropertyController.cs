using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Account_DTO;
using DATN_ACV_DEV.Model_DTO.Property_DTO;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers.Property
{
    [Route("api/DetailProperty")]
    [ApiController]
    public class DetailPropertyController : ControllerBase, IBaseController<DetailPropertyRequest, DetailPropertyResponse>
    {
        private readonly DBContext _context;
        private DetailPropertyRequest _request;
        private readonly IMapper _mapper;
        private BaseResponse<DetailPropertyResponse> _res;
        private DetailPropertyResponse _response;
        private string _apiCode = "DetailProperty";
        private TbProperty _Property;
        public DetailPropertyController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DetailPropertyResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _Property = new TbProperty();
            _response = new DetailPropertyResponse();
        }
        public void AccessDatabase()
        {

            try
            {
                _Property = _context.TbProperties.Where(c => c.Id == _request.Id).FirstOrDefault();
                if (_Property != null)
                {
                    _response.Id = _Property.Id;
                    _response.Name = _Property.Name;
                    _response.ProductId = _Property.ProductId;
                    _response.CategoryId = _Property.CategoryId;
                }
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
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
        public BaseResponse<DetailPropertyResponse> Process(DetailPropertyRequest request)
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
