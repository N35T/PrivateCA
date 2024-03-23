using PrivateCA.API;
using PrivateCA.Client;
using PrivateCA.Core;
using PrivateCA.Core.CA;
using PrivateCA.Core.OpenSSL;
using Sharprompt;


var registerDomainDisplay = "Register a new Domain";
var createCADisplay = "Create your own Certification Authority";
var action = Prompt.Select<string>("Hello! What do you want to do today?", [registerDomainDisplay, createCADisplay]);

if(action.Equals(registerDomainDisplay)) {
    await RegisterDomainAction();
}else if(action.Equals(createCADisplay)) {
    await CreateCAAction();
}else {
    Console.WriteLine("Error when processing your choice!");
    return;
}

async Task CreateCAAction() {
    Console.WriteLine("INFO: Your following inputs should match your appsettings.json for the API.");

    var name = Prompt.Input<string>("Your CA name");
    var issuer = Prompt.Input<string>("Your issuer");
    var password = Prompt.Password("What is the CA Password");
    var path = Prompt.Input<string>("Your CA path");

    var domain = Prompt.Input<string>("Your CA domain").ToLower();
    var port = Prompt.Input<int>("On what port is your CA running?");

    Console.WriteLine("Generating Root Certificate and Private Key...");
    OpenSSL.GenerateCAPrivateKey(name, path, password);
    OpenSSL.GenerateRootCertificate(name, path, issuer, password);

    Console.WriteLine("Generating SSL Certificates...");
    SSLConfig config = await SSLHandler.GenerateSSLAsync(domain, password, new LocalCAApi(name, path, issuer, password));
    Console.WriteLine("Done with the SSL Configuration!\n\nStarting to register the domain with nginx...");

    NginX.RegisterDomain(domain, port, config.CertPath, config.PrivateKeyPath, config.DhConfigPath);

    Console.WriteLine("Everything went smooooth - Your CA is all set!\nHave a good day!");
}

async Task RegisterDomainAction() {
    var domain = Prompt.Input<string>("Your domain").ToLower();
    var port = Prompt.Input<int>("On what port is your service running?");
    var password = Prompt.Password("What is the CA Password");

    Console.WriteLine("Generating SSL Certificates...");
    SSLConfig config = await SSLHandler.GenerateSSLAsync(domain, password, new PrivateCAApi());

    Console.WriteLine("Done with the SSL Configuration!\n\nStarting to register the domain with nginx...");

    NginX.RegisterDomain(domain, port, config.CertPath, config.PrivateKeyPath, config.DhConfigPath);

    Console.WriteLine("Everything went smooooth - Your domain should now be reachable!\nHave a good day!");
}