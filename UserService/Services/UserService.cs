using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using UserService.DBModels;
using UserService.Models;

namespace UserService.Services;

public interface IUserService
{
    public ValueTask<UserDTO> GetUser(int id);
    public ValueTask<bool> UpdateUserProfile(int userId, UserUpdateDTO userUpdateDto);
    public ValueTask<int?> GetRoleId(string roleName);
    public ValueTask<bool> AddSkill(int userId, int skillId);
    public ValueTask<bool> RemoveSkill(int userId, int skillId);
    public ValueTask<bool> UpdateFCM(FCMUpdateDTO fcm, int userId);
    public ValueTask<List<SkillDTO>> GetSkills();
}

public class UserService(HackatonContext _context) : IUserService
{
    public async ValueTask<UserDTO> GetUser(int id)
    {
        var user = await _context.Users.Include(u => u.RoleNavigation).FirstOrDefaultAsync(u => u.Id == id);
        return new UserDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ThirdName = user.ThirdName,
            Role = user.RoleNavigation.Name,
        };
    }

    public async ValueTask<int?> GetRoleId(string roleName)
    {
        return await _context.Roles.Where(r => r.Name == roleName)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();
    }

    public async ValueTask<bool> AddSkill(int userId, int skillId)
    {
        if (!_context.Skills.Any(s => s.Id == skillId))
        {
            return false;
        }
        
        if (await _context.UsersSkills.AnyAsync(us => us.UserId == userId && us.SkillId == skillId))
        {
            return false;
        }
        
        await _context.UsersSkills.AddAsync(new UsersSkill
        {
            SkillId = skillId,
            UserId = userId,
        });
        
        return await _context.SaveChangesAsync() > 0;
    }

    public async ValueTask<bool> RemoveSkill(int userId, int skillId)
    {
        var skill = await _context.UsersSkills.Where(us => us.UserId == userId && us.SkillId == skillId).Select(us => us).FirstOrDefaultAsync();
        if (skill == null)
            return false;
        
        _context.UsersSkills.Remove(skill);
        return await _context.SaveChangesAsync() > 0;
    }

    public async ValueTask<bool> UpdateFCM(FCMUpdateDTO fcm, int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        user.FcmToken = fcm.FcmToken;
        _context.Update(user);
        
        return await _context.SaveChangesAsync() > 0;
    }

    public async ValueTask<List<SkillDTO>> GetSkills()
    {
        return await _context.Skills.Select(s => new SkillDTO
        {
            Id = s.Id,
            Name = s.Name,
        }).ToListAsync();
    }


    public async ValueTask<bool> UpdateUserProfile(int userId, UserUpdateDTO userUpdateDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        var roleName = userUpdateDto.Role ?? user.RoleNavigation?.Name;
        var roleId = await GetRoleId(roleName);
        user.FirstName = userUpdateDto.FirstName ?? user.FirstName;
        user.LastName = userUpdateDto.LastName ?? user.LastName;
        user.Email = userUpdateDto.Email ?? user.Email;
        user.ThirdName = userUpdateDto.Patronymic ?? user.ThirdName;
        user.Role = roleId ?? user.RoleNavigation.Id;
        _context.Users.Update(user);
        return await _context.SaveChangesAsync() > 0;
    }
}