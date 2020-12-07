using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace api.core.Models
{
    [DataContract]
    public sealed class UserCredentials
    {
        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
