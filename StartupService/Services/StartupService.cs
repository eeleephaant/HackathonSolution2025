using Microsoft.EntityFrameworkCore;
using StartupService.DBModels;
using StartupService.Models;

namespace StartupService.Services;

public interface IStartupService
{
    public ValueTask<bool> AddNew(NewStartupDTO newStartup, User byUser);
    public ValueTask<StartupDTO?> GetStartup(int startupId);
    public ValueTask<List<StartupDTO>> GetStartups();
    public ValueTask<bool> AddWorker(int userId, int startupId);
    public ValueTask<bool> AddScientist(int startupId, int userId);
    public ValueTask<bool> AddInvestor(int startupId, int userId);
    public ValueTask<bool> RemoveUserFromStartup(int startupId, int userId);
}

public class StartupService(HackatonContext _context) : IStartupService
{
    public async ValueTask<bool> AddNew(NewStartupDTO newStartup, User byUser)
    {
        await _context.Startups.AddAsync(new Startup
        {
            Name = newStartup.Name,
            Description = newStartup.Description,
            CreatedAt = DateTime.UtcNow,
            OwnerId = byUser.Id,
        });
        return await _context.SaveChangesAsync() > 0;
    }

    public async ValueTask<bool> AddScientist(int startupId, int userId)
    {
        var startup = await _context.Startups.FindAsync(startupId);
        if (startup == null)
            return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        startup.StartupScientists.Add(new StartupScientist
        {
            UserId = userId,
            StartupId = startupId,
        });
        
        return await _context.SaveChangesAsync() > 0;
    }    
    
    public async ValueTask<bool> AddInvestor(int startupId, int userId)
    {
        var startup = await _context.Startups.FindAsync(startupId);
        if (startup == null)
            return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        startup.StartupInvestors.Add(new StartupInvestor
        {
            InvestorId = userId,
            StartupId = startupId,
        });
        
        return await _context.SaveChangesAsync() > 0;
    }

    public async ValueTask<bool> AddWorker(int userId, int startupId)
    {
        var startup = await _context.Startups.FindAsync(startupId);
        if (startup == null)
            return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        startup.StartupWorkers.Add(new StartupWorker
        {
            UserId = userId,
            StartupId = startupId,
        });
        
        return await _context.SaveChangesAsync() > 0;
    }

    public async ValueTask<bool> RemoveUserFromStartup(int startupId, int userId)
    {
        var startup = await _context.Startups.FindAsync(startupId);
        if (startup == null)
            return false;

        var userWorker = await _context.StartupWorkers.FindAsync(userId);
        if (userWorker != null)
        {
            _context.StartupWorkers.Remove(userWorker);
            return true;
        }

        var userInvestor = await _context.StartupInvestors.FindAsync(userId);
        if (userInvestor != null)
        {
            _context.StartupInvestors.Remove(userInvestor);
        }

        var userScientist = await _context.StartupScientists.FindAsync(userId);
        if (userScientist != null)
        {
            _context.StartupScientists.Remove(userScientist);
        }

        return await _context.SaveChangesAsync() > 0;
    }

    public async ValueTask<List<StartupDTO>> GetStartups()
    {
        var startups = await _context.Startups.ToListAsync();

        var startupDtos = new List<StartupDTO>();

        foreach (var startup in startups)
        {
            var startupDto = await GetStartup(startup.Id);
            if (startupDto != null)
            {
                startupDtos.Add(startupDto);
            }
        }

        return startupDtos;
    }

    public async ValueTask<StartupDTO?> GetStartup(int startupId)
    {
        var startup = await _context.Startups.FindAsync(startupId);

        if (startup == null)
            return null;

        var investors = startup.StartupInvestors.Select(si => si.Investor).Select(inv => new UserDTO
        {
            Id = inv.Id,
            FirstName = inv.FirstName,
            LastName = inv.LastName,
            ThirdName = inv.ThirdName
        }).ToList();
        var scientists = startup.StartupScientists.Select(si => si.User).Select(scientist => new UserDTO
        {
            Id = scientist.Id,
            FirstName = scientist.FirstName,
            LastName = scientist.LastName,
            ThirdName = scientist.ThirdName
        }).ToList();
        var workers = startup.StartupWorkers.Select(si => si.User).Select(worker => new UserDTO
        {
            Id = worker.Id,
            FirstName = worker.FirstName,
            LastName = worker.LastName,
            ThirdName = worker.ThirdName
        }).ToList();

        return new StartupDTO
        {
            Id = startup.Id,
            Name = startup.Name,
            Description = startup.Description,
            CreatedAt = startup.CreatedAt,
            OwnerUser = new UserDTO
            {
                Id = startup.OwnerId ?? 0,
                FirstName = startup.Owner.FirstName,
                LastName = startup.Owner.LastName,
                ThirdName = startup.Owner.ThirdName
            },
            Scientists = scientists,
            Investors = investors,
            Workers = workers,
        };
    }
}