using newproj.Models.Users;

namespace newproj.Helpers;

public static class RoleAuthorization
{
    public static bool CanManageUser(UserRole actor, UserRole target)
    {
        return actor > target;
    }

    public static bool CanAssignRole(UserRole actor, UserRole assigned)
    {
        return actor >= assigned;
    }
}