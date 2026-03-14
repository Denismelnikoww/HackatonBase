namespace Infrastructure.Email
{

    public partial class EmailTemplateBuilder
    {
        public class EmailData
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Link { get; set; }
            public string ButtonText { get; set; }
            public int ExpiryTime { get; set; } = 24;
        }
    }
}
