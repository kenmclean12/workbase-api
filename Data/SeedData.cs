using newproj.DTO.Client;
using newproj.DTO.Request;
using newproj.DTO.RequestHistory;
using newproj.DTO.User;
using newproj.Models.Clients;
using newproj.Models.Requests;
using newproj.Models.Users;

namespace newproj.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Clients.Any() && db.Users.Any() && db.Requests.Any())
        {
            return;
        }

        using var transaction = db.Database.BeginTransaction();

        try
        {
            // Clear any partially seeded data from a previous failed seed.
            db.RequestHistory.RemoveRange(db.RequestHistory);
            db.Requests.RemoveRange(db.Requests);
            db.Users.RemoveRange(db.Users);
            db.Clients.RemoveRange(db.Clients);
            db.SaveChanges();

            // -----------------------------------------------------------------
            // Clients
            // -----------------------------------------------------------------

            var clients = Enumerable.Range(1, 100)
                .Select(index => Client.Create(new ClientCreateDTO
                {
                    Name = $"Client {index}",
                    Email = $"client{index}@workbase.test",
                    Phone = $"+1-555-{(1000 + index):D4}",
                    Website = $"https://client{index}.workbase.test",
                    Description = $"Seeded client record for client {index}.",
                }))
                .ToList();

            db.Clients.AddRange(clients);
            db.SaveChanges();

            // -----------------------------------------------------------------
            // Users
            // -----------------------------------------------------------------

            var seededUsers = new List<User>();

            var firstNames = new[]
            {
                "Ava",
                "Liam",
                "Mia",
                "Noah",
                "Emma",
                "Ethan",
                "Sophia",
                "Lucas",
                "Olivia",
                "Mason",
                "Isabella",
                "James",
                "Charlotte",
                "Benjamin",
                "Amelia",
                "Henry",
                "Harper",
                "Elijah",
                "Evelyn",
                "Daniel",
            };

            var lastNames = new[]
            {
                "Smith",
                "Johnson",
                "Brown",
                "Davis",
                "Miller",
                "Wilson",
                "Moore",
                "Taylor",
                "Anderson",
                "Thomas",
                "Jackson",
                "White",
                "Harris",
                "Martin",
                "Thompson",
                "Garcia",
                "Martinez",
                "Robinson",
                "Clark",
                "Rodriguez",
            };

            for (var i = 1; i <= 100; i++)
            {
                var firstName = firstNames[(i - 1) % firstNames.Length];
                var lastName = lastNames[(i - 1) % lastNames.Length];

                var role = i % 10 == 0
                    ? UserRole.SUPERADMIN
                    : i % 5 == 0
                        ? UserRole.ADMIN
                        : i % 3 == 0
                            ? UserRole.STANDARD
                            : UserRole.READONLY;

                // First 50 users have no client.
                // Remaining 50 users all belong to Client 1.
                int? clientId = i <= 50
                    ? null
                    : clients[0].Id;

                var user = User.Create(new UserCreateDTO
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = $"user{i}@workbase.test",
                    Password = "Password123!",
                    Role = role,
                    ClientId = clientId,
                });

                seededUsers.Add(user);
            }

            db.Users.AddRange(seededUsers);
            db.SaveChanges();

            // -----------------------------------------------------------------
            // Requests
            // -----------------------------------------------------------------

            var requests = new List<Request>();

            for (var i = 1; i <= 100; i++)
            {
                var clientId = clients[(i - 1) % clients.Count].Id;
                var createdByUserId = seededUsers[(i - 1) % seededUsers.Count].Id;

                var request = Request.Create(new RequestCreateDTO
                {
                    ClientId = clientId,
                    Title = $"Request {i}: {GetRequestTitle(i)}",
                    Description =
                        $"Seeded request {i} for client {clientId}. " +
                        "This is a generated record used for testing the WorkBase request board and workflow.",
                    Priority = i % 4 == 0
                        ? RequestPriority.HIGH
                        : i % 2 == 0
                            ? RequestPriority.MEDIUM
                            : RequestPriority.LOW,
                }, createdByUserId);

                requests.Add(request);
            }

            db.Requests.AddRange(requests);
            db.SaveChanges();

            // -----------------------------------------------------------------
            // Request History
            // Seed history for Client 4 requests.
            // -----------------------------------------------------------------

            var targetClient = clients[3];

            var client4Requests = db.Requests
                .Where(r => r.ClientId == targetClient.Id)
                .ToList();

            if (client4Requests.Any())
            {
                var historyList = new List<RequestHistory>();

                foreach (var request in client4Requests)
                {
                    historyList.Add(RequestHistory.Create(new RequestHistoryCreateDTO
                    {
                        RequestId = request.Id,
                        UserId = seededUsers[0].Id,
                        Action = "CREATED",
                    }));

                    historyList.Add(RequestHistory.Create(new RequestHistoryCreateDTO
                    {
                        RequestId = request.Id,
                        UserId = seededUsers[1].Id,
                        Action = "STATUS_CHANGED_TO_IN_PROGRESS",
                    }));

                    historyList.Add(RequestHistory.Create(new RequestHistoryCreateDTO
                    {
                        RequestId = request.Id,
                        UserId = seededUsers[2].Id,
                        Action = "PRIORITY_UPDATED",
                    }));
                }

                db.RequestHistory.AddRange(historyList);
                db.SaveChanges();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static string GetRequestTitle(int index)
    {
        return index switch
        {
            1 => "Website refresh",
            2 => "Client onboarding",
            3 => "Billing review",
            4 => "Security audit",
            5 => "Report export",
            6 => "CRM sync",
            7 => "Workflow update",
            8 => "Migration support",
            9 => "Vendor setup",
            10 => "Training request",
            _ => "Operational task",
        };
    }
}
