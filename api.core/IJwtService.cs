using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace api.core
{
    public interface IJwtService
    {
        string GenerateSecurityToken(string email, byte[] salt, string[] roles);

        bool ValidateToken(string token, byte[] salt);
    }
}
