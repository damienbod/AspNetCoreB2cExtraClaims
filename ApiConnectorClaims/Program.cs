using ApiConnectorClaims;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ApiConnectorCertificateValidationService>();
builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
        .AddCertificate(options => // code from ASP.NET Core sample
        {
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth
            options.AllowedCertificateTypes = CertificateTypes.All;
            options.RevocationMode = X509RevocationMode.NoCheck;

            // Default values
            //options.AllowedCertificateTypes = CertificateTypes.Chained;
            //options.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            //options.RevocationMode = X509RevocationMode.Online;
            //options.ValidateCertificateUse = true;
            //options.ValidateValidityPeriod = true;

            options.Events = new CertificateAuthenticationEvents
            {
                OnCertificateValidated = context =>
                {
                    var validationService = context.HttpContext.RequestServices.GetService<ApiConnectorCertificateValidationService>();

                    if (validationService.ValidateCertificate(context.ClientCertificate))
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                            new Claim(ClaimTypes.Name, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer)
                        };

                        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                        context.Success();
                    }
                    else
                    {
                        context.Fail("invalid cert");
                    }

                    return Task.CompletedTask;
                }
            };
        })
        .AddCertificateCache();

// All other service configuration

builder.WebHost.UseKestrel(so =>
{
    var cert = new X509Certificate2(Path.Combine("./cert_rsa512.pfx"), "1234");
    so.Limits.MinRequestBodyDataRate = null;
    so.ConfigureHttpsDefaults(o =>
    {
        o.ServerCertificate = cert;
        o.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
    });
    //so.ListenLocalhost(5002, listenOptions =>
    //{
    //    listenOptions.UseHttps(cert);
    //    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    //    listenOptions.UseConnectionLogging();
    //});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
