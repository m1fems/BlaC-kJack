using System;
using System.Collections.Generic;
using System.Text;

namespace BlackJack
{
    class User
    {
        private string username;
        private string password;

        public User() { }

        public User(string username, string password, Chips chips)
        {
            Username = username;
            Password = password;
            Chips = chips;
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public Chips Chips
        {
            get; set;
        }

    }
}
