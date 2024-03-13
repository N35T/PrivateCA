using PrivateCA.Core.Commands;

namespace PrivateCA.Core.OpenSSL; 

public static class OpenSSL {

    public static string GetCAKeyName(string CAName, string path) {
        return Path.Combine(path, CAName + ".ca.key");
    }
    public static string GetCACertName(string caName, string path) {
        return Path.Combine(path, caName + ".ca.crt");
    }

    private static string GetCSRPath(string domainName, string path) {
        return Path.Combine(path, domainName + ".client.csr");
    }
    
    private static string GetClientKeyPath(string domainName, string path) {
        return Path.Combine(path, domainName + ".client.key");
    }
    
    private static string GetClientCertPath(string domainName, string path) {
        return Path.Combine(path, domainName + ".client.crt");
    }
    

    public static string GenerateCAPrivateKey(string caName, string path, string password) {
        var fullOut = GetCAKeyName(caName, path);
        $"openssl genrsa -aes256 -out {fullOut} -passout pass:{password} 4096".Bash();

        return fullOut;
    }

    public static string GenerateRootCertificate(string caName, string path, string issuer, string password) {
        var privKey = GetCAKeyName(caName, path);
        var caCert = GetCACertName(caName, path);
        $"openssl req -x509 -new -nodes -key {privKey} -passin pass:{password} -sha256 -days 1826 -out {caCert} -subj '/CN={caName}/C=DE/ST={issuer}/L={issuer}/O={issuer}'"
            .Bash();

        return caCert;
    }

    public static (string, string) GenerateCSRAndPrivKey(string domainToSign, string path, string issuer) {
        var privKey = GetClientKeyPath(domainToSign, path);
        var csrPath = GetCSRPath(domainToSign, path);
        $"sudo openssl req -new -nodes -out {csrPath} -newkey rsa:4096 -keyout {privKey} -subj '/CN={domainToSign.Split('.')[0]}/C=DE/ST={issuer}/L={issuer}/O={issuer}'"
            .Bash();

        return (csrPath, privKey);
    }

    public static string GenerateDHConfig(string path) {
        var dhPath = Path.Combine(path, "dhparam.pem");
        $"sudo openssl dhparam -out {dhPath} 4096".Bash();

        return dhPath;
    }

    public static string GenerateCSRExtContent(string domainToSign) {
        return $"""
          authorityKeyIdentifier=keyid,issuer
          basicConstraints=CA:FALSE
          keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
          subjectAltName = @alt_names
          
          [alt_names]
          DNS.1 = {domainToSign}
          """;
    }

    public static void SignCSRWithCAKey(string csrPath, string extPath, string outPath, string caPath, string caName, string password) {
        var caKey = GetCAKeyName(caName, caPath);
        var caCert = GetCACertName(caName, caPath);
        $"openssl x509 -req -in {csrPath} -CA {caCert} -CAkey {caKey} -passin pass:{password} -CAcreateserial -out {outPath} -days 730 -sha256 -extfile {extPath}"
            .Bash();
    }
}