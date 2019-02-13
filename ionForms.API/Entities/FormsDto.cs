using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ionForms.API.Entities
{
    public class FormsDto
    {
        public int Id { get; set; }
        public string AccountID { get; set; }
        public string FormID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
