namespace KrasnyyOktyabr.ComV77ApplicationConnection;

public sealed class ComV77ApplicationConnection : IComV77ApplicationConnection
{
    private readonly IComV77ApplicationConnection.Properties _properties;

    private ComV77ApplicationConnection(IComV77ApplicationConnection.Properties properties)
    {
        _properties = properties;
    }

    public Task ConnectAsync(IComV77ApplicationConnection.Properties properties, CancellationToken cancellationToken) => throw new NotImplementedException();

    public ValueTask DisposeAsync() => throw new NotImplementedException();

    /// <remarks>
    /// Is nested because needs access to <see cref="ComV77ApplicationConnection(IComV77ApplicationConnection.Properties)"/> private constructor.
    /// </remarks>
    public sealed class Factory : IComV77ApplicationConnectionFactory<ComV77ApplicationConnection>
    {
        private readonly SemaphoreSlim _factoryLock = new(1);

        private readonly Dictionary<IComV77ApplicationConnection.Properties, ComV77ApplicationConnection> _propertiesConnections = [];

        public async Task<ComV77ApplicationConnection> GetConnectionAsync(string infobasePath, string username, string password)
        {
            IComV77ApplicationConnection.Properties connectionProperties = new(infobasePath, username, password);

            await _factoryLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (_propertiesConnections.TryGetValue(connectionProperties, out ComV77ApplicationConnection? connection))
                {
                    return connection;
                }

                ComV77ApplicationConnection newConnection = new(connectionProperties);

                _propertiesConnections.Add(connectionProperties, newConnection);

                return newConnection;
            }
            finally
            {
                _factoryLock.Release();
            }
        }

        public ValueTask DisposeAsync() => throw new NotImplementedException();
    }
}
