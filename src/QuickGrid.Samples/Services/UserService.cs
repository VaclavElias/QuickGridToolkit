namespace QuickGrid.Samples.Services;

public static class UserService
{
    // Add random remote working flag to some users
    public static List<UserDto> GetUsers() =>
    [
            new() { Id = 1078, Name = "Jack", Country = "China", Age = 19, RemoteWorking = true },
            new() { Id = 1189, Name = "Katherine", Country = "India", Age = 36, RemoteWorking = false },
            new() { Id = 1290, Name = "Lucas", Country = "Spain", Age = 27, RemoteWorking = true },
            new() { Id = 1311, Name = "Mia", Country = "Argentina", Age = 31, RemoteWorking = false },
            new() { Id = 1412, Name = "Noah", Country = "Japan", Age = 24, RemoteWorking = true },
            new() { Id = 1513, Name = "Olivia", Country = "Italy", Age = 42, RemoteWorking = false },
            new() { Id = 1614, Name = "Patrick", Country = "South Korea", Age = 38, RemoteWorking = true },
            new() { Id = 1674, Name = "Alice", Country = "USA", Age = 25, RemoteWorking = false },
            new() { Id = 1715, Name = "Quinn", Country = "South Africa", Age = 47, RemoteWorking = true },
            new() { Id = 1816, Name = "Ryan", Country = "Nigeria", Age = 26, RemoteWorking = false },
            new() { Id = 1917, Name = "Sophia", Country = "Egypt", Age = 33, RemoteWorking = true },
            new() { Id = 2018, Name = "Thomas", Country = "Kenya", Age = 30, RemoteWorking = false },
            new() { Id = 2753, Name = "Bob", Country = "Canada", Age = 32, RemoteWorking = true },
            new() { Id = 3835, Name = "Charlie", Country = "Mexico", Age = 45, RemoteWorking = true },
            new() { Id = 4653, Name = "David", Country = "Germany", Age = 22, RemoteWorking = true },
            new() { Id = 5536, Name = "Emma", Country = "France", Age = 39, RemoteWorking = true },
            new() { Id = 6634, Name = "Frank", Country = "UK", Age = 28, RemoteWorking = true },
            new() { Id = 7666, Name = "Grace", Country = "Australia", Age = 41, RemoteWorking = true },
            new() { Id = 8267, Name = "Henry", Country = "Russia", Age = 50, RemoteWorking = true },
            new() { Id = 9676, Name = "Isabella", Country = "Brazil", Age = 29, RemoteWorking = true }
        ];
}