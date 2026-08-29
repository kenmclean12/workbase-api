using newproj.DTO.Client;
using newproj.DTO.Request;
using newproj.DTO.User;
using newproj.Models.Clients;
using newproj.Models.Requests;
using newproj.Models.Users;

namespace newproj.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Clients.Any() || db.Users.Any() || db.Requests.Any())
        {
            return;
        }

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

        var seededUsers = new List<User>();
        for (var i = 1; i <= 100; i++)
        {
            var firstName = new[]
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
            }[i % 20];

            var lastName = new[]
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
            }[i % 20];

            var role = i % 10 == 0
                ? UserRole.SUPERADMIN
                : i % 5 == 0
                    ? UserRole.ADMIN
                    : i % 3 == 0
                        ? UserRole.STANDARD
                        : UserRole.READONLY;

            var user = User.Create(new UserCreateDTO
            {
                FirstName = firstName,
                LastName = lastName,
                Email = $"user{i}@workbase.test",
                Password = "Password123!",
                Role = role,
                ClientId = clients[(i - 1) % clients.Count].Id,
            });

            seededUsers.Add(user);
        }

        db.Users.AddRange(seededUsers);
        db.SaveChanges();

        var requests = new List<Request>();
        for (var i = 1; i <= 100; i++)
        {
            var clientId = clients[(i - 1) % clients.Count].Id;
            var createdByUserId = seededUsers[(i - 1) % seededUsers.Count].Id;
            var assignedUserId = seededUsers[(i * 3 + 1) % seededUsers.Count].Id;

            var request = Request.Create(new RequestCreateDTO
            {
                ClientId = clientId,
                CreatedByUserId = createdByUserId,
                AssignedToUserId = assignedUserId,
                Title = $"Request {i}: {GetRequestTitle(i)}",
                Description = $"Seeded request {i} for client {clientId}. This is a generated record used for testing the WorkBase request board and workflow.",
                Status = (RequestStatus)((i - 1) % 3 + 1),
                Priority = i % 4 == 0 ? RequestPriority.HIGH : i % 2 == 0 ? RequestPriority.MEDIUM : RequestPriority.LOW,
                Internal = i % 5 == 0,
            });

            requests.Add(request);
        }

        db.Requests.AddRange(requests);
        db.SaveChanges();
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
