namespace Web.Common.Models.Authorize
{
    public class LoginModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}