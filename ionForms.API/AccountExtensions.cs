using ionForms.API.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ionForms.API
{
    public static class AccountExtensions
    {
        public static void EnsureSeedDataForContext(this FDDataContext context)
        {
            if (context.Accounts.Any())
            {
                return;
            }

            // init seed data
            var accounts = new List<Account>()
            {
                new Account()
                {
                     Title = "New York City",
                     Description = "The one with that big park.",
                     Forms = new List<Form>()
                     {
                         new Form() {
                             Title = "Central Park",
                             Description = "The most visited urban park in the United States."
                         },
                          new Form() {
                             Title = "Empire State Building",
                             Description = "A 102-story skyscraper located in Midtown Manhattan."
                          },
                     }
                },
                new Account()
                {
                    Title = "Antwerp",
                    Description = "The one with the cathedral that was never really finished.",
                    Forms = new List<Form>()
                     {
                         new Form() {
                             Title = "Cathedral",
                             Description = "A Gothic style cathedral, conceived by architects Jan and Pieter Appelmans."
                         },
                          new Form() {
                             Title = "Antwerp Central Station",
                             Description = "The the finest example of railway architecture in Belgium."
                          },
                     }
                },
                new Account()
                {
                    Title = "Paris",
                    Description = "The one with that big tower.",
                    Forms = new List<Form>()
                     {
                         new Form() {
                             Title = "Eiffel Tower",
                             Description =  "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel."
                         },
                          new Form() {
                             Title = "The Louvre",
                             Description = "The world's largest museum."
                          },
                     }
                }
            };

            context.Accounts.AddRange(accounts);
            context.SaveChanges();
        }
    }
}
