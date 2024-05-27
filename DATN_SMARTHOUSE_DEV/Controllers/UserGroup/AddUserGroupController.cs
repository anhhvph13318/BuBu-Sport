using AutoMapper;
using DATN_ACV_DEV.Controllers.Condition;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Cart_DTO;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using DATN_ACV_DEV.Model_DTO.UserGroup_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/AddUserGroup")]
    [ApiController]
    public class AddUserGroupController : ControllerBase, IBaseController<CreateUserGroupRequest, CreateUserGroupResponse>
    {
        private readonly DBContext _context;
        private CreateUserGroupRequest _request;
        private BaseResponse<CreateUserGroupResponse> _res;
        private CreateUserGroupResponse _response;
        private string _apiCode = "AddUserGroup";
        private TbUserGroup _userGroup;
        private bool checkCart = false;
        private bool checkProduct = false;
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        public AddUserGroupController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateUserGroupResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateUserGroupResponse();
        }

        public void AccessDatabase()
        {
            _context.Add(_userGroup);
            _context.SaveChanges();
            _response.id = _userGroup.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
        }
        public void GenerateObjects()
        {
            _userGroup = new TbUserGroup
            {
                Id = Guid.NewGuid(),
                Name = _request.name,
                Code = _request.code,
                IsDelete = false,
                CreateBy = _request.AdminId,
                CreateDate = DateTime.Now,
            };
        }

        public void PreValidation()
        {
            //#region Sản phẩm không tồn tại 
            //ConditionCart.AddToCart_C01(_context, _request, _apiCode, _conC01, _conC01Field);
            //#endregion

            //#region Số lượng sản phẩm không đủ 
            //ConditionCart.AddToCart_C02(_context, _request, _apiCode, _conC02, _conC02Field);
            //#endregion
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateUserGroupResponse> Process(CreateUserGroupRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
               // PreValidation();
                GenerateObjects();
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
