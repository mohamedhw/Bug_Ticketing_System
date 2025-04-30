
using Bug_Ticketing_System.BL.Dtos;
using Bug_Ticketing_System.BL.Dtos.Common;

namespace Bug_Ticketing_System.BL.Mangers.Users
{
    public interface IUserManger
    {
        Task<GeneralResult> AddUserAsync(UserAddDto userAddDto);
        Task<GeneralResult> Login(UserLoginDto userAddDto);

    }
}
