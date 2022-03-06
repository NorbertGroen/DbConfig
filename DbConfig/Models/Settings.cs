namespace DbConfig.Models
{
    public class Settings
    {
        public Settings(string id, string value)
        {
            Id = id;
            Value = value;
        }
        public string Id { get; set; }
        public string Value { get; set; }
    }
    //public record Settings(string Id, string Value); //vanaf C# 9.0
}