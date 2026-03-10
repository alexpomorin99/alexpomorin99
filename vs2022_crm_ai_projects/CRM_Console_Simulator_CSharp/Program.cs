using System;
using System.Collections.Generic;

namespace CRMConsoleSimulator {
    class Lead {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "New";
    }

    class Program {
        static readonly List<Lead> Leads = new();
        static int _nextId = 1;

        static void Main() {
            Console.WriteLine("Simple CRM console simulator (leads + pipeline)");
            while (true) {
                Console.WriteLine("
1) Add lead  2) List leads  3) Change status  0) Exit");
                Console.Write("Choice: ");
                var key = Console.ReadLine();
                if (key == "0") break;
                switch (key) {
                    case "1": AddLead(); break;
                    case "2": ListLeads(); break;
                    case "3": ChangeStatus(); break;
                    default: Console.WriteLine("Unknown command"); break;
                }
            }
        }

        static void AddLead() {
            Console.Write("Client name: ");
            var name = Console.ReadLine() ?? string.Empty;
            Leads.Add(new Lead { Id = _nextId++, Name = name });
            Console.WriteLine("Lead added.");
        }

        static void ListLeads() {
            if (Leads.Count == 0) { Console.WriteLine("No leads."); return; }
            foreach (var l in Leads)
                Console.WriteLine($"#{l.Id}: {l.Name} [{l.Status}]");
        }

        static void ChangeStatus() {
            Console.Write("Lead id: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) return;
            var lead = Leads.Find(l => l.Id == id);
            if (lead == null) { Console.WriteLine("Lead not found"); return; }
            Console.Write("New status (New/In progress/Won/Lost): ");
            lead.Status = Console.ReadLine() ?? lead.Status;
            Console.WriteLine("Status updated.");
        }
    }
}
