using AutoMapper;
using Azure;
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
    [Route("api/DetailPolicy")]
    [ApiController]
    public class DetailPolicyController : ControllerBase, IBaseController<DetailPolicyRequest, DetailPolicyResponse>
    {
        private readonly DBContext _context;
        private DetailPolicyRequest _request;
        private BaseResponse<DetailPolicyResponse> _res;
        private DetailPolicyResponse _response;
        private string _apiCode = "DetailPolicy";
        private TbPolicy _Policy;
        public DetailPolicyController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DetailPolicyResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new DetailPolicyResponse();
        }

        public void AccessDatabase()
        {
            _Policy = _context.TbPolicies.Where(c => c.Id == _request.ID && c.EndDate >= DateTime.Now).FirstOrDefault();
            var Image = _context.TbImages.Where(i => i.Id == _Policy.ImageId).FirstOrDefault();
            if (_Policy != null)
            {
                _response.policy.ID = _Policy.Id;
                _response.policy.Name = _Policy.Name;
                _response.policy.Description = _Policy.Description;
                _response.policy.Status = _Policy.Status;
                _response.policy.StartDate = _Policy.StartDate;
                _response.policy.EndDate = _Policy.EndDate;
                _response.policy.Type = _Policy.Type;
                _response.policy.Image = Image.Url;
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
        public BaseResponse<DetailPolicyResponse> Process(DetailPolicyRequest request)
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
