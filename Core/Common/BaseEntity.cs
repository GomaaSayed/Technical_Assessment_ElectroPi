using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Technical_Assessment_ElectroPi.Core.Common
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? CreateBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? UpdatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; } = DateTime.Now;


    }
}
