using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace api.core.Security
{
    /// <summary>
    /// This attribute is for the purpose of adding optional (OR) role authorizations, like in the example
    /// </summary>
    /// <example>
    ///  [AuthorizeRoles(RoleDefinitions.Administrator, RoleDefinitions.Customer)]
    /// </example>
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }
    }
}
