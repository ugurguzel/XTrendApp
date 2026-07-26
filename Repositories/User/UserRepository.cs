using Dapper;
using XTrendApp.Web.Data;
using XTrendApp.Web.Models.User;
using BCrypt.Net;

namespace XTrendApp.Web.Repositories.User
{
    public class UserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> GetByUsernameAsync(string username)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<UserModel>(
                @"SELECT *
          FROM Users
          WHERE Username = @Username",
                new { Username = username });
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<UserModel>(
                @"SELECT
            Id,
            Username,
            FullName,
            Email,
            IsAdmin,
            IsActive,
            CreatedAt,
            UpdatedAt,
            LastLogin
          FROM Users
          ORDER BY Username");
        }

        public async Task<int> InsertAsync(UserModel model)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
        INSERT INTO Users
        (
            Username,
            PasswordHash,
            FullName,
            Email,
            IsAdmin,
            IsActive,
            CreatedAt
        )
        VALUES
        (
            @Username,
            @PasswordHash,
            @FullName,
            @Email,
            @IsAdmin,
            @IsActive,
            @CreatedAt
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
    ";

            return await connection.ExecuteScalarAsync<int>(sql, model);
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
        UPDATE Users
        SET IsActive = 0,
            UpdatedAt = GETDATE()
        WHERE Id = @Id
          AND IsActive = 1;";

            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                Id = id
            });

            return affectedRows > 0;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
        UPDATE Users
        SET IsActive = 1,
            UpdatedAt = GETDATE()
        WHERE Id = @Id
          AND IsActive = 0;";

            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                Id = id
            });

            return affectedRows > 0;
        }

        public async Task<UserEditViewModel?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
        SELECT
            Id,
            Username,
            FullName,
            Email,
            IsAdmin,
            IsActive
        FROM Users
        WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<UserEditViewModel>(
                sql,
                new { Id = id });
        }

        public async Task<bool> UpdateAsync(UserEditViewModel model)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
        UPDATE Users
        SET
            Username = @Username,
            FullName = @FullName,
            Email = @Email,
            IsAdmin = @IsAdmin,
            IsActive = @IsActive
        WHERE Id = @Id";

            return await connection.ExecuteAsync(sql, model) > 0;
        }

        public async Task UpdateLastLoginAsync(int id)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
        UPDATE Users
        SET LastLogin = GETDATE()
        WHERE Id=@Id";

            await connection.ExecuteAsync(sql, new
            {
                Id = id
            });
        }

        public async Task<UserModel?> GetUserAsync(int id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<UserModel>(
                @"SELECT *
          FROM Users
          WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<bool> ChangePasswordAsync(UserPasswordViewModel model)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
        UPDATE Users
        SET PasswordHash = @PasswordHash,
            UpdatedAt = GETDATE()
        WHERE Id = @Id";

            var affected = await connection.ExecuteAsync(sql, new
            {
                model.Id,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            });

            return affected > 0;
        }

        public async Task<UserProfileViewModel?> GetProfileAsync(int id)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
        SELECT
            Id,
            Username,
            FullName,
            Email
        FROM Users
        WHERE Id=@Id";

            return await connection.QueryFirstOrDefaultAsync<UserProfileViewModel>(
                sql,
                new
                {
                    Id = id
                });
        }

        public async Task<bool> UpdateProfileAsync(UserProfileViewModel model)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
        UPDATE Users
        SET
            FullName=@FullName,
            Email=@Email,
            UpdatedAt=GETDATE()
        WHERE Id=@Id";

            return await connection.ExecuteAsync(sql, model) > 0;
        }

        public async Task<bool> ChangeMyPasswordAsync(
    int userId,
    string currentPassword,
    string newPassword)
        {
            using var connection = _context.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync<UserModel>(
                @"SELECT *
          FROM Users
          WHERE Id=@Id",
                new { Id = userId });

            if (user == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(
                    currentPassword,
                    user.PasswordHash))
            {
                return false;
            }

            var sql = @"
        UPDATE Users
        SET
            PasswordHash=@PasswordHash,
            UpdatedAt=GETDATE()
        WHERE Id=@Id";

            var affected = await connection.ExecuteAsync(sql, new
            {
                Id = userId,



                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword)
            });

            return affected > 0;
        }


    }
}   