using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CCSystem.Presentation.Helpers
{
    public class ToastHelper
    {
        private const int DefaultDuration = 5000;

        public enum ToastType
        {
            Success,
            Error, 
            Warning,
            Info
        }

        public static void Show(ITempDataDictionary tempData, string message, ToastType type = ToastType.Info, int duration = DefaultDuration, string title = null)
        {
            tempData["ToastMessage"] = message;
            tempData["ToastType"] =     type.ToString().ToLower();
            tempData["ToastDuration"] = duration;
            if (!string.IsNullOrEmpty(title))
            {
                tempData["ToastTitle"] = title;
            }
        }

        // Success
        public static void ShowSuccess(ITempDataDictionary tempData, string message,
            int duration = DefaultDuration, string title = "Success")
        {
            Show(tempData, message, ToastType.Success, duration, title);
        }

        //  Error
        public static void ShowError(ITempDataDictionary tempData, string message,
            int duration = DefaultDuration, string title = "Error")
        {
            Show(tempData, message, ToastType.Error, duration, title);
        }

        // Warning
        public static void ShowWarning(ITempDataDictionary tempData, string message,
            int duration = DefaultDuration, string title = "Warning")
        {
            Show(tempData, message, ToastType.Warning, duration, title);
        }

        // Info
        public static void ShowInfo(ITempDataDictionary tempData, string message,
            int duration = DefaultDuration, string title = "Info")
        {
            Show(tempData, message, ToastType.Info, duration, title);
        }
    }
}
