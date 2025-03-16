using AuthService.Bridge;
using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Status> GetUserProfileAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new Status { Code = "1001", Message = "User not found", Data = null };
            }

            return new Status
            {
                Code = "0000",
                Message = "User profile fetched successfully",
                Data = new
                {
                    user.Id,
                    user.Name,
                    user.Surname,
                    user.Email,
                    user.Cell,
                    user.ProfilePicture,  
                    user.IsEmailVerified,  
                    user.IsPhoneVerified,  
                    user.IsActive,         
                    user.IsFitMember,      
                    user.IsMarketing       
                }
            };
        }

        public async Task<Status> UpdateUserProfileAsync(UpdateProfileRequest request)
        {
            
            if (!IsValidEmail(request.Email))
            {
                return new Status { Code = "1002", Message = "Invalid email format", Data = null };
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return new Status { Code = "1001", Message = "User not found", Data = null };
            }


            /* user.Name = request.Name;
             user.Surname = request.Surname;
             user.Cell = request.Cell;
             user.Email = request.Email;
             user.ProfilePicture = request.ProfilePicture;*/

            // Only update fields if they are provided (not null)
            user.Name = !string.IsNullOrEmpty(request.Name) ? request.Name : user.Name;
            user.Surname = !string.IsNullOrEmpty(request.Surname) ? request.Surname : user.Surname;
            user.Cell = !string.IsNullOrEmpty(request.Cell) ? request.Cell : user.Cell;
            user.ProfilePicture = !string.IsNullOrEmpty(request.ProfilePicture) ? request.ProfilePicture : user.ProfilePicture;



            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new Status { Code = "0000", Message = "User profile updated successfully", Data = null };
        }

        
        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";  
            return Regex.IsMatch(email, emailPattern);
        }
    }
}
