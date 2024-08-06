namespace EventPAM.BuildingBlocks.Mailing;

public class MailSettings
{
    public string Server { get; set; } = default!;

    public int Port { get; set; } = default!;

    public string SenderFullName { get; set; } = default!;

    public string SenderEmail { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public string Password { get; set; } = default!;

    public bool AuthenticationRequired { get; set; } = default!;

    public string? DkimPrivateKey { get; set; } = default!;

    public string? DkimSelector { get; set; } = default!;

    public string? DomainName { get; set; } = default!;

    public MailSettings()
    {
        AuthenticationRequired = false;
    }

    public MailSettings(
        string server,
        int port,
        string senderFullName,
        string senderEmail,
        string userName,
        string password,
        bool authenticationRequired = false,
        string? dkimPrivateKey = null,
        string? dkimSelector = null,
        string? domainName = null
    )
    {
        Server = server;
        Port = port;
        SenderFullName = senderFullName;
        SenderEmail = senderEmail;
        UserName = userName;
        Password = password;
        AuthenticationRequired = authenticationRequired;
        DkimPrivateKey = dkimPrivateKey;
        DkimSelector = dkimSelector;
        DomainName = domainName;
    }
}
