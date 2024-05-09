namespace SimpleAuthExample.DB.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public virtual List<Role> Roles { get; set; }
    }
}
