using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockerRoom.Core.Entities
{
    public class Locker
    {
        [Key]
        public int LockerID { get; set; }
        public string LockerCode { get; set; } = string.Empty;
        public int GroupID { get; set; }
        public string Gender { get; set; } = "Male";
        public bool IsOccupied { get; set; } = false;

        public LockerGroup? Group { get; set; }
    }
}
