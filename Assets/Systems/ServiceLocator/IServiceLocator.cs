public interface IServiceLocator
{
    public void Register<T>(T service) where T : class;
    public T Get<T>() where T : class;

    void RegisterFor<T>(object obj, T service) where T : class;
    T GetFor<T>(object obj) where T : class;
}
