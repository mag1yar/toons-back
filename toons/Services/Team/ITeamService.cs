using toons.Models.Team;
using toons.Models.User;

namespace toons.Services.Team
{
    public interface ITeamService
    {
        Task<TeamResponse?> GetListById(int id);
    }
}
