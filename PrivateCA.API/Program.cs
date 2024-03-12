using PrivateCA.Core.DTOs;
using PrivateCA.Core.OpenSSL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/signcsr", (CsrDTO data) => {

    var csrPath = "/etc/privateca/csrContent";
    var extPath = "/etc/privateca/extContent";

    File.WriteAllText(csrPath, data.CsrContent); 
    File.WriteAllText(extPath, data.ExtContent); 
    var outPath = "/etc/privateca/certContent";

    var caPath = "/etc/privateca/";
    var caName = "peter";

    OpenSSL.SignCSRWithCAKey(csrPath, extPath, outPath, caPath, caName, data.Password);

    var certContent = File.ReadAllText(outPath); 
    return new CsrResponseDTO(certContent); ;
});
    // .WithName("SignCertificate")
    // .WithOpenApi();

app.Run();