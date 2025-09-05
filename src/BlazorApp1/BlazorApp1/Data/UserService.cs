namespace BlazorApp1.Data;

public static class UserService
{
    public static List<UserDto> GetUsers() => new()
    {
            new() { Id = 1, Name = "Alice", Country = "USA", Age = 25 },
            new() { Id = 2, Name = "Bob", Country = "Canada", Age = 32 },
            new() { Id = 3, Name = "Charlie", Country = "Mexico", Age = 45 },
            new() { Id = 4, Name = "David", Country = "Germany", Age = 22 },
            new() { Id = 5, Name = "Emma", Country = "France", Age = 39 },
            new() { Id = 6, Name = "Frank", Country = "UK", Age = 28 },
            new() { Id = 7, Name = "Grace", Country = "Australia", Age = 41 },
            new() { Id = 8, Name = "Henry", Country = "Russia", Age = 50 },
            new() { Id = 9, Name = "Isabella", Country = "Brazil", Age = 29 },
            new() { Id = 10, Name = "Jack", Country = "China", Age = 19 },
            new() { Id = 11, Name = "Katherine", Country = "India", Age = 36 },
            new() { Id = 12, Name = "Lucas", Country = "Spain", Age = 27 },
            new() { Id = 13, Name = "Mia", Country = "Argentina", Age = 31 },
            new() { Id = 14, Name = "Noah", Country = "Japan", Age = 24 },
            new() { Id = 15, Name = "Olivia", Country = "Italy", Age = 42 },
            new() { Id = 16, Name = "Patrick", Country = "South Korea", Age = 38 },
            new() { Id = 17, Name = "Quinn", Country = "South Africa", Age = 47 },
            new() { Id = 18, Name = "Ryan", Country = "Nigeria", Age = 26 },
            new() { Id = 19, Name = "Sophia", Country = "Egypt", Age = 33 },
            new() { Id = 20, Name = "Thomas", Country = "Kenya", Age = 30 }
        };
}