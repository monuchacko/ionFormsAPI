using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ionForms.API.Entities
{
    public class AccountsDto
    {
        public int Id { get; set; }
        public string AccountID { get; set; }
        public string Title { get; set; }

        public int NumberOfForms
        {
            get
            {
                return Forms.Count;
            }
        }

        public ICollection<FormsDto> Forms { get; set; }
        = new List<FormsDto>();

    }
}
