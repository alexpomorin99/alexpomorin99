using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AIAgentConsoleOpenAI {
    class Program {
        static async Task Main() {
            Console.WriteLine("AI agent: simple assistant via OpenAI API (chat)");
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "YOUR_API_KEY_HERE";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            while (true) {
                Console.Write("You: ");
                var user = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(user) || user == "/exit") break;

                var payload = new {
                    model = "gpt-4.1-mini",
                    messages = new object[] {
                        new { role = "system", content = "You are a helpful assistant for CRM and Telegram bots." },
                        new { role = "user", content = user }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var resp = await client.PostAsync("https://api.openai.com/v1/chat/completions",
                    new StringContent(json, Encoding.UTF8, "application/json"));
                var body = await resp.Content.ReadAsStringAsync();
                Console.WriteLine("Raw response: ");
                Console.WriteLine(body);
            }
        }
    }
}
