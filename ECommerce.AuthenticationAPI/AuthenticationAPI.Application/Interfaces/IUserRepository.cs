using AuthenticationAPI.Application.DTOs;
using AuthenticationAPI.Domain.Entities;
using ECommerce.SharedLibrary.Responses;

namespace AuthenticationAPI.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<Response> Register(AppUserDTO appUserDTO);
        Task<Response> Login(LoginDTO loginDTO);
        Task<GetUserDTO> GetUser(int userId);
        Task<AppUser> GetUserByEmail(string email);
    }
}
