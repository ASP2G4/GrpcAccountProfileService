using GrpcAccountProfileService;
using GrpcAccountProfileService.Contexts;
using GrpcAccountProfileService.Entities;
using GrpcAccountProfileService.Services;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountProfileService.Tests.Services
{
    public class ProfileService_IntegrationTest
    {
        private readonly AccountProfileContext _context;
        private readonly IProfileService _profileService;

        public ProfileService_IntegrationTest()
        {
            var options = new DbContextOptionsBuilder<AccountProfileContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AccountProfileContext(options);
            _profileService = new ProfileService(_context);
        }

        [Fact]
        public async Task GetProfileByUserId_ShouldReturnProfile_WhenProfileExists()
        {
            // arrange
            var userId = Guid.NewGuid().ToString();
            var profile = new AccountProfile
            {
                UserId = userId,
                FirstName = "Test",
                LastName = "Test",
                PhoneNumber = "111111111111",
                Address = new AccountProfileAddress
                {
                    StreetName = "Testgatan 11",
                    PostalCode = "11111",
                    City = "Test"
                }
            };
            _context.AccountProfiles.Add(profile);
            await _context.SaveChangesAsync();
            // act
            var response = await _profileService.GetProfileByUserId(new GetProfileByUserIdRequest { UserId = userId }, null);
            // assert
            Assert.NotNull(response);
            Assert.Equal(userId, response.Profile.UserId);
        }
        [Fact]
        public async Task GetProfileByUserId_ShouldReturnNotFound_WhenProfileDoesNotExist()
        {
            // arrange
            var userId = Guid.NewGuid().ToString();
            // act
            var response = await _profileService.GetProfileByUserId(new GetProfileByUserIdRequest { UserId = userId }, null);
            // assert
            Assert.NotNull(response);
            Assert.Null(response.Profile);
            Assert.Equal(404, response.StatusCode);
        }

        [Fact]
        public async Task CreateProfile_ShouldCreateProfile()
        {
            // arrange
            var userId = Guid.NewGuid().ToString();

            var request = new CreateProfileRequest
            {
                Profile = new Profile
                {
                    UserId = userId,
                    FirstName = "Test",
                    LastName = "Test",
                    PhoneNumber = "111111111111",
                    Address = new ProfileAddress
                    {
                        StreetName = "Testgatan 11",
                        PostalCode = "11111",
                        City = "Test"
                    }
                }
            };

            //act
            var response = await _profileService.CreateProfile(request, null);

            // assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal("Account succesfully created", response.Message);

        }

        [Fact]
        public async Task CreateProfile_ShouldNotCreateProfile_IfProfileExsits()
        {
            // arrange
            var userId = Guid.NewGuid().ToString();
            var request = new CreateProfileRequest
            {
                Profile = new Profile
                {
                    UserId = userId,
                    FirstName = "Test",
                    LastName = "Test",
                    PhoneNumber = "111111111111",
                    Address = new ProfileAddress
                    {
                        StreetName = "Testgatan 11",
                        PostalCode = "11111",
                        City = "Test"
                    }
                }
            };
            // act
            var response = await _profileService.CreateProfile(request, null);
            var secondResponse = await _profileService.CreateProfile(request, null);
            // assert
            Assert.NotNull(secondResponse);
            Assert.False(secondResponse.Success);
            Assert.Equal("A profile with the given UserId already exists.", secondResponse.Message);
        }

        [Fact]
        public async Task CreateProfile_ShouldNotCreateProfile_IfProfileIsNull()
        {
            // arrange
            var request = new CreateProfileRequest
            {
                Profile = null

            };
            // act
            var response = await _profileService.CreateProfile(request, null);
            // assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal("Account profile is null", response.Message);
        }

        [Fact]
        public async Task UpdateProfile_ShouldUpdateProfile()
        {
            //arrange
            var userId = Guid.NewGuid().ToString();
            var profile = new AccountProfile
            {
                UserId = userId,
                FirstName = "Test",
                LastName = "Test",
                PhoneNumber = "111111111111",
                Address = new AccountProfileAddress
                {
                    StreetName = "Testgatan 11",
                    PostalCode = "11111",
                    City = "Test"
                }
            };
            _context.AccountProfiles.Add(profile);
            await _context.SaveChangesAsync();

            var updatedProfile = new UpdateProfileRequest
            {
                Profile = new Profile
                {
                    UserId = userId,
                    FirstName = "Updated",
                    LastName = "Updated",
                    PhoneNumber = "222222222222",
                    Address = new ProfileAddress
                    {
                        StreetName = "Updatedgatan 22",
                        PostalCode = "22222",
                        City = "Updated"
                    }
                }
            };
            // act
            var response = await _profileService.UpdateProfile(updatedProfile, null);

            // assert
            Assert.NotNull(response);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task UpdateProfile_ShouldNotUpdateProfile_IfProfileIsNull()
        {
            // arrange
            var request = new UpdateProfileRequest
            {
                Profile = null
            };
            // act
            var response = await _profileService.UpdateProfile(request, null);
            // assert
            Assert.NotNull(response);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task DeleteProfile_ShouldDeleteProfile()
        {
            // arrange
            var userId = Guid.NewGuid().ToString();
            var profile = new AccountProfile
            {
                UserId = userId,
                FirstName = "Test",
                LastName = "Test",
                PhoneNumber = "111111111111",
                Address = new AccountProfileAddress
                {
                    StreetName = "Testgatan 11",
                    PostalCode = "11111",
                    City = "Test"
                }
            };
            _context.AccountProfiles.Add(profile);
            await _context.SaveChangesAsync();

            var request = new DeleteProfileByUserIdRequest
            {
                UserId = userId
            };
            // act
            var response = await _profileService.DeleteProfileByUserId(request, null);
            // assert
            Assert.NotNull(response);
            Assert.True(response.Success);

            var deleted = await _context.AccountProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteProfile_ShouldReturnFalse_IfProfileDontExists()
        {
            // arrange
            var userId = Guid.NewGuid().ToString();
            var request = new DeleteProfileByUserIdRequest
            {
                UserId = userId
            };
            // act
            var response = await _profileService.DeleteProfileByUserId(request, null);
            // assert
            Assert.NotNull(response);
            Assert.False(response.Success);

        }
    }
}
