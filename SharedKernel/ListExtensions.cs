namespace SharedKernel;

public static class ListExtensions
{
    
    public static List<T> ReverseList<T>(this List<T> list)
    {
        list.Reverse();
        return list.ToList();
    }
    
    public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
    {
        foreach (var value in list)
        {
            await func(value);
        }
    }
}