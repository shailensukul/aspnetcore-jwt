using api.core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace api.data.Models
{
    public sealed class User : IEntity
    {
        public User()
        {
            this.Inititialize(new StreamingContext());
        }

        [Key]
        [EmailAddress]
        [Required]
        [DataMember]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public byte[] Salt { get; set; }

        [StringLength(50)]
        [DataMember]
        public string FirstName { get; set; }

        [StringLength(50)]
        [DataMember]
        public string LastName { get; set; }

        [NotMapped]
        public string[] Roles
        {
            get
            {
                return (this.RolesString ?? string.Empty).Split(",");
            }
            set
            {
                this.RolesString = string.Join(",", (value ?? new string[] { }));
            }
        }

        [DataMember]
        public string RolesString
        {
            get; set;
        }

        public void Inititialize(StreamingContext context)
        {
        }

    }
}
