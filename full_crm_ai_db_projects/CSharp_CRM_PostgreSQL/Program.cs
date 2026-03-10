using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace CSharpCrmPostgres {
    class Lead {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "New";
    }

    class Program {
        const string ConnString = "Host=localhost;Port=5432;Username=postgres;Password=YOUR_PASSWORD;Database=crm_db";

        static async Task Main() {
            Console.WriteLine("CRM + PostgreSQL demo");
            await EnsureSchema();

            while (true) {
                Console.WriteLine("
1) Add lead  2) List leads  3) Change status  0) Exit");
                Console.Write("Choice: ");
                var key = Console.ReadLine();
                if (key == "0") break;
                switch (key) {
                    case "1": await AddLead(); break;
                    case "2": await ListLeads(); break;
                    case "3": await ChangeStatus(); break;
                    default: Console.WriteLine("Unknown command"); break;
                }
            }
        }

        static async Task EnsureSchema() {
            await using var con = new NpgsqlConnection(ConnString);
            await con.OpenAsync();
            var sql = @"CREATE TABLE IF NOT EXISTS leads (
                id SERIAL PRIMARY KEY,
                name TEXT NOT NULL,
                status TEXT NOT NULL DEFAULT 'New'
            );";
            await using var cmd = new NpgsqlCommand(sql, con);
            await cmd.ExecuteNonQueryAsync();
        }

        static async Task AddLead() {
            Console.Write("Client name: ");
            var name = Console.ReadLine() ?? string.Empty;
            await using var con = new NpgsqlConnection(ConnString);
            await con.OpenAsync();
            await using var cmd = new NpgsqlCommand("INSERT INTO leads (name) VALUES (@name);", con);
            cmd.Parameters.AddWithValue("name", name);
            await cmd.ExecuteNonQueryAsync();
            Console.WriteLine("Lead added.");
        }

        static async Task ListLeads() {
            await using var con = new NpgsqlConnection(ConnString);
            await con.OpenAsync();
            await using var cmd = new NpgsqlCommand("SELECT id, name, status FROM leads ORDER BY id", con);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) { Console.WriteLine("No leads."); return; }
            while (await reader.ReadAsync()) {
                Console.WriteLine($"#{reader.GetInt32(0)}: {reader.GetString(1)} [{reader.GetString(2)}]");
            }
        }

        static async Task ChangeStatus() {
            Console.Write("Lead id: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) return;
            Console.Write("New status (New/In progress/Won/Lost): ");
            var status = Console.ReadLine() ?? "New";
            await using var con = new NpgsqlConnection(ConnString);
            await con.OpenAsync();
            await using var cmd = new NpgsqlCommand("UPDATE leads SET status=@status WHERE id=@id", con);
            cmd.Parameters.AddWithValue("status", status);
            cmd.Parameters.AddWithValue("id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            Console.WriteLine(rows > 0 ? "Status updated." : "Lead not found.");
        }
    }
}
