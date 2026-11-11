using FluentEmail.Core;

namespace newproj.Services.Emails;

public class EmailService(IFluentEmail email) : IEmailService
{
    private readonly IFluentEmail _email = email;

    public async Task SendAsync(string to, string subject, string body)
    {
        var response = await _email.To(to).Subject(subject).Body(body, true).SendAsync();
        if (!response.Successful)
        {
            Console.WriteLine($"Failed to send email to {to}: {string.Join("; ", response.ErrorMessages)}");
            throw new Exception("Failed to send invitation email.");
        }
    }
}