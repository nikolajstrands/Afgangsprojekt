using System;
using UserDataAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace UserDataAPI.Data
{
    // Repository når systemet kører normalt
    public class UserDataRepo : IUserDataRepo
    {
        private readonly UserAlbumContext _db;
        private readonly DbSet<User> _users;

        public UserDataRepo()
        {
            _db = new UserAlbumContext();
            _users = _db.Users;
        }

        // Tilføj ny bruger
        public int Add(User newUser)
        {
            _users.Add(newUser);
            return _db.SaveChanges();
   
        }

        // Hent bruger efter ID    
        public User GetById(int id)
        {
            var user = _users.Select(u => u).SingleOrDefault(u => u.Id == id);               
            return user;            
        }

        // Gem bruger
        public int Save(User user)
        {
            _db.Entry(user).State = EntityState.Modified;
            return _db.SaveChanges();
            
        }

    }    
}