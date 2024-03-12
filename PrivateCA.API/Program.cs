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

var workerpath = "/worker/temp/";
var caPath = "/ca/";
var caName = "peter";

app.UseHttpsRedirection();

app.MapGet("/signcsr", (CsrDTO data) => {

    var guid = Guid.NewGuid().ToString();
    var workingOn = Path.Combine(workerpath, guid);
    var certContent = "";

    Directory.CreateDirectory(workingOn);

    try {
        certContent = GetCertContent(data, workingOn);
    }
    finally { 
        Directory.Delete(workingOn);
    }

    return new CsrResponseDTO(certContent);
});

string GetCertContent(CsrDTO data, string workingOn) {
    var csrPath = Path.Combine(workingOn, "csrContent");
    var extPath = Path.Combine(workingOn, "extContent");

    File.WriteAllText(csrPath, data.CsrContent);
    File.WriteAllText(extPath, data.ExtContent);

    var outPath = Path.Combine(workingOn, "certContent");

    OpenSSL.SignCSRWithCAKey(csrPath, extPath, outPath, caPath, caName, data.Password);

    var certContent = File.ReadAllText(outPath);
    return certContent;
}

app.Run();