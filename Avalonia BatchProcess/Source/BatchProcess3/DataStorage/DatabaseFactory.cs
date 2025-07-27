using BatchProcess3.ViewModels;
using System;

namespace BatchProcess3.DataStorage;

public class DatabaseFactory(Func<DatabaseService> factory)
{
    public DatabaseService GetDatabaseService(Action<DatabaseService>? afterCreation = null)
    {
        var databaseService = factory();
        
        afterCreation?.Invoke(databaseService);
        
        return databaseService;
    }
}