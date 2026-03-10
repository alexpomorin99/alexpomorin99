using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Npgsql;

namespace OpenAIPostgresLog {
    class Program {
        const string ConnString = "Host=localhost;Port=5432;Username=postgres;Password=YOUR_PASSWORD;Database=ai_log_db";

        static async Task Main() {
            Console.WriteLine("AI assistant + PostgreSQL logging");
            await EnsureSchema();
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

                await LogInteraction(user, body);
            }
        }

        static async Task EnsureSchema() {
            await using var con = new NpgsqlConnection(ConnString);
            await con.OpenAsync();
            var sql = @"CREATE TABLE IF NOT EXISTS ai_logs (
                id SERIAL PRIMARY KEY,
                user_prompt TEXT NOT NULL,
                raw_response TEXT NOT NULL,
                created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
            );";
            await using var cmd = new NpgsqlCommand(sql, con);
            await cmd.ExecuteNonQueryAsync();
        }

        static async Task LogInteraction(string prompt, string response) {
            await using var con = new NpgsqlConnection(ConnString);
            await con.OpenAsync();
            await using var cmd = new NpgsqlCommand(
                "INSERT INTO ai_logs (user_prompt, raw_response) VALUES (@p, @r);", con);
            cmd.Parameters.AddWithValue("p", prompt);
            cmd.Parameters.AddWithValue("r", response);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
