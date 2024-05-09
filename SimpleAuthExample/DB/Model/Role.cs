﻿namespace SimpleAuthExample.DB.Model
{
    public class Role
    {
        public int Id { get; set; }
        public required string Title { get; set; }

        public virtual List<User> Users { get; set; }
    }
}
