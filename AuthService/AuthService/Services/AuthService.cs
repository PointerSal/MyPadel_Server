using AuthService.Bridge;
using AuthService.Interfaces;
using AuthService.Models;
using AuthService.Model; 
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public AuthService(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        public async Task<Status> RegisterUser(RegisterRequest request)
        {
            try
            {
                if (!IsValidEmail(request.Email))
                {
                    return new Status { Code = "1002", Message = "Invalid email format", Data = null };
                }
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    return new Status { Code = "1001", Message = "Email already exists", Data = null };
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var newUser = new User
                {
                    Guid = Guid.NewGuid(),
                    Name = request.Name,
                    Surname = request.Surname,
                    Email = request.Email,
                    Password = hashedPassword,
                    Cell = request.Cell,
                    OTP = new Random().Next(100000, 999999).ToString(), // ✅ Generates a 6-digit OTP
                    IsEmailVerified = false,
                    IsPhoneVerified = false,
                    IsActive = true
                };

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return new Status { Code = "0000", Message = "User registered successfully", Data = null };
            }
            catch (Exception ex)
            {
                return new Status { Code = "1111", Message = "Internal Server Error", Data = ex.Message };
            }
        }

        public async Task<Status> LoginUser(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return new Status { Code = "1002", Message = "Invalid email or password", Data = null };
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordValid)
            {
                return new Status { Code = "1002", Message = "Invalid email or password", Data = null };
            }

            // Generate JWT token
            var token = _tokenService.GenerateToken(user.Guid.ToString(), user.Email);

            // Include user data in response along with the token
            var responseData = new
            {
                user.Guid,
                user.Name,
                user.Surname,
                user.Email,
                user.Cell,
                Token = token
            };

            // Return the user data and token
            return new Status { Code = "0000", Message = "Login successful", Data = responseData };
        }


        public async Task<Status> VerifyEmail(VerifyEmailRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.OTP == request.OTP);
            if (user == null)
            {
                return new Status { Code = "1003", Message = "Invalid OTP or email", Data = null };
            }

            user.IsEmailVerified = true;
            await _context.SaveChangesAsync();

            return new Status { Code = "0000", Message = "Email verified successfully", Data = null };
        }

        public async Task<Status> VerifyPhone(VerifyPhoneRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Cell == request.Cell && u.OTP == request.OTP);
            if (user == null)
            {
                return new Status { Code = "1003", Message = "Invalid OTP or phone number", Data = null };
            }

            user.IsPhoneVerified = true;
            await _context.SaveChangesAsync();

            return new Status { Code = "0000", Message = "Phone verified successfully", Data = null };
        }

        public async Task<Status> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return new Status { Code = "1004", Message = "User not found", Data = null };
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.Password = hashedPassword;
            await _context.SaveChangesAsync();

            return new Status { Code = "0000", Message = "Password reset successfully", Data = null };
        }

        public async Task<Status> DeleteAccount(DeleteAccountRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return new Status { Code = "1004", Message = "User not found", Data = null };
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new Status { Code = "0000", Message = "Account deleted successfully", Data = null };
        }
        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Basic email regex
            return Regex.IsMatch(email, emailPattern);
        }
    }
}
