namespace GUI.FileBase
{
    public class Alert
    {
        public static class SweetAlertHelper
        {
            public static string ShowMessage(string title, string message, string type)
            {
                return $"swal('{title}', '{message}', '{type}');";
            }

            public static string ShowSuccess(string title, string message)
            {
                return ShowMessage(title, message, "success");
            }

            public static string ShowError(string title, string message)
            {
                return ShowMessage(title, message, "error");
            }
        }
    }
}
