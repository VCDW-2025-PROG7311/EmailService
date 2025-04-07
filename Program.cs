using SendGrid.Helpers.Mail;
using SendGrid;
using MailKit.Net.Smtp;
using MimeKit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/SendEmail", async (
    string toName, 
    string toEmail,
    string fromName,
    string fromEmail,
    string subject, 
    string plainTextContent,
    string htmlContent) =>
    {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);
        var to = new EmailAddress(toEmail, toName);
        var from = new EmailAddress(fromEmail, fromName);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);

        return response;
    }
)
.WithName("SendEmail")
.WithOpenApi();

app.MapPost("/SendGmail", async (GmailEmailRequest request) =>
{
    var gmailConfig = new
    {
        Email = Environment.GetEnvironmentVariable("GMAIL_EMAIL"),
        Password = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD")
    };

    var message = new MimeMessage();
    message.From.Add(new MailboxAddress(request.FromName, gmailConfig.Email));
    message.To.Add(new MailboxAddress(request.ToName, request.ToEmail));
    message.Subject = request.Subject;

    message.Body = new TextPart("plain")
    {
        Text = request.Body
    };

    try
    {
        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(gmailConfig.Email, gmailConfig.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        return Results.Ok("Email sent successfully via Gmail.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Failed to send email: {ex.Message}");
    }
})
.WithName("SendGmail")
.WithOpenApi();


app.Run();

public record GmailEmailRequest(string FromName, string ToName, string ToEmail, string Subject, string Body);
