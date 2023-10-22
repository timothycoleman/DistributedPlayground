namespace DistributedPlayground.Algorithms.Tests.Infrastructure;

using DistributedPlayground.Infrastructure;
using DistributedPlayground.Infrastructure.Messages;

class TestDefinition
{
	private readonly string _sutId;

	public TestDefinition(string sutId)
	{
		_sutId = sutId;
	}

	public List<Message> Givens { get; private set; } = new();
	public List<Message> Whens { get; private set; } = new();
	public List<Action<Message>> Thens { get; private set; } = new();

	public TestDefinition Given(params Message[] givens)
	{
		Givens.AddRange(givens.Select(x => x with { To = _sutId }));
		return this;
	}

	// typically (always?) only have one 'when' per test, but we do support multiple.
	public TestDefinition When(params Message[] whens)
	{
		Whens.AddRange(whens.Select(x => x with { To = _sutId }));
		return this;
	}

	public TestDefinition WhenFrom(string from, Message when) =>
		When(when with { From = from });

	public TestDefinition WhenCallback<T>(string to, T message) where T : Message =>
		When(
			new Callback<T>(
				Delay.None,
				message with
				{
					From = _sutId,
					To = to,
				})
			{
				From = _sutId,
				To = _sutId,
			});

	// expect messages equal to these
	public TestDefinition Then(params Message[] expecteds)
	{
		Thens.AddRange(expecteds.Select<Message, Action<Message>>(
			expected => actual =>
				Assert.Equal(
					expected with { From = _sutId },
					actual)));
		return this;
	}

	public TestDefinition ThenTo(string to, params Message[] expecteds) =>
		Then(expecteds.Select(x => x with { To = to }).ToArray());

	public TestDefinition Then<T>(string to, T message, Delay? withCallbackAfter) where T : Message
	{
		message = message with
		{
			From = _sutId,
			To = to,
		};

		Then(message);

		if (withCallbackAfter != null)
		{
			Then(new Callback<T>(withCallbackAfter.Value, message)
			{
				From = message.From,
				To = message.From,
			});
		}

		return this;
	}
}
