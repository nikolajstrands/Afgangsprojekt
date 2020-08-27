using System;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using UserDataAPI.Controllers;
using UserDataAPI.Data;
using System.Security.Claims;


public class UserControllerTest
{
    UserController _controller;
    IUserDataRepo _repo;
 
    public UserControllerTest()
    {
        _repo = new UserDataRepoTest();

        _controller = new UserController(_repo);

        MockUser("88421113");
        
    }

    // Test GET api/me/albums
    [Fact]
    public void GetUserAlbums_WhenCalled_ReturnsOkResult()
    {
        // Act
        var okResult = _controller.GetUserAlbums();
 
        // Assert
        Assert.IsType<OkObjectResult>(okResult.Result);
    }
 
    [Fact]
    public void GetUserAlbums_WhenCalled_ReturnsRightAlbums()
    {
        // Act
        var okResult = _controller.GetUserAlbums().Result as OkObjectResult;
 
        // Assert
        var albums = Assert.IsType<List<Guid>>(okResult.Value);
        Assert.Equal(albums, new List<Guid>{
            new Guid("b01f56de-692b-46ce-bb09-ae2068d892a4"),
            new Guid("6bdb0e2f-d2a3-4e9d-8de3-857066ab373d"),
            new Guid("ef73f9a0-33ad-4b1b-8d32-beceb43511ac"),
            new Guid("84d3e86f-f89c-4fed-9c1f-f967da6ef4ea")
        });
    }

    [Fact]
    public void GetUserAlbums_UnknownId_ReturnsEmptyList()
    {
        // Arrange
        MockUser("84579");

        // Act
        var okResult = _controller.GetUserAlbums().Result as OkObjectResult;
    
        // Assert
        var albums = Assert.IsType<List<Guid>>(okResult.Value);
        Assert.Equal(albums, new List<Guid>());
    }

    [Fact]
    public void GetUserAlbums_NoUserId_ReturnsForbidden()
    {
        // Arrange
        MockUserWithoutUserId();

        // Act
        var forbiddenResult = _controller.GetUserAlbums();
    
        // Assert
        Assert.IsType<ForbidResult>(forbiddenResult.Result);
        
    }

    [Fact]
    public void GetUserAlbums_EmptyUserId_ReturnsForbidden()
    {
        // Arrange
        MockUser("");

        // Act
        var forbiddenResult = _controller.GetUserAlbums();
    
        // Assert
        Assert.IsType<ForbidResult>(forbiddenResult.Result);
        
    }

    // Test PUT api/me/albums  
    [Fact]     
    public void AddAlbums_NoUserId_ReturnsForbidden()
    {
        // Arrange
        MockUserWithoutUserId();

        // Act
        var forbiddenResult = _controller.PutUserAlbums(new List<Guid>() { 
            new Guid("c8a6b015-3e55-448d-b16e-aeb1d0e1237d"),
            new Guid("f2335ac8-900f-4cdc-978e-592ddb57950e"),
            new Guid("3c3aabf8-c730-44f1-99b7-40f4af03709b")
        });
    
        // Assert
        Assert.IsType<ForbidResult>(forbiddenResult);
        
    }

    [Fact]
    public void AddAlbums_EmptyUserId_ReturnsForbidden()
    {
        // Arrange
        MockUser("");

        // Act
        var forbiddenResult = _controller.PutUserAlbums(new List<Guid>() { 
            new Guid("c8a6b015-3e55-448d-b16e-aeb1d0e1237d"),
            new Guid("f2335ac8-900f-4cdc-978e-592ddb57950e"),
            new Guid("3c3aabf8-c730-44f1-99b7-40f4af03709b")
        });    

        // Assert
        Assert.IsType<ForbidResult>(forbiddenResult);
        
    }

    [Fact]
    public void AddAlbums_WhenCalled_ReturnsCreatedResult()
    {
        // Act
        var createdResult = _controller.PutUserAlbums(new List<Guid>() { 
            new Guid("c8a6b015-3e55-448d-b16e-aeb1d0e1237d"),
            new Guid("f2335ac8-900f-4cdc-978e-592ddb57950e"),
            new Guid("3c3aabf8-c730-44f1-99b7-40f4af03709b")
        });
 
        // Assert
        Assert.IsType<CreatedResult>(createdResult);
    }

    [Fact]
    public void AddAlbums_InvalidObjectPassed_ReturnsBadRequest()
    {
        // Arrange
        var albumIDs = new List<Guid>
        {
        };
        _controller.ModelState.AddModelError("AlbumIDs", "Must be non-empty");
    
        // Act
        var badResponse = _controller.PutUserAlbums(albumIDs);
    
        // Assert
        Assert.IsType<BadRequestObjectResult>(badResponse);
    }
    
