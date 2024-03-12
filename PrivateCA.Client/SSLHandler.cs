using PrivateCA.Core.OpenSSL;

namespace PrivateCA.Client;

public static class SSLHandler {

    private const string SSLKeyPath = "/etc/privateca/";

    public static async Task<SSLConfig> GenerateSSLAsync(string domain, string password) {
        var defaultColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        var sslPath = Path.Combine(SSLKeyPath, domain);
        var sslConfig = new SSLConfig();

        try {
            Console.WriteLine("Generating CSR");
            (var csrPath, var keyPath) = OpenSSL.GenerateCSRAndPrivKey(domain, sslPath, "N35T");
            Console.WriteLine("Generating Diffie Hellman Parameters");
            var dhPath = OpenSSL.GenerateDHConfig(sslPath);

            sslConfig.PrivateKeyPath = keyPath;
            sslConfig.DhConfigPath = dhPath;

            var csrContent = await File.ReadAllTextAsync(csrPath);
            var extContent = OpenSSL.GenerateCSRExtContent(domain);

            Console.WriteLine("\nWaiting for the signing process...");
            var privateCa = new PrivateCAApi();
            var csrResponse = await privateCa.SignCsrAsync(new CsrDTO(csrContent, extContent, password));

            Console.WriteLine("Reponse from signing service!");

            var certPath = Path.Combine(sslPath, "publiccert.cert");
            await File.WriteAllTextAsync(certPath, csrResponse.CertContent);

            sslConfig.CertPath = certPath;
            
        }catch(Exception e) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Experienced an Error!");
            Console.WriteLine(e);
            throw;
        }finally {
            Console.ForegroundColor = defaultColor;
        }

        return sslConfig;
    }

}
