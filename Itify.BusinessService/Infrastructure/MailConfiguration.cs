namespace Itify.BusinessService.Infrastructure;

public class MailConfiguration
{
    public bool MailEnable { get; set; }
    public string MailHost { get; set; } = null!;
    public ushort MailPort { get; set; }
    public string MailAddress { get; set; } = null!;
    public string MailUser { get; set; } = null!;
    public string MailPassword { get; set; } = null!;
    public string FrontendUrl { get; set; } = null!;
}
