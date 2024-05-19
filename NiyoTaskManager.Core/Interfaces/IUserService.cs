using NiyoTaskManager.Data.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Core.Interfaces
{
    public interface IUserService
    {
        Task<SignInResultDTO> SignInAsync(SignInDTO model);
        Task<SignInResultDTO> SignUpAsync(SignUpDTO model);
        Task DeleteAccountAsync(string id);
        Task<UserBindingDTO> FetchUserAsync(string email);
        Task<List<UserBindingDTO>> FetchAllUsers();
    }
}
