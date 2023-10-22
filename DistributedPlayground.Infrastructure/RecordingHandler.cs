namespace DistributedPlayground.Infrastructure;

public class RecordingHandler : IHandle
{
	private readonly List<Message> _handledMessages = new();

	public IReadOnlyList<Message> HandledMessages => _handledMessages;

	public void Clear()
	{
		_handledMessages.Clear();
	}

	public void Handle(Message message)
	{
		_handledMessages.Add(message);
	}
}
