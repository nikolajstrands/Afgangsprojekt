using System;
using UserDataAPI.Models;
using System.Collections.Generic;

namespace UserDataAPI.Data
{
    // Interfaces for repository
    public interface IUserDataRepo
    {
        int Add(User newUser);
        User GetById(int id);
        int Save(User user);
    }
}