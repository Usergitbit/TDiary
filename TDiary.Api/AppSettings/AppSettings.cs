namespace TDiary.Api.AppSettings
{
    public class AppSettings
    {
        public Cors Cors { get; set; }
        public Authorization Authorization { get; set; }
        public Server Server { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }
}
