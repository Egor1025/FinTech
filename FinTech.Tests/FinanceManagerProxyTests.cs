namespace FinTech.Tests;

public class FinanceManagerProxyTests
{
    [Fact]
    public void GetBankAccounts_ReturnsCachedAccounts()
    {
        // Arrange
        var manager = new FinanceManager();
        var account = DomainFactory.CreateBankAccount("Test", 100);
        manager.AddBankAccount(account);

        var proxy = new FinanceManagerProxy(manager);

        // Act
        var firstCall = proxy.GetBankAccounts().ToList();
        var secondCall = proxy.GetBankAccounts().ToList();

        // Assert: обе выборки должны совпадать, так как используется кэш
        Assert.Equal(firstCall.Count, secondCall.Count);
        Assert.Equal(firstCall.First().Id, secondCall.First().Id);
    }

    [Fact]
    public void AddBankAccount_InvalidatesCache()
    {
        // Arrange
        var manager = new FinanceManager();
        var proxy = new FinanceManagerProxy(manager);
        var account1 = DomainFactory.CreateBankAccount("Test1", 100);
        manager.AddBankAccount(account1);

        var initial = proxy.GetBankAccounts().ToList();
        Assert.Single(initial);

        // Act: добавляем новый счет через прокси, что должно сбросить кэш
        proxy.AddBankAccount(DomainFactory.CreateBankAccount("Test2", 200));
        var updated = proxy.GetBankAccounts().ToList();

        // Assert: обновленный список должен содержать 2 счета
        Assert.Equal(2, updated.Count);
    }
}