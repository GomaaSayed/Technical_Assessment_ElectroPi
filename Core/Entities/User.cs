
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technical_Assessment_ElectroPi.Core.Entities
{
    public class User : IdentityUser
    {
        [NotMapped]
        public string Password { get; set; }
    }
}
