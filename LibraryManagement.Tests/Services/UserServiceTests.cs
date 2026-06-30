using LibraryManagement.Tests.Helpers;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Repositories;
using LibraryManagement.Web.Services;
using Moq;

namespace LibraryManagement.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task EmailExistsAsync_WhenEmailExists_ReturnsTrue()
    {
        var users = new List<User>
        {
            new() { UserId = 1, Username = "usera", Email = "usera@test.com", Password = "12345" }
        };
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(users));
        var service = new UserService(repository.Object);

        var result = await service.EmailExistsAsync("usera@test.com");

        Assert.True(result);
    }

    [Fact]
    public async Task UsernameExistsAsync_WhenUsernameExists_ReturnsTrue()
    {
        var users = new List<User>
        {
            new() { UserId = 1, Username = "usera", Email = "usera@test.com", Password = "12345" }
        };
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(users));
        var service = new UserService(repository.Object);

        var result = await service.UsernameExistsAsync("usera");

        Assert.True(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithBcryptPassword_ReturnsUser()
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("12345");
        var user = new User
        {
            UserId = 1,
            Username = "usera",
            Email = "usera@test.com",
            Password = hashedPassword
        };
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync("usera")).ReturnsAsync(user);
        var service = new UserService(repository.Object);

        var result = await service.ValidateLoginAsync(new LoginViewModel
        {
            Username = "usera",
            Password = "12345"
        });

        Assert.NotNull(result);
        Assert.Equal("usera@test.com", result!.Email);
    }
    [Fact]
    public async Task FindOrCreateExternalUserAsync_WhenEmailExists_ReturnsExistingUser()
    {
        var user = new User
        {
            UserId = 2,
            Username = "existing",
            Email = "existing@test.com",
            Password = "password"
        };
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByEmailAsync("existing@test.com")).ReturnsAsync(user);
        var service = new UserService(repository.Object);

        var result = await service.FindOrCreateExternalUserAsync("existing@test.com", "Existing User", "Google", "google-1");

        Assert.Equal(user.UserId, result.UserId);
        Assert.Equal("Google", result.Provider);
        Assert.Equal("google-1", result.ProviderUserId);
        repository.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
        repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task FindOrCreateExternalUserAsync_WhenEmailDoesNotExist_CreatesUserWithUserRole()
    {
        var users = new List<User>();
        User? addedUser = null;
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByEmailAsync("newuser@test.com")).ReturnsAsync((User?)null);
        repository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(users));
        repository.Setup(r => r.Add(It.IsAny<User>())).Callback<User>(user => addedUser = user);
        var service = new UserService(repository.Object);

        var result = await service.FindOrCreateExternalUserAsync("newuser@test.com", "New User", "Google", "google-2");

        Assert.NotNull(addedUser);
        Assert.Equal("newuser@test.com", result.Email);
        Assert.Equal("User", result.Role);
        Assert.Equal("Google", result.Provider);
        Assert.Equal("google-2", result.ProviderUserId);
        Assert.False(string.IsNullOrWhiteSpace(result.Password));
        repository.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
