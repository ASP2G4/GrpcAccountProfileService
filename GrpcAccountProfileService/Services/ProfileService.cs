using Grpc.Core;
using GrpcAccountProfileService.Contexts;
using GrpcAccountProfileService.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrpcAccountProfileService.Services
{
    public interface IProfileService
    {
        Task<GetProfileByUserIdReply> GetProfileByUserId(GetProfileByUserIdRequest request, ServerCallContext context);
        Task<CreateProfileReply> CreateProfile(CreateProfileRequest request, ServerCallContext context);
        Task<UpdateProfileReply> UpdateProfile(UpdateProfileRequest request, ServerCallContext context);
        Task<DeleteProfileByUserIdReply> DeleteProfileByUserId(DeleteProfileByUserIdRequest request, ServerCallContext context);
    }
    public class ProfileService(AccountProfileContext context) : ProfileHandler.ProfileHandlerBase, IProfileService
    {
        private readonly AccountProfileContext _context = context;

        public override async Task<GetProfileByUserIdReply> GetProfileByUserId(GetProfileByUserIdRequest request, ServerCallContext context)
        {
            var profile = await _context.AccountProfiles
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.UserId == request.UserId);

            if (profile == null)
            {
                return new GetProfileByUserIdReply
                {
                    Profile = null,
                    StatusCode = 404 
                };
            }

            return new GetProfileByUserIdReply
            {
                Profile = new Profile
                {
                    Id = profile.Id,
                    UserId = profile.UserId,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    PhoneNumber = profile.PhoneNumber,
                    Address = new ProfileAddress
                    {
                        StreetName = profile.Address?.StreetName,
                        PostalCode = profile.Address?.PostalCode,
                        City = profile.Address?.City
                    }
                },
                StatusCode = 200
            };
        }

        public override async Task<CreateProfileReply> CreateProfile(CreateProfileRequest request, ServerCallContext context)
        {
            if (request.Profile == null)
                return new CreateProfileReply { Success = false, Message = "Account profile is null" };

            var userProfileExist = await _context.AccountProfiles
                .FirstOrDefaultAsync(p => p.UserId == request.Profile.UserId);

            if (userProfileExist != null)
            {
                return new CreateProfileReply
                {
                    Success = false,
                    Message = "A profile with the given UserId already exists."
                };
            }

            var accountProfile = new AccountProfile
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.Profile.UserId,
                FirstName = request.Profile.FirstName,
                LastName = request.Profile.LastName,
                PhoneNumber = request.Profile.PhoneNumber,
                Address = new AccountProfileAddress
                {
                    StreetName = request.Profile.Address.StreetName,
                    PostalCode = request.Profile.Address.PostalCode,
                    City = request.Profile.Address.City
                }
            };

            _context.AccountProfiles.Add(accountProfile);
            await _context.SaveChangesAsync();

            return new CreateProfileReply { Success = true, Message = "Account succesfully created" };
        }
        public override async Task<UpdateProfileReply> UpdateProfile(UpdateProfileRequest request, ServerCallContext context)
        {
            if (request.Profile == null)
            {
                return new UpdateProfileReply { Success = false };
            }

            var profile = await _context.AccountProfiles
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.UserId == request.Profile.UserId);
            profile.FirstName = request.Profile.FirstName;
            profile.LastName = request.Profile.LastName;
            profile.PhoneNumber = request.Profile.PhoneNumber;
            profile.Address.StreetName = request.Profile.Address.StreetName;
            profile.Address.PostalCode = request.Profile.Address.PostalCode;
            profile.Address.City = request.Profile.Address.City;

            await _context.SaveChangesAsync();

            return new UpdateProfileReply { Success = true };
        }

        public override async Task<DeleteProfileByUserIdReply> DeleteProfileByUserId(DeleteProfileByUserIdRequest request, ServerCallContext context)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserId))
                return new DeleteProfileByUserIdReply { Success = false };

            var profile = await _context.AccountProfiles
                .FirstOrDefaultAsync(p => p.UserId == request.UserId);

            if (profile == null)
            {
                return new DeleteProfileByUserIdReply { Success = false };
            }

            _context.AccountProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return new DeleteProfileByUserIdReply { Success = true };
        }
    }
}