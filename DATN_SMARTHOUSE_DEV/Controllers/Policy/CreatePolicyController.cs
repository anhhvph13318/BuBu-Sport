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
    [Route("api/ CreatePolicy")]
    [ApiController]
    public class CreatePolicyController : ControllerBase, IBaseController<CreatePolicyRequest, CreatePolicyResponse>
    {
        private readonly DBContext _context;
        private CreatePolicyRequest _request;
        private BaseResponse<CreatePolicyResponse> _res;
        private CreatePolicyResponse _response;
        private string _apiCode = " CreatePolicy";
        private TbPolicy _Policy;
        public CreatePolicyController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreatePolicyResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreatePolicyResponse();
        }

        public void AccessDatabase()
        {
            _context.Add(_Policy);
            _context.SaveChanges();
            _response.ID = _Policy.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            _Policy = new TbPolicy()
            {
                Id = Guid.NewGuid(),
                Name = _request.Name,
                Status = _request.Status,
                ImageId = _request.ImageId,
                Description = _request.Description,
                Type = _request.Type,
                StartDate = _request.StartDate,
                EndDate = _request.EndDate,
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
        public BaseResponse<CreatePolicyResponse> Process(CreatePolicyRequest request)
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
