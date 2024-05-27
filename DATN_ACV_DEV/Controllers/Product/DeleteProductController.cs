using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/DeleteProduct")]
    [ApiController]
    public class DeleteProductController : ControllerBase, IBaseController<DeleteProduct, DeleteProductRes>
    {
        private readonly DBContext _context;
        private DeleteProduct _request;
        private BaseResponse<DeleteProductRes> _res;
        private DeleteProductRes _response;
        private string _apiCode = "DeleteProduct";
        private TbProduct _Produst;
        public DeleteProductController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DeleteProductRes>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new DeleteProductRes();
        }

        public void AccessDatabase()
        {
            _context.SaveChanges();
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
                _Produst = _context.TbProducts.Where(p => p.Id == _request.ID && p.IsDelete == false).FirstOrDefault();
                if (_Produst != null)
                {
                    _Produst.IsDelete = true;
                    _Produst.UpdateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993");//Guid của 1 tài khoản có trong DB
                    _Produst.UpdateDate = DateTime.Now; // Ngày hiện tại 
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
        public BaseResponse<DeleteProductRes> Process(DeleteProduct request)
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
