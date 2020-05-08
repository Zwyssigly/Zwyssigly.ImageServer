namespace Zwyssigly.ImageServer.Contracts
{ 
    public class AccountConfiguration
    {
        public string Name { get; }
        public string Type { get; }
        public string? Password { get; }
        public string[] Permissions { get; }

        public AccountConfiguration(string name, string type, string? password, string[] permissions)
        {
            Name = name;
            Type = type;
            Password = password;
            Permissions = permissions;
        }
    }
}
