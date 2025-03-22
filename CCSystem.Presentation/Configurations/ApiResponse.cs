using System.Text.Json.Serialization;

namespace CCSystem.Presentation.Configurations
{
	public class ApiResponse<T>
	{
		[JsonPropertyName("message")]
		public string Message { get; set; }
		[JsonPropertyName("data")]
		public List<T> Data { get; set; }
	}
}
