namespace PennyProject.Models
{
    public class UserUpdateRequestDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
    }
}
