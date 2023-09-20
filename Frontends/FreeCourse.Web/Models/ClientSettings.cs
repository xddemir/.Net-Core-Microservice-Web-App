namespace FreeCourse.Web.Models;

public class ClientSettings
{
    public Client WebClientForUser { get; set; }
    public Client WebClient { get; set; }
}

public class Client
{
    public string ClientId{ get; set; }
    public string ClientSecret { get; set; }
}
