using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockerRoom.Core.Entities
{
    public class LockerGroup
    {
        [Key]
        public int GroupID { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public ICollection<Locker> Lockers { get; set; } = new List<Locker>();
    }
}
