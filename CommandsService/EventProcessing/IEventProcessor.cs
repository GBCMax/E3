namespace CommandsService.EventProcessing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string _message);
    }
}