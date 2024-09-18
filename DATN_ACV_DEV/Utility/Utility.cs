namespace DATN_ACV_DEV.Utility
{
    public class Utility
    {
        public const int SHOP_ID_SHIP = 4694595;
        public const int SHOP_ID = 190763;
        public const string tokenGHN = "47ca4b17-af64-11ee-8bfa-8a2dda8ec551";
        public const string FROM_NAME = "Shop đồ điện tử ACV";
        public const string FROM_PHONE = "0988119262";
        public const string FROM_ADDRESS = "72 Thành Thái, Phường 14, Quận 10, Hồ Chí Minh, Vietnam";
        public const string FROM_WARD_NAME = "Phường 14";
        public const string FROM_DISTRIC_NAME = "Quận 10";
        public const string FROM_PROVICE_NAME = "HCM";
        public const string RETURN_PHONE = "0988119262";
        public const string RETURN_ADDRESS = "ngõ 85 xuân thủy cầu giấy hà nội";
        public const string CODE_SUPPER_ADMIN = "SUPERADMIN";


        public const int SERVICE_TYPE_DEFAULT = 2;
        public const int FORM_DISTRICT_ID_DEFAULT = 3440;
        public const int ORDER_STATUS_WAIT_CONFIRM = 0;
        public const string ORDER_STATUS_WAIT_CONFIRM_TEXT = "Chờ xác nhận";
        public const int ORDER_STATUS_PREPARE_GOODS = 1;// chuẩn bị hàng
        public const string ORDER_STATUS_PREPARE_GOODS_TEXT = "Chuẩn bị hàng";
        public const int ORDER_STATUS_SHIPPED = 2; // gửi hàng 
        public const string ORDER_STATUS_SHIPPED_TEXT = "Đang vận chuyển"; 
        public const int ORDER_STATUS_CANCELLED = 3; // đã hủy đơn
        public const string ORDER_STATUS_CANCELLED_TEXT = "Đã hủy"; 
        public const int ORDER_STATUS_RETURNS_PRODUCT = 4;// trả hàng 
        public const string ORDER_STATUS_RETURNS_PRODUCT_TEXT = "Đã hoàn hàng";
        public const int ORDER_STATUS_PARTIAL_REFUND = 5;// trả hàng 1 phần 
        public const string ORDER_STATUS_PARTIAL_REFUND_TEXT = "Trả hàng 1 phần";
        public const int ORDER_DURING_THE_RETURN_PERIOD = 6;// hóa đơn mới tạo, trong thời gian đổi trả.
        public const string ORDER_DURING_THE_RETURN_PERIOD_TEXT = "Trong thời gian đổi trả";
        public const int ORDER_STATUS_DONE = 7;// giao thành công.
        public const string ORDER_STATUS_DONE_TEXT = "Hoàn thành đơn hàng";
        public const int LimitDefault = 15;
        public const string Status_Category_Active = "Đang hoạt động";
        public const string Status_Category_No_Active = "Đang hoạt động";

        public const string LOGIN_FAIL = "Tài khoản không tồn tại. Vui lòng đăng ký tài khoản mới !!! ";
        public const string LOGIN_DONE = "Đăng nhập thành công";
        public const string LOGIN_SECRETKEY = "DATN_ACV_SECRETKEY_TOKEN";
        public const string LOGIN_CUSTOMER_SECRETKEY = "CUSTOMER_SECRETKEY_TOKEN";

        public const string CUSTOMER_ACTIVE = "Active";

        public const string PRODUCT_NOTFOUND = "Sản phẩm không tồn tại hoặc số lượng không đủ. Vui lòng đổi sản phẩm khác hoặc giảm số lượng";
        public const string PRODUCT_QUANTITY_IS_NOT_ENOUGH = "Số lượng sản phẩm không đủ.";


        public const string ORDER_CARTDETAIL_NOTFOUND = "Bạn cần chọn sản phẩm để tạo hóa đơn !";
        public const string QUANTITY_VOUCHER_EXCEES_REGULATIONS = "Số lượng voucher không được lớn hơn 2";
        public const string VOUCHER_NOTFOUND = "Voucher không tồn tại hoặc số lượng còn lại không đủ.";
        public const string VOUCHER_OF_THE_TYPE_SAME = "Voucher không được cùng loại";
        public const string VOUCHER_DO_NOT_APPLY_ORDER = "Voucher không phải là voucher giảm giá";
        public const string VOUCHER_CAN_PRO_OR_CATE = "Voucher phải được gán sản phẩm hoặc danh mục ";

        public const string VOUCHER_FREESHIP = "free ship";
        public const string VOUCHER_DISCOUNT = "discount";

        public const string CART_ITEM_NOTFOUND = "Số lượng sản phẩm không đủ.";

        public const string ADDRESS_DELIVERY_NOTFOUND = "Bận chưa có địa chỉ nhận hàng, vui lòng thêm địa chỉ nhận hàng.";
        public const string ORDER_NOTFOUND = "Đơn hàng không tồn tại";
        public const string ORDER_STATUS_EXIST = "Trạng thái của hóa đơn không được phép sửa. ";
        public const string CREATE_ORDER_GHN_STATUS_EXIST = "Trạng thái đơn hàng không hợp lệ.";

        public const string VOUCHER_CODE_DUPLICATE = "mã voucher đã tồn tại.";
        public const string VOUCHER_CODE_LENGHT = "mã voucher không được dài quá 10 ký tự.";
        public const string VOUCHER_END_DATE_EXITS = "ngày kết thúc không được là ngày quá khứ.";
        public const string VOUCHER_UNIT_EXITS = "đơn vị giảm giá không hợp lệ.";
        public const string VOUCHER_DONT_APPLY_PRODUCT = "voucher không được áp dụng cho những sản phẩm trong hóa đơn mã : ";
    }
}
