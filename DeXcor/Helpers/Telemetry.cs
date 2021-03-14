using System;
using System.Runtime.CompilerServices;

namespace DeXcor.Helpers
{
    public static class Telemetry
    {
        public static void LogException(Exception exception, string tag = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1, [CallerMemberName] string memberName = "")
        {
            //var shortFilePath = filePath.Split("DeXcor")?.Last() ?? string.Empty;
            //var logProperties = new Dictionary<string, string>
            //{
            //    {"Tag", tag },
            //    {"Type", exception.GetType().ToString()},
            //    {"Message", exception.Message},

            //    {"LineNumber", lineNumber.ToString() },
            //    {"MemberName", memberName }
            //};
            //if (exception == null)
            //{
            //    Analytics.TrackEvent("Exception is null", logProperties);
            //    return;
            //}
            //Crashes.TrackError(exception, logProperties);
        }
        public static void TrackNavigate(Type sourcePageType, object parameter = null)
        {
            //var param = string.Empty;
            //if (parameter != null)
            //    param = Convert.ToString(parameter);
            //Analytics.TrackEvent("Navigate", new Dictionary<string, string> { { "Type", sourcePageType.FullName }, { "Parameter", param } });
        }
    }
}