    [Fact]
    public void AddAlbums_ValidObjectPassed_ReturnedResponseHasAddedAlbums()
    {
        // Arrange
        var testData = new List<Guid>() { 
            new Guid("c8a6b015-3e55-448d-b16e-aeb1d0e1237d"),
            new Guid("f2335ac8-900f-4cdc-978e-592ddb57950e"),
            new Guid("3c3aabf8-c730-44f1-99b7-40f4af03709b")
        };

        // Act
        var createdResult = _controller.PutUserAlbums(testData);

        // Assert
        Assert.IsType<CreatedResult>(createdResult);
        
        var okResult = _controller.GetUserAlbums().Result as OkObjectResult;

        var albums = Assert.IsType<List<Guid>>(okResult.Value);
        Assert.Equal(albums, new List<Guid> {
                    new Guid("b01f56de-692b-46ce-bb09-ae2068d892a4"),
                    new Guid("6bdb0e2f-d2a3-4e9d-8de3-857066ab373d"),
                    new Guid("ef73f9a0-33ad-4b1b-8d32-beceb43511ac"),
                    new Guid("84d3e86f-f89c-4fed-9c1f-f967da6ef4ea"),
                    new Guid("c8a6b015-3e55-448d-b16e-aeb1d0e1237d"),
                    new Guid("f2335ac8-900f-4cdc-978e-592ddb57950e"),
                    new Guid("3c3aabf8-c730-44f1-99b7-40f4af03709b")
                });
     

    }

    // Test DELETE api/users
    [Fact]     
    public void RemoveAlbums_NoUserId_ReturnsForbidden()
    {
        // Arrange
        MockUserWithoutUserId();

        // Act
        var forbiddenResult = _controller.DeleteUserAlbums(new List<Guid>() { 
            new Guid("c8a6b015-3e55-448d-b16e-aeb1d0e1237d"),
            new Guid("f2335ac8-900f-4cdc-978e-592ddb57950e"),
            new Guid("3c3aabf8-c730-44f1-99b7-40f4af03709b")
        });
    
        // Assert
        Assert.IsType<ForbidResult>(forbiddenResult);
        
    }

    [Fact]
    public void Removelbums_EmptyUserId_ReturnsForbidden()
    {
        // Arrange
        MockUser("");

        // Act
        var forbiddenResult = _controller.PutUserAlbums(new List<Guid>() { 
            new Guid("c8a6b015-3e55-448d-b16e-aeb1d0e1237d"),
            new Guid("f2335ac8-900f-4cdc-978e-592ddb57950e"),
            new Guid("3c3aabf8-c730-44f1-99b7-40f4af03709b")
        });
    
        // Assert
        Assert.IsType<ForbidResult>(forbiddenResult);
        
    }

    [Fact]
    public void RemoveAlbums_WhenCalled_ReturnsOkResult()
    {
        // Act
        var okResult = _controller.DeleteUserAlbums(new List<Guid>() { 
            new Guid("c8a6b015-3e55-448d-b16e-aeb1d0e1237d"),
            new Guid("f2335ac8-900f-4cdc-978e-592ddb57950e"),
            new Guid("3c3aabf8-c730-44f1-99b7-40f4af03709b")
        });
 
        // Assert
        Assert.IsType<OkResult>(okResult);
    }

    [Fact]
    public void RemoveAlbums_ValidObjectPassed_ReturnedResponseHasRemovedAlbums()
    {
        // Arrange
        var testData = new List<Guid>
                {
                    new Guid("b01f56de-692b-46ce-bb09-ae2068d892a4"),
                    new Guid("6bdb0e2f-d2a3-4e9d-8de3-857066ab373d"),
                    new Guid("ef73f9a0-33ad-4b1b-8d32-beceb43511ac"),
                    new Guid("84d3e86f-f89c-4fed-9c1f-f967da6ef4ea")
                };

        // Act
        var removedResult = _controller.DeleteUserAlbums(testData);

        // Assert
        Assert.IsType<OkResult>(removedResult);
        
        var okResult = _controller.GetUserAlbums().Result as OkObjectResult;

        var albums = Assert.IsType<List<Guid>>(okResult.Value);
        Assert.Equal(albums, new List<Guid>{ });

    }


    private void MockUser(string userId)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    private void MockUserWithoutUserId()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {           
        }));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }
}