using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;

namespace api.data.Commands
{
    [DataContract(Name ="AddRoles", Namespace = Namespace.CommandNamespace)]
    public sealed class AddRolesCommand
    {
        [DataMember]
        public string UserId
        {
            get;
            set;
        } 

        [DataMember]
        public Collection<string> Roles
        {
            get;
            set;
        }

        [OnDeserialized]
        public void Initialize(StreamingContext context)
        {
            this.Roles ??= new Collection<string>();
        }
    }
}
