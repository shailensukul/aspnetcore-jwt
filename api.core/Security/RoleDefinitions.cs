using System;
using System.Collections.Generic;
using System.Text;

namespace api.core.Security
{
    public sealed class RoleDefinitions
    {
        public const string Administrator = "Administrator";
        public const string Customer = "Customer";
        public const string Office = "Office";
        public const string Everyone = "Everyone";

        private RoleDefinitions()
        { }

        public static readonly HashSet<string> All = new HashSet<string>()
        {
            Administrator,
            Customer,
            Office,
            Everyone
        };
    }
}
