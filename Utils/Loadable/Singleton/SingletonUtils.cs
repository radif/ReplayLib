using Replay.Utils;

public static class SingletonUtils
{
    public static void HandleStartupInterfaceSupport<T>(T singleton)
    {
        //IReplaySerialazable
        var serializable = singleton as IReplaySerialazable; 
        serializable?.Deserialize();
    }
}
