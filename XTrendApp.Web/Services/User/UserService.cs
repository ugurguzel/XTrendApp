using System.Net.Http;
using XTrendApp.Web.Models.User;
using XTrendApp.Web.Repositories.User;

namespace XTrendApp.Web.Services.User
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<UserEditViewModel?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<int> InsertAsync(UserCreateViewModel model)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(model.Username);

            if (existingUser != null)
            {
                throw new Exception("This username is already in use.");
            }

            var user = new UserModel
            {
                Username = model.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                FullName = model.FullName,
                Email = model.Email,
                IsAdmin = model.IsAdmin,
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now
            };

            return await _userRepository.InsertAsync(user);
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            return await _userRepository.DeactivateAsync(id);
        }

        public async Task<bool> ActivateAsync(int id)
        {
            return await _userRepository.ActivateAsync(id);
        }

        public async Task<bool> UpdateAsync(UserEditViewModel model)
        {
            return await _userRepository.UpdateAsync(model);
        }

        public async Task<bool> ChangePasswordAsync(UserPasswordViewModel model)
        {
            return await _userRepository.ChangePasswordAsync(model);
        }

        public async Task<UserProfileViewModel?> GetProfileAsync(int id)
        {
            return await _userRepository.GetProfileAsync(id);
        }

        public async Task<bool> UpdateProfileAsync(UserProfileViewModel model)
        {
            return await _userRepository.UpdateProfileAsync(model);
        }
    }
}