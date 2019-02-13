using ionForms.API.Entities;
using System.Collections.Generic;

namespace ionForms.API.DataStore
{
    public class AccountsDataStore
    {
        public static AccountsDataStore Current { get; } = new AccountsDataStore();

        public List<AccountsDto> Accounts { get; set; }

        public AccountsDataStore()
        {
            Accounts = new List<AccountsDto>()
            { };

            //Accounts = new List<AccountsDto>()
            //{
            //    new AccountsDto
            //    {
            //        Id = 1,
            //        AccountID = "cid_1",
            //        Name = "Accts 1",
            //        Forms = new List<FormsDto>()
            //        {
            //            new FormsDto()
            //            {
            //                Id = 1,
            //                FormID = "frm_1",
            //                Name = "Form 1",
            //                Description = "desc form"
            //            },
            //            new FormsDto()
            //            {
            //                Id = 1,
            //                FormID = "frm_2",
            //                Name = "Form 2",
            //                Description = "desc form"
            //            }
            //        }
            //    },
            //    new AccountsDto
            //    {
            //        Id = 2,
            //        AccountID = "cid_t_40460136_bdac_4fb1_a5cc_403826c99dec",
            //        Name = "Accts 2",
            //        Forms = new List<FormsDto>()
            //        {
            //            new FormsDto()
            //            {
            //                Id = 1,
            //                FormID = "fid_t_bd122b13_d98a_45b4_ad08_b3d74a519c00",
            //                Name = "Form 1",
            //                Description = "desc form"
            //            },
            //            new FormsDto()
            //            {
            //                Id = 1,
            //                FormID = "fid_t_bd122b13_d98a_45b4_ad08_b3d74a519c01",
            //                Name = "Form 2",
            //                Description = "desc form"
            //            }
            //        }
            //    }
            //};

        }
    }
}
