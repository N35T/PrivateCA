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

var workerpath = "/worker/temp/"; // path from appsettings
var caPath = "/ca/"; // path from appsettings
var caName = "peter"; // CA-NAME from appsettings

async Task<string> GetCertContentAsync(CsrDTO data, string workingOn) {
    var csrPath = Path.Combine(workingOn, "csrContent");
    var extPath = Path.Combine(workingOn, "extContent");

    await File.WriteAllTextAsync(csrPath, data.CsrContent);
    await File.WriteAllTextAsync(extPath, data.ExtContent);

    var outPath = Path.Combine(workingOn, "certContent");

    OpenSSL.SignCSRWithCAKey(csrPath, extPath, outPath, caPath, caName, data.Password);

    var certContent = await File.ReadAllTextAsync(outPath);
    return certContent;
}

void CheckIfCaExists() {
    var caKeyName = OpenSSL.GetCAKeyName(caName, caPath);
    var caCertName = OpenSSL.GetCACertName(caName, caPath);
    if (!Path.Exists(caKeyName)) {
        OpenSSL.GenerateCAPrivateKey(caName, caPath, password);
    }
    if (!Path.Exists(caCertName)) {
        OpenSSL.GenerateRootCertificate(caName, caPath, "N35T", password); // ISSUER from appsettings
    }
}

app.UseHttpsRedirection();

app.MapGet("/signcsr", async (CsrDTO data) => {
    CheckIfCaExists(); // PW from appsettings

    var guid = Guid.NewGuid().ToString();
    var workingOn = Path.Combine(workerpath, guid);
    var certContent = "";

    Directory.CreateDirectory(workingOn);

    try {
        certContent = await GetCertContentAsync(data, workingOn);
    } finally {
        Directory.Delete(workingOn, true);
    }

    return new CsrResponseDTO(certContent);
});

app.MapGet("/pubkey", async () => {
    CheckIfCaExists();
    var pubkey = await File.ReadAllTextAsync(OpenSSL.GetCACertName(caName, caPath));
    return pubkey;
});

app.Run();