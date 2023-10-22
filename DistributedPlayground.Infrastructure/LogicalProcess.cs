using DistributedPlayground.Infrastructure.Messages;

namespace DistributedPlayground.Infrastructure;

public abstract class LogicalProcess : ILogicalProcess
{
	public IHandle Output { get; set; } = NoOpHandler.Instance;

	public LogicalProcess(string id)
	{
		Id = id;
	}

	public string Id { get; }

	public abstract void Handle(Message message);

	protected static void HandleImpl(Message message)
	{
		// message with no particular handler is no-op
	}

	public abstract void Start();

	protected void Send<T>(string to, T message, Delay? callbackIn = null) where T : Message
	{
		var m = message with
		{
			From = Id,
			To = to,
		};

		Output.Handle(m);

		if (callbackIn != null)
		{
			Output.Handle(new Callback<T>(callbackIn.Value, m)
			{
				From = Id,
				To = Id,
			});
		}
	}

	protected void SendToAll<T>(T message, Delay? callbackIn = null) where T : Message =>
		Send(to: Message.All, message, callbackIn);

	protected void SendToAllOthers<T>(T message, Delay? callbackIn = null) where T : Message =>
		Send(to: Message.AllOthers, message, callbackIn);

	protected void SendToSelf<T>(T message) where T : Message =>
		Send(to: Id, message);
}
