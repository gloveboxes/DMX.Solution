using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeToShineClient.Model.Entity
{
    public class UserColor
    {
        public int Id { get; set; }
        public string ColorName { get; set; }
        public bool? Approved { get; set; }
        public DateTime? Submitted { get; set; }

        public string SubmitterName { get; set; }
        public int SubmitterAge { get; set; }
        public string SubmitterLocation { get; set; }

        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public UserColor()
        {
            Submitted = DateTime.Now;
        }
    }
}
