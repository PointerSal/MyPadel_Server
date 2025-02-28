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
        private readonly EmailService _emailService;
        private readonly PhoneOTPService _phoneOtpService;

        public AuthService(ApplicationDbContext context, TokenService tokenService, EmailService emailService, PhoneOTPService PhoneOTPService)
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
            _phoneOtpService = PhoneOTPService;
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
                string emailOTP = new Random().Next(100000, 999999).ToString();
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Surname = request.Surname,
                    Email = request.Email,
                    Password = hashedPassword,
                    EmailOTP = emailOTP,                   
                    EmailOTPExpiry = DateTime.UtcNow.AddSeconds(200),                   
                    IsEmailVerified = false,
                    IsPhoneVerified = false,
                    IsActive = true,
                    IsMarketing = request.IsMarketing
                };


                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                var token = _tokenService.GenerateToken(newUser.Id.ToString(), newUser.Email);


                string messageBody = $"Your OTP for email verification is: {emailOTP}";
                bool emailSent = _emailService.SendEmail(request.Email, "Email Verification", messageBody);

                if (emailSent)
                {
                    var responseData = new
                    {
                        newUser.Id,
                        newUser.Name,
                        newUser.Surname,
                        newUser.Email,
                        newUser.Cell,
                        newUser.IsEmailVerified,
                        newUser.IsPhoneVerified,
                        newUser.IsFitMember,
                        newUser.IsMarketing,
                        Token = token
                    };

                    return new Status { Code = "0000", Message = "User registered successfully, verification email sent.", Data = responseData };
                }
                else
                {
                    return new Status { Code = "1005", Message = "Failed to send verification email.", Data = null };
                }
            }
            catch (Exception ex)
            {
                return new Status { Code = "1111", Message = "Internal Server Error", Data = ex.Message };
            }
        }

        public async Task<Status> VerifyEmail(VerifyEmailRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.EmailOTP == request.OTP);

            if (user == null)
            {
                return new Status { Code = "1003", Message = "Invalid OTP or email", Data = null };
            }

            if (user.EmailOTPExpiry.HasValue && user.EmailOTPExpiry.Value < DateTime.UtcNow)
            {
                return new Status { Code = "1006", Message = "OTP has expired", Data = null };
            }

            user.IsEmailVerified = true;   
            await _context.SaveChangesAsync();

            return new Status { Code = "0000", Message = "Email verified successfully", Data = null };
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

            var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email);

            var responseData = new
            {
                user.Id,
                user.Name,
                user.Surname,
                user.Email,
                user.Cell,
                user.IsEmailVerified,
                user.IsPhoneVerified,
                user.IsFitMember,
                Token = token
            };

            return new Status { Code = "0000", Message = "Login successful", Data = responseData };
        }

        public async Task<Status> AddPhoneNumber(string email, string cell)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return new Status { Code = "1004", Message = "User not found", Data = null };
                }

                // If the cell number is the same, just resend the OTP
                if (user.Cell == cell)
                {
                    string newOTP = new Random().Next(100000, 999999).ToString();
                    user.PhoneOTP = newOTP;
                    user.PhoneOTPExpiry = DateTime.UtcNow.AddSeconds(200);

                    await _context.SaveChangesAsync();

                    bool phoneSent = await _phoneOtpService.SendPhoneOTP(user.Cell, newOTP);

                    if (phoneSent)
                    {
                        return new Status { Code = "0000", Message = "Phone OTP resent successfully", Data = null };
                    }
                    else
                    {
                        return new Status { Code = "1005", Message = "Failed to send OTP", Data = null };
                    }
                }
                else
                {
                    string phoneOTP = new Random().Next(100000, 999999).ToString();
                    user.Cell = cell;
                    user.PhoneOTP = phoneOTP;
                    user.PhoneOTPExpiry = DateTime.UtcNow.AddSeconds(200);

                    await _context.SaveChangesAsync();

                    bool phoneSent = await _phoneOtpService.SendPhoneOTP(cell, phoneOTP);

                    if (phoneSent)
                    {
                        return new Status { Code = "0000", Message = "Phone number added successfully, OTP sent.", Data = null };
                    }
                    else
                    {
                        return new Status { Code = "1005", Message = "Failed to send OTP", Data = null };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Status { Code = "1111", Message = "Internal Server Error", Data = ex.Message };
            }
        }



       




        public async Task<Status> VerifyPhone(VerifyPhoneRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Cell == request.Cell && u.PhoneOTP == request.OTP);
            if (user == null)
            {
                return new Status { Code = "1003", Message = "Invalid OTP or phone number", Data = null };
            }

            if (user.PhoneOTPExpiry.HasValue && user.PhoneOTPExpiry.Value < DateTime.UtcNow)
            {
                return new Status { Code = "1006", Message = "OTP has expired", Data = null };
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


        public async Task<Status> ResendEmailOTP(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new Status { Code = "1004", Message = "User not found", Data = null };
            }

            string newOTP = new Random().Next(100000, 999999).ToString();  
            user.EmailOTP = newOTP;   
            user.EmailOTPExpiry = DateTime.UtcNow.AddSeconds(200);  

            await _context.SaveChangesAsync();

            string messageBody = $"Your OTP for email verification is: {newOTP}";
            bool emailSent = _emailService.SendEmail(user.Email, "Email Verification", messageBody);

            if (emailSent)
            {
                return new Status { Code = "0000", Message = "OTP resent successfully", Data = null };
            }

            return new Status { Code = "1005", Message = "Failed to send OTP", Data = null };
        }


        public async Task<Status> UpdatePassword(UpdatePasswordRequest request)
        {
            try
            {
                // Retrieve user by email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return new Status { Code = "1004", Message = "User not found", Data = null };
                }

                // Verify current password
                var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password);
                if (!isPasswordValid)
                {
                    return new Status { Code = "1002", Message = "Invalid current password", Data = null };
                }

                // Update password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.Password = hashedPassword;
                await _context.SaveChangesAsync();

                // Optionally, send an email notification for password change (can be customized)
                var emailSent = _emailService.SendEmail(user.Email, "Password Updated", "Your password has been successfully updated.");
                if (!emailSent)
                {
                    return new Status { Code = "1005", Message = "Failed to send password update notification", Data = null };
                }

                return new Status { Code = "0000", Message = "Password updated successfully", Data = null };
            }
            catch (Exception ex)
            {
                return new Status { Code = "1111", Message = "Internal Server Error", Data = ex.Message };
            }
        }
        public async Task<Status> DeleteAccount(DeleteAccountRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return new Status { Code = "1004", Message = "User not found", Data = null };
            }

            // Verify the password
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordValid)
            {
                return new Status { Code = "1002", Message = "Invalid password", Data = null };
            }

            // Proceed with account deletion
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new Status { Code = "0000", Message = "Account deleted successfully", Data = null };
        }

        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; 
            return Regex.IsMatch(email, emailPattern);
        }
    }
}
