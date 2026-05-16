namespace CartService.UnitTests;

internal abstract class ServiceTestBase<TService> : TestBase
{
    /// <summary>
    /// Method that gets an instance of service.
    /// </summary>
    protected abstract TService GetService();

    /// <summary>
    /// Method that creates instances for every necessary dependency.
    /// </summary>
    protected abstract void InstantiatedDependencies();
}