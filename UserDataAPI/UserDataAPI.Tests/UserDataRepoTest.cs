using System;
using UserDataAPI.Models;
using System.Collections.Generic;
using UserDataAPI.Data;
using System.Linq;

// Test-repository med hardcodede værdier
public class UserDataRepoTest : IUserDataRepo
{
    private readonly List<User> _users;

    public UserDataRepoTest()
    {
        _users = new List<User>()
        {
            new User
            {
                Id = 1,
                AlbumIDs = new List<Guid>
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid()
                }
            },
            new User
            {
                Id = 2,
                AlbumIDs = new List<Guid>
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid()
                }
            },
            new User
            {
                Id = 88421113,
                AlbumIDs = new List<Guid>
                {
                    new Guid("b01f56de-692b-46ce-bb09-ae2068d892a4"),
                    new Guid("6bdb0e2f-d2a3-4e9d-8de3-857066ab373d"),
                    new Guid("ef73f9a0-33ad-4b1b-8d32-beceb43511ac"),
                    new Guid("84d3e86f-f89c-4fed-9c1f-f967da6ef4ea")
                }
            }
        };
    }

    public int Add(User newUser)
    {
        _users.Add(newUser);
        return 1;
    }
    
    public User GetById(int id)
    {
        return _users.Where(u => u.Id == id)
        .FirstOrDefault();
    }

    public int Save(User user)
    {
        var oldUser = _users.Where(u => u.Id == user.Id)
        .FirstOrDefault();

        _users.Remove(oldUser);
        _users.Add(user);

        return 1;     
    }
}
    
