namespace DistributedPlayground.Algorithms.Tests.Infrastructure;

using DistributedPlayground.Infrastructure;

class Runner
{
	private readonly RecordingHandler _output;
	private readonly LogicalProcess _sut;

	public Runner(LogicalProcess sut)
	{
		_output = new();
		_sut = sut;
		_sut.Output = _output;
	}

	public void Run(Func<TestDefinition, TestDefinition> configureDefinition)
	{
		var def = configureDefinition(new TestDefinition(_sut.Id));

		// given
		foreach (var m in def.Givens)
			_sut.Handle(m);

		// when
		if (def.Whens.Count > 0)
			_output.Clear();

		foreach (var m in def.Whens)
			_sut.Handle(m);

		// then
		Assert.Collection(
			_output.HandledMessages,
			def.Thens.ToArray());
	}
}

