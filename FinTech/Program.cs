using Microsoft.Extensions.DependencyInjection;

namespace FinTech;

public class Program
{
    public static void Main()
    {
        // Настройка DI-контейнера
        var serviceCollection = new ServiceCollection();

        // Регистрируем FinanceManager и прокси для него
        serviceCollection.AddSingleton<FinanceManager>();
        serviceCollection.AddSingleton<IFinanceManager>(provider =>
            new FinanceManagerProxy(provider.GetRequiredService<FinanceManager>()));

        // Регистрируем команды как Transient
        serviceCollection.AddTransient<CreateBankAccountCommand>();
        serviceCollection.AddTransient<EditBankAccountCommand>();
        serviceCollection.AddTransient<RemoveBankAccountCommand>();
        serviceCollection.AddTransient<ListDataCommand>();

        serviceCollection.AddTransient<CreateCategoryCommand>();
        serviceCollection.AddTransient<EditCategoryCommand>();
        serviceCollection.AddTransient<RemoveCategoryCommand>();

        serviceCollection.AddTransient<CreateOperationCommand>();
        serviceCollection.AddTransient<EditOperationCommand>();
        serviceCollection.AddTransient<RemoveOperationCommand>();

        serviceCollection.AddTransient<AnalyticsCommand>();
        serviceCollection.AddTransient<ImportExportCommand>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nГлавное меню. Выберите область работы:");
            Console.WriteLine("1. Счета");
            Console.WriteLine("2. Категории");
            Console.WriteLine("3. Операции");
            Console.WriteLine("4. Аналитика");
            Console.WriteLine("5. Импорт/Экспорт данных");
            Console.WriteLine("6. Просмотр всех данных");
            Console.WriteLine("0. Выход");
            Console.Write("Ваш выбор: ");
            var mainChoice = Console.ReadLine();

            switch (mainChoice)
            {
                case "1":
                    ShowAccountsMenu(serviceProvider);
                    break;
                case "2":
                    ShowCategoriesMenu(serviceProvider);
                    break;
                case "3":
                    ShowOperationsMenu(serviceProvider);
                    break;
                case "4":
                    var analyticsCmd = serviceProvider.GetRequiredService<AnalyticsCommand>();
                    TryExecuteCommand(analyticsCmd);
                    break;
                case "5":
                    var impExpCmd = serviceProvider.GetRequiredService<ImportExportCommand>();
                    TryExecuteCommand(impExpCmd);
                    break;
                case "6":
                    var listCmd = serviceProvider.GetRequiredService<ListDataCommand>();
                    TryExecuteCommand(listCmd);
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
        }
    }

    private static void ShowAccountsMenu(ServiceProvider provider)
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine("\nМеню счетов:");
            Console.WriteLine("1. Добавить счет");
            Console.WriteLine("2. Редактировать счет");
            Console.WriteLine("3. Удалить счет");
            Console.WriteLine("4. Просмотр счетов");
            Console.WriteLine("0. Назад");
            Console.Write("Ваш выбор: ");
            var choice = Console.ReadLine();
            ICommand command = null;
            switch (choice)
            {
                case "1":
                    command = provider.GetRequiredService<CreateBankAccountCommand>();
                    break;
                case "2":
                    command = provider.GetRequiredService<EditBankAccountCommand>();
                    break;
                case "3":
                    command = provider.GetRequiredService<RemoveBankAccountCommand>();
                    break;
                case "4":
                    command = provider.GetRequiredService<ListDataCommand>();
                    break;
                case "0":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
            if (command != null)
                TryExecuteCommand(command);
        }
    }

    private static void ShowCategoriesMenu(ServiceProvider provider)
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine("\nМеню категорий:");
            Console.WriteLine("1. Добавить категорию");
            Console.WriteLine("2. Редактировать категорию");
            Console.WriteLine("3. Удалить категорию");
            Console.WriteLine("4. Просмотр категорий");
            Console.WriteLine("0. Назад");
            Console.Write("Ваш выбор: ");
            var choice = Console.ReadLine();
            ICommand command = null;
            switch (choice)
            {
                case "1":
                    command = provider.GetRequiredService<CreateCategoryCommand>();
                    break;
                case "2":
                    command = provider.GetRequiredService<EditCategoryCommand>();
                    break;
                case "3":
                    command = provider.GetRequiredService<RemoveCategoryCommand>();
                    break;
                case "4":
                    command = provider.GetRequiredService<ListDataCommand>();
                    break;
                case "0":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
            if (command != null)
                TryExecuteCommand(command);
        }
    }

    private static void ShowOperationsMenu(ServiceProvider provider)
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine("\nМеню операций:");
            Console.WriteLine("1. Добавить операцию");
            Console.WriteLine("2. Редактировать операцию");
            Console.WriteLine("3. Удалить операцию");
            Console.WriteLine("4. Просмотр операций");
            Console.WriteLine("0. Назад");
            Console.Write("Ваш выбор: ");
            var choice = Console.ReadLine();
            ICommand command = null;
            switch (choice)
            {
                case "1":
                    command = provider.GetRequiredService<CreateOperationCommand>();
                    break;
                case "2":
                    command = provider.GetRequiredService<EditOperationCommand>();
                    break;
                case "3":
                    command = provider.GetRequiredService<RemoveOperationCommand>();
                    break;
                case "4":
                    command = provider.GetRequiredService<ListDataCommand>();
                    break;
                case "0":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
            if (command != null)
                TryExecuteCommand(command);
        }
    }

    private static void TryExecuteCommand(ICommand command)
    {
        try
        {
            command.Execute();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
    }
}