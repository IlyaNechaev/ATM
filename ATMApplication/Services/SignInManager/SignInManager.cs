namespace ATMApplication.Services;

public class SignInManager
{
    public IServiceProvider ServiceProvider { get; set; }
    public SignInManager(IServiceProvider provider)
    {
        ServiceProvider = provider;
    }
    public LogIn LogIn()
    {
        return new LogIn(this);
    }
    public LogOut LogOut()
    {
        return new LogOut(this);
    }
}

public class LogIn
{
    private SignInManager _manager { get; set; }
    public LogIn(SignInManager manager)
    {
        _manager = manager;
    }

    public T GetService<T>()
    {
        return (T)_manager.ServiceProvider.GetService(typeof(T));
    }
}

public class LogOut
{
    private SignInManager _manager { get; set; }
    public LogOut(SignInManager manager)
    {
        _manager = manager;
    }

    public T GetService<T>()
    {
        return (T)_manager.ServiceProvider.GetService(typeof(T));
    }
}