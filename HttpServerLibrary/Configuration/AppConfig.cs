using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HttpServerLibrary.Configuration
{
    public sealed class AppConfig
    {
        private static AppConfig instance;

        [JsonConstructor]
        static AppConfig()
        {
            if (File.Exists(FILE_NAME))
            {
                var configFile = File.ReadAllTextAsync(FILE_NAME);
                var file = File.ReadAllText(FILE_NAME);
                instance = JsonSerializer.Deserialize<AppConfig>(file)!;
            }
            else
            {
                Console.WriteLine($"Файл настроек {FILE_NAME} не найден");
                instance = new AppConfig();
            }
        }

        public static AppConfig GetInstance()
        {
            if (instance == null)
                instance = new AppConfig();
            return instance;
        }

        public const string FILE_NAME = "appconfig.json";

        public string? Domain { get; set; } = string.Empty;

        public uint Port { get; set; }

        public string? StaticDirectoryPath { get; set; } = string.Empty;

        public SmtpClient? SmtpClient { get; set; }

        public NetworkCredential? NetworkCredential { get; set; }

        public string ConnectionString { get; set; } = string.Empty;
    }
}
