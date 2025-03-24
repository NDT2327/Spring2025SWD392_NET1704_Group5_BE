using System.Text.Json.Serialization;

namespace CCSystem.Presentation.Helpers
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }//khi co loi
        public List<ErrorMessage> Messages { get; set; }
        public string SuccessMessage { get; set; }
        public T Data { get; set; }

        public string GetError() => Messages?.FirstOrDefault()?.DescriptionErrors?.FirstOrDefault() ?? "Unknown error";
    }

    public class ErrorMessage
    {
        public string FieldNameError { get; set; }
        public List<string> DescriptionErrors { get; set; }
    }
}
