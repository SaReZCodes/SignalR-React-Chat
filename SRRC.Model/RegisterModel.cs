namespace SCM.Model
{
    public class RegisterModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public int[] Groups { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
        public string LastLoggedIn { get; set; }
        public int? OstanId { set; get; }
        public int? ShahrestanId { set; get; }
        public bool IsAdmin { set; get; }
    }
}