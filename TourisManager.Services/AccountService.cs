using System;
using System.Collections.Generic;
using System.Text;
using TourisManager.Core.Entities;

namespace TourisManager.Services
{
    public interface AccountService
    {
        public bool Create(User user);
        public bool Login(string email, string password);
        public User FinebyEmail(string email);
        public bool Update(User user);

    }
}
