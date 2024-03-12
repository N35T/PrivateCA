using PrivateCA.Client;
using PrivateCA.Core;
using Sharprompt;

const string helpText = """
    ------------------------------------------
    Usage: privateca registerdomain
    """;

if(args.Length < 1) {
    
    Console.WriteLine(helpText);
    return;
}

if(args[0].ToLower().Equals("registerdomain")){
    Console.WriteLine(helpText);
    return;
}
var domain = Prompt.Input<string>("Your domain").ToLower();
var port = Prompt.Input<int>("On what port is your service running?");
var password = Prompt.Password("What is the CA Password");

Console.WriteLine("Generating SSL Certificates...");
SSLConfig config = await SSLHandler.GenerateSSLAsync(domain, password);

Console.WriteLine("Done with the SSL Configuration!\n\nStarting to register the domain with nginx...");

NginX.RegisterDomain(domain, port, config.CertPath, config.PrivateKeyPath, config.DhConfigPath);

Console.WriteLine("Everything went smooooth - Your domain should now be reachable!\nHave a good day!");