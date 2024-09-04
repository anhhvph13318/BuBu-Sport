namespace DATN_ACV_DEV.FileBase
{
    public class BaseResponse<T>
    {
        public string Status { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();

        public T Data { get; set; }

        public BaseResponse()
        {
            Messages = new List<Message>();
        }
    }
}
