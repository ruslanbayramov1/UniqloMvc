using UniqloMvc.Enums;

namespace UniqloMvc.Extensions;

public static class RoleExtension
{
    public static string GetRole(this Roles role)
    {
        string res = role switch
        {
            Roles.Admin => nameof(Roles.Admin),
            Roles.User => nameof(Roles.User),
            Roles.SMM => nameof(Roles.SMM),
            Roles.Cashier => nameof(Roles.Cashier),
        };
        return res; 
    }
}
