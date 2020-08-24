using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MakeMagic.Models
{   
    [Table("Characters")]
    public class Character
    {
        public int Id { get; protected set; }

        public string Name { get; protected set; }

        public string Role { get; protected set; }

        public string School { get; protected set; }

        public string House { get; protected set; }

        public string Patronus { get; protected set; }


        public virtual void Update(string name, string role, string school, string house, string patronus)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentNullException(nameof(role));
            if (string.IsNullOrWhiteSpace(school))
                throw new ArgumentNullException(nameof(school));
            if (string.IsNullOrWhiteSpace(house))
                throw new ArgumentNullException(nameof(house));
            if (string.IsNullOrWhiteSpace(patronus))
                throw new ArgumentNullException(nameof(patronus));

            Name = name;
            Role = role;
            School = school;
            House = house;
            Patronus = patronus;
        }

        public Character(string name, string role, string school, string house, string patronus)
        {
            Update(name, role, school, house, patronus);
        }

        protected Character()
        {

        }
    }
}
