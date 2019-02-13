using ionForms.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ionForms.API.DataStore
{
    public class FormsDataStore
    {
        public static FormsDataStore Current { get; } = new FormsDataStore();

        public List<FormsDto> Forms { get; set; }

        public FormsDataStore()
        {
            Forms = new List<FormsDto>()
            { };
        }
    }
}
