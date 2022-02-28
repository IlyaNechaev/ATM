using ATMApplication.Data;

namespace ATMApplication.Services;

public static class JWTSignIn
{
    public static string UsingJWT(this LogIn logIn, User user)
    {
        var jwtUtils = logIn.GetService<IJwtUtils>();
        var repositoryFactory = logIn.GetService<IRepositoryFactory>();

        return jwtUtils.GenerateJSONWebToken(user);
    }

    public static void UsingJWT(this LogOut logOut, string token)
    {
        var jwtUtils = logOut.GetService<IJwtUtils>();
        var repositoryFactory = logOut.GetService<IRepositoryFactory>();
    }
}