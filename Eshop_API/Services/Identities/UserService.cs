using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using eshop_api.Entities;
using eshop_api.Helpers;
using eshop_pbl6.Entities;
using eshop_pbl6.Helpers.Identities;
using eshop_pbl6.Models.DTO.Identities;

namespace eshop_pbl6.Services.Identities
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Permission>> GetAllPermission()
        {
            return await Task.FromResult(_context.Permissions.ToList());
        }

        public async Task<List<RoleDto>> GetAllRoles()
        {
            var roles = _context.Roles.ToList();
            List<RoleDto> roleDtos = new List<RoleDto>();
            foreach (var item in roles)
            {
                RoleDto roleDto = new RoleDto();
                roleDto.Id = item.Id;
                roleDto.Name = item.Name.ToString();
                roleDtos.Add(roleDto);
            }
            return await Task.FromResult(roleDtos);
        }

        public async Task<User> GetById(int idUser)
        {
            return await Task.FromResult(_context.AppUsers.FirstOrDefault(x => x.Id == idUser));
        }

        public async Task<List<string>> GetPermissionByUser(int idUser)
        {
            var role = (from us in _context.AppUsers
                        join r in _context.Roles on us.RoleId equals r.Id
                        join rip in _context.RoleInPermissions on r.Id equals rip.RoleId
                        join p in _context.Permissions on rip.PermissionId equals p.Id
                        where (us.Id == idUser)
                        select (p.name)).ToList();
            if (role != null)
            {
                return await Task.FromResult(role);
            }
            throw null;
        }

        public Task<UserDto> GetUsersDto()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Login(string username,string password)
        {
            username = username.ToLower().Trim();
            var currentUser = _context.AppUsers
                .FirstOrDefault(u => u.Username.ToLower().Trim() == username);
            if (currentUser == null)
            {
                return false;
            }
            using var hmac = new HMACSHA512(currentUser.PasswordSalt);
            var passwordBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < currentUser.PasswordHash.Length; i++)
            {
                if (currentUser.PasswordHash[i] != passwordBytes[i])
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<User> Register(CreateUpdateUserDto create)
        {
            throw new NotImplementedException();
        }
        
        public async Task<bool> ChangePassword(string username, string passwordOld, string passwordNew)
        {
            if(await Login(username,passwordOld) == true){
                var user = _context.AppUsers.FirstOrDefault(x =>x.Username.ToLower() == username.ToLower().Trim());
                using var hmac = new HMACSHA512();
                var passwordBytes = Encoding.UTF8.GetBytes(passwordNew);
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(passwordBytes);
                _context.AppUsers.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<UserDto> UpdateUserById(UpdateUserDto userDto,int idUser)
        {
            try{
                var user = _context.AppUsers.FirstOrDefault(x => x.Id == idUser);
                if(user != null){
                    user.Phone  = userDto.Phone ?? user.Phone;
                    user.BirthDay = userDto.BirthDay ?? user.BirthDay;
                    user.Email = userDto.Email ?? user.Email;
                    user.FirstName  = userDto.FirstName ?? user.FirstName;
                    user.LastName = userDto.LastName ?? user.LastName;
                    user.Gender = userDto.Gender ?? user.Gender;
                    var result = _context.AppUsers.Update(user);
                    await _context.SaveChangesAsync();
                    var json = JsonSerializer.Serialize(userDto);
                    var userDtoResult = JsonSerializer.Deserialize<UserDto>(json);
                    return userDtoResult;
                }
                return null;
            }
            catch(Exception ex){
                throw ex;
            }
        }
    }
}