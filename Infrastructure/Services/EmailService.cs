using Application.Interfaces;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        // Här simuleras mailutskicket
        Console.WriteLine("================ EMAIL SKICKAT ================");
        Console.WriteLine($"TILL: {to}");
        Console.WriteLine($"ÄMNE: {subject}");
        Console.WriteLine($"MEDDELANDE: {body}");
        Console.WriteLine("===============================================");

        return Task.CompletedTask;
    }
}