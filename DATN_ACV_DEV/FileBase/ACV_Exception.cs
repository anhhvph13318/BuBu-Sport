namespace DATN_ACV_DEV.FileBase
{
    public class ACV_Exception : Exception
    {
        public List<Message> Messages { get; set; }

        public ACV_Exception()
        {
            Messages = new List<Message>();
        }
        public ACV_Exception(Message message)
        {
            Messages = new List<Message> { message };
        }
    }
}
