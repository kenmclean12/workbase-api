namespace newproj.Services.Users;

public enum UserUpdateResult
{
    UserNotFound,
    CannotManageRole,
    CannotAssignRole,
    DuplicateEmail,
    Success
}