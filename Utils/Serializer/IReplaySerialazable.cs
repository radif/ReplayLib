namespace Replay.Utils
{
    //this interface will call deserialize on awake automatically if the object inherits from:
    //Singleton
    //ComponentSingleton
    //otherwise both methods should be called manually
    public interface IReplaySerialazable
    {
        void Serialize();
        void Deserialize();
    }

}