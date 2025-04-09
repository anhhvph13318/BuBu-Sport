using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.ProductDetail_DTO;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DATN_ACV_DEV.Controllers.ProductDetail
{
	[Route("api/GetListProductDetail")]
	[ApiController]
	public class GetListProductDetailController : ControllerBase, IBaseController<GetListProductDetailRequest, GetListProductDetailResponse>
	{
		private readonly DBContext _context;
		private GetListProductDetailRequest _request;
		private BaseResponse<GetListProductDetailResponse> _res;
		private GetListProductDetailResponse _response;
		private string _apiCode = "GetListProductDetail";
		private TbProductDetail _ProductDetail;
		private TbProduct _Product;
		private List<TbProductDetail> _ProductDetails;
		public GetListProductDetailController(DBContext context) 
		{
			_context = context;
			_res = new BaseResponse<GetListProductDetailResponse>()
			{
				Status = StatusCodes.Status200OK.ToString(),
				Data = null
			};
			_response = new GetListProductDetailResponse();
		}
		public void AccessDatabase()
		{
			throw new NotImplementedException();
		}

		public void CheckAuthorization()
		{
			throw new NotImplementedException();
		}

		public void GenerateObjects()
		{
			_ProductDetails = _context.TbProductDetails.Where(c=>c.Product.Name.Contains(_request.Name)).OrderByDescending(c=>c.CreateDate).ToList();
			var query = _ProductDetails
				.Select(a => new ProductDetailDTO
				{
					Id = a.ProductId,
					Code = _context.TbProducts.Where(c=>c.Id == a.ProductId).Select(c=>c.Code).FirstOrDefault(),
					Image = _context.TbImages.Where(d=>d.Id == _context.TbProductDetails.Where(c=>c.Id == a.Id).Select(c=>c.ImageId).FirstOrDefault()).Select(d=>d.Url).FirstOrDefault(),
					Name = _context.TbProducts.Where(c => c.Id == a.ProductId).Select(c => c.Name).FirstOrDefault(),
					Color = _context.TbColors.Where(d=>d.Id == _context.TbProductDetails.Where(c => c.Id == a.Id).Select(c => c.ColorId).FirstOrDefault()).Select(d=>d.Name).FirstOrDefault(),
					Size = _context.TbSizes.Where(d => d.Id == _context.TbProductDetails.Where(c => c.Id == a.Id).Select(c => c.SizeId).FirstOrDefault()).Select(d => d.SizeName).FirstOrDefault(),
					Price = _context.TbProducts.Where(c => c.Id == a.ProductId).Select(c => c.Price).FirstOrDefault(),
					Quantity = 1,
					// Thêm các thuộc tính khác nếu ProductDetailDTO có
				})
				.ToList();
			_response.LstProduct = query;
			_response.TotalCount = _ProductDetails.Count;
		_res.Data = _response;
		}

		public void PreValidation()
		{
			throw new NotImplementedException();
		}
		[HttpPost]
		[Route("Process")]
		public BaseResponse<GetListProductDetailResponse> Process(GetListProductDetailRequest request)
		{
			try
			{
				_request = request;
				//CheckAuthorization();
				//PreValidation();
				GenerateObjects();
				//PostValidation();
				//AccessDatabase();
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
