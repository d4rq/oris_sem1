﻿namespace OrmLibrary.UnitTests.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Gender { get; set; }
    }
}
