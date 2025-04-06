![](https://github.com/VCDW-2025-PROG7311/EmailService/workflows/.NET/badge.svg) <-- This is covered at the end of this readme...

## Integrating SendGrid into a Minimal API and adding GitHub Actions

You will need to integrate email notifications into your app. One good solution that you can easily sign up for (do not pay) is [SendGrid](https://sendgrid.com/en-us/solutions/email-api)

You will implement the email-sending feature in a [Minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0).

Read these resources before carrying on!

### Sign up for SendGrid

1. Sign up at the link above
1. Once signed up, go to the Integration docs: https://app.sendgrid.com/guide/integrate.
1. Choose WebAPI as the setup method.
1. Choose C# as a language.
1. Follow the steps, including giving your ApiKey a name
1. Once you give it a name, you will get an ApiKey which you need to add as an environment variable (I named by environment variable `SENDGRID_API_KEY` as suggested by SendGrid).
1. The SendGrid platform will guide you to add a verified sender. This is the address from which you will send your emails. I used my Gmail.

### Create a Minimal API project
1. ```dotnet new webapi -o EmailService```
1. Once the project is created, run it and you will see it's similar to an API to the end user.
1. Watch this video to understand how it works: https://www.youtube.com/watch?v=87oOF9Ve-KA&ab_channel=IAmTimCorey (Go to 36 mins and watch until the end).

### Integrating SendGrid into the Mininal API

1. install the SendGrid NuGet package as indicated.
1. Using the code on the SendGrid Integration Docs, you can refactor it a bit:
    ```
    var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
    var client = new SendGridClient(apiKey);
    var from = new EmailAddress("test@example.com", "Example User");
    var subject = "Sending with SendGrid is Fun";
    var to = new EmailAddress("test@example.com", "Example User");
    var plainTextContent = "and easy to do anywhere, even with C#";
    var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
    var response = await client.SendEmailAsync(msg);
    ```

1. In `Program.cs`, add a MapGet called `SendEmail` with an anonymous `async` method that sends the email using the NuGet package: 
    ```
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
    ```
    What is happening here:
    1. We are defining a single action for this API that is named SendEmail. It responds to GET requests made to the `/SendEmail` endpoint.
    1. We have refactored the code to get everything that is needed as parameters in the request. You need to add validation.
    1. The relevant data is assembled as per the documentation and an email is sent.
1. Test this in the Swagger docs when you run the API or Postman. Note: A status code of 202 means the email is sent. If you try to use institutional email to send, it gets finicky.
1. If all works, good stuff! Now integrate it into your any app you want to :-)
1. We might  deploy this when we get to containerization.

### Adding GitHub Actions
1. Follow this tutorial: https://thesharperdev.com/how-to-create-a-net-core-api-ci-action-using-github-actions/
1. Bear in mind I deviated with some commands and put dotnet 8.0.
1. Add in a badge as well.
1. Since this service uses an Environment variable, add it in as a secret and use it in your workflow: [https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions](https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions#using-encrypted-secrets-in-a-workflow)https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions#using-encrypted-secrets-in-a-workflow

/END
