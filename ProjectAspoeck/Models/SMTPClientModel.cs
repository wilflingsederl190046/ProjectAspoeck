namespace ProjectAspoeck.Models;

public class SMTPClientModel
{
    public string SenderAddress { get; set; }
    public string SenderDisplayname { get; set; }
    public string UserName { get; set; }
    public string Password{ get; set; }
    public string Host{ get;set; }
    public string Port{ get; set; }
    public string EnableSSl { get; set; }
    public string UseDefaultCredentials { get; set; }
    public string IsBodyHtml { get; set; } 
}