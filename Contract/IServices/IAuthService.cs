using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technical_Assessment_ElectroPi.Core.Entities;

namespace Technical_Assessment_ElectroPi.Contract
{
    public interface IAuthService
    {

        Task<IdentityResult> RegisterAsync(User model);
        Task<string> LoginAsync(User model);

    }
}
