namespace GUI.FileBase
{
    public class ACV_Exception : Exception
    {
        public List<Message> Messages { get; set; }

        public ACV_Exception()
        {
            Messages = new List<Message>();
        }
    }
}
