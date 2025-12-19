namespace ApiSolutionTestVentas.Entities;

public class AppSettings
{
    
    public ConnectionStrings ConnectionStrings { get; set; } = default!;
    public Jwt Jwt { get; set; } = default!;
    public SmtpConfiguration SmtpConfiguration { get; set; } = default!;
    
    public EmailTemplates EmailTemplates { get; set; } = default!;
}

public class ConnectionStrings
{
    public string DefaultConnection { get; set; } = String.Empty;
    public string AzureStorage { get; set; } = String.Empty;
    
    public string ContainerNameBlob { get; set; } = String.Empty;
}

public class Jwt
{
    public string JWTKey { get; set; } = default!;
    public int LifetimeInSeconds { get; set; } = 0;
}

public class SmtpConfiguration
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Server { get; set; } = default!;
    public int PortNumber { get; set; }
    public string FromName { get; set; } = default!;
    public bool EnableSsl { get; set; }
}

public class EmailTemplates
{
    public string SubjectCreateUser { get; set; } = default!;
    public string SubjectResetPassword { get; set; } = default!;
    public string SubjectChangePassword { get; set; } = default!;
    public string CreateUserEmail { get; set; } = default!;
    public string ResetPasswordEmail { get; set; } = default!;
    public string ChangePasswordEmail { get; set; } = default!;
}