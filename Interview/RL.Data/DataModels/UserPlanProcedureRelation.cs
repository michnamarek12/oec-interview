using RL.Data.DataModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RL.Data.DataModels
{
    public class ProcedureUser : IChangeTrackable
    {
        public int UserId { get; set; }
        public int ProcedureId { get; set; }
        public User User { get; set; }
        public Procedure Procedure { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
