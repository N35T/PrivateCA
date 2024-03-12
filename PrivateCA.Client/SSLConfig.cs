namespace PrivateCA.Client;

public class SSLConfig {

    public string PrivateKeyPath {get;set;} = null!;

    public string CertPath {get;set;} = null!;

    public string DhConfigPath {get;set;} = null!;
}
