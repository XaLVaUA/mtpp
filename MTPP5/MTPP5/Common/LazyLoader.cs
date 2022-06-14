namespace MTPP5.Common;

public class LazyLoader<T>
{
    private readonly Func<Task<T?>> _valueFactory;

    private T? _value;
    private Func<Task<T?>> _getValue;

    public LazyLoader(Func<Task<T?>> valueFactory)
    {
        _getValue = AcquireValueAsync;
        _valueFactory = valueFactory;
    }

    public Task<T?> GetValueAsync() => _getValue();

    private async Task<T?> AcquireValueAsync()
    {
        _value = await _valueFactory();
        _getValue = ReturnValueAsync;
        return await GetValueAsync();
    }

    private Task<T?> ReturnValueAsync() => Task.FromResult(_value);
}
