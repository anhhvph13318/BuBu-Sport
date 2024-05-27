namespace DATN_ACV_DEV.FileBase
{
    public class Message
    {
        public string Code { get; set; }

        public string MessageText { get; set; }

        public string Field { get; set; }
        public static Message CreateErrorMessage(string apiCode, string apiCondition, string messageText, string field)
        {
            return new Message
            {
                Code = "E." + apiCode + "." + apiCondition,
                MessageText = messageText,
                Field = field
            };
        }
    }
}
