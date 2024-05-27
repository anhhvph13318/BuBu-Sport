using AutoMapper;
using DATN_ACV_DEV.Controllers;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.Model_DTO.Account_DTO;
using DATN_ACV_DEV.Model_DTO.AddressDelivery;
using DATN_ACV_DEV.Model_DTO.Cart_DTO;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using DATN_ACV_DEV.Model_DTO.GroupCustomer_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using DATN_ACV_DEV.Model_DTO.Invoice_DTO;
using DATN_ACV_DEV.Model_DTO.PaymentMedthod_DTO;
using DATN_ACV_DEV.Model_DTO.Policy_DTO;
using DATN_ACV_DEV.Model_DTO.Supplier;
using DATN_ACV_DEV.Model_DTO.User_DTO;
using DATN_ACV_DEV.Model_DTO.Voucher_DTO;
using DATN_ACV_DEV.Model_DTO.Property_DTO;
using DATN_ACV_DEV.Utility;
using DATN_ACV_DEV.Model_DTO.Image_DTO;

namespace DATN_ACV_DEV
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TbProduct, HomePageModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.PriceNet, opt => opt.MapFrom(src => src.PriceNet))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.Status == 1 ? "Hoạt động" : "Ngưng hoạt động"))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.tb_Image.Url));

            CreateMap<TbVoucher, VoucherDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.product.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.category.Name));
            /*.ForMember(dest => dest.GroupCustomerName, opt => opt.MapFrom(src => src.groupCustomer.Name))*/

            CreateMap<TbCategory, CategoryDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == 1 ? "Hoạt động" : "Ngưng hoạt động"))
               .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image.Url));

            CreateMap<TbPolicy, PolicyDTO>()
               .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
               .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
               .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
               .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image.Url));

            CreateMap<TbCartDetail, CartDTO>()
               .ForMember(dest => dest.CartDetailID, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductId))
               .ForMember(dest => dest.NameProduct, opt => opt.MapFrom(src => src.tbProduct.Name))
               .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.tbImage.Url))
               .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.tbProduct.Price));

            CreateMap<TbPaymentMethod, PaymentMethodDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.CartNumber, opt => opt.MapFrom(src => src.CardNumber))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<TbAddressDelivery, AddressDeliveryDTO>()
              .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.address, opt => opt.MapFrom(src => $"{src.WardName}, {src.DistrictName} ,{src.ProvinceName}"))
              .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.Status == true ? "Mặc định" : "không mặc định"));



            CreateMap<TbUser, UserDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
               .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.UserCode))
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position));
            CreateMap<TbGroupCustomer, GroupCustomerDTO>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.MinPoint, opt => opt.MapFrom(src => src.MinPoint))
             .ForMember(dest => dest.MaxPoint, opt => opt.MapFrom(src => src.MaxPoint));

            CreateMap<TbCustomer, CustomerDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Adress, opt => opt.MapFrom(src => src.Adress))
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Rank))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.YearOfBirth, opt => opt.MapFrom(src => src.YearOfBirth))
                .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Sex))
                .ForMember(dest => dest.Point, opt => opt.MapFrom(src => src.Point));

            CreateMap<TbAccount, AccountDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.AccountCode, opt => opt.MapFrom(src => src.AccountCode));

            CreateMap<TbSupplier, SupplierDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.Adress, opt => opt.MapFrom(src => src.Adress))
               .ForMember(dest => dest.ProvideProducst, opt => opt.MapFrom(src => src.ProvideProducst));
            CreateMap<TbInvoice, InvoiceDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
               .ForMember(dest => dest.InputDate, opt => opt.MapFrom(src => src.InputDate));

            CreateMap<TbUser, UserDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
               .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.UserCode))
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position));
            CreateMap<TbGroupCustomer, GroupCustomerDTO>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.MinPoint, opt => opt.MapFrom(src => src.MinPoint))
             .ForMember(dest => dest.MaxPoint, opt => opt.MapFrom(src => src.MaxPoint));

            CreateMap<TbCustomer, CustomerDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Adress, opt => opt.MapFrom(src => src.Adress))
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Rank))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.YearOfBirth, opt => opt.MapFrom(src => src.YearOfBirth))
                .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Sex))
                .ForMember(dest => dest.Point, opt => opt.MapFrom(src => src.Point));

            CreateMap<TbAccount, AccountDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.AccountCode, opt => opt.MapFrom(src => src.AccountCode));

            CreateMap<TbSupplier, SupplierDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.Adress, opt => opt.MapFrom(src => src.Adress))
               .ForMember(dest => dest.ProvideProducst, opt => opt.MapFrom(src => src.ProvideProducst));
            //.ForMember(dest => dest.status, opt => opt.MapFrom(src => src.Status == true ? "Mặc định" : "không mặc định"))
            //.ForMember(dest => dest.receiverName, opt => opt.MapFrom(src => src.ReceiverName))
            //.ForMember(dest => dest.receiverphone, opt => opt.MapFrom(src => src.ReceiverPhone));


            CreateMap<TbOrder, GetListOrderAdminDTO>()
               .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.code, opt => opt.MapFrom(src => src.OrderCode))
               .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dest => dest.status, opt => opt.MapFrom(src => Common.ConvertStatusOrder(src.Status.Value)))
               .ForMember(dest => dest.amountShip, opt => opt.MapFrom(src => src.AmountShip))
               .ForMember(dest => dest.amountDiscount, opt => opt.MapFrom(src => src.TotalAmountDiscount))
               .ForMember(dest => dest.totalAmount, opt => opt.MapFrom(src => src.TotalAmount))
               .ForMember(dest => dest.nameCustomer, opt => opt.MapFrom(src => src.customer.Name))
               .ForMember(dest => dest.paymentMethodName, opt => opt.MapFrom(src => src.paymentMethod.Name))
               .ForMember(dest => dest.products, opt => opt.MapFrom(src => string.Join(",", src.orderDetail.Select(x => x.Product.Name).ToList())));

            CreateMap<TbProperty, GetListPropertyDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
               .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));
            CreateMap<TbImage, GetListImageDTO>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
             .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
             .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
             .ForMember(dest => dest.InAcitve, opt => opt.MapFrom(src => src.InAcitve));

        }
    }
}
