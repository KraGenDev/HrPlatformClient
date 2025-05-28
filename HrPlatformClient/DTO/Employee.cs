using Newtonsoft.Json;

namespace HrPlatformClient.DTO
{
    public class Employee
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("department")]
        public string Department { get;set;}
        [JsonProperty("position")]
        public string Position { get; set; }
    }
}