using Newtonsoft.Json;

namespace HrPlatformClient.DTO
{
    public record DepartmentDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
