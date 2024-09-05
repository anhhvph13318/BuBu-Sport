namespace GUI.Controllers.Shared;
public class SweetAlertHelper
{
    public static string ShowMessage(string messageCaption, string messageContent, SweetAlertMessageType messageType)
    {
        var fire = $"swal('{messageCaption}','{messageContent}','{messageType}');";
        return fire;
    }
}

public enum SweetAlertMessageType
{
    warning, error, success, info
}
