namespace EventPAM.BuildingBlocks.Mailing;

public class ToEmail
{
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;

    public ToEmail() { }

    public ToEmail(string email, string fullName)
    {
        Email = email;
        FullName = fullName;
    }
}
