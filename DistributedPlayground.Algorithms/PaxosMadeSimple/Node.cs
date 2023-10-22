using DistributedPlayground.Infrastructure;
using DistributedPlayground.Infrastructure.Messages;

namespace DistributedPlayground.PaxosMadeSimple;

// Paxos election Based on the paper Paxos Made Simple - Lamport 2001
//
// todo: consider how to model a machine restarting.. maybe components should handle
// a restart message and there should be a component that sends them
// todo: failover and reelection
// todo: state machine replication
public record Proposal(int Number, string Value);

public static class Messages
{
	// Attempt to get the information required before making a proposal
	public record Prepare(
		int ProposalNumber)
		: Message;

	// This is a promise to not accept proposals less than the ProposalNumber
	// It includes the maximum accepted proposal to constrain the proposer when it proposes a value
	public record Prepared(
		int ProposalNumber,
		Proposal? MaxAcceptedProposal)
		: Message;

	// Propose to the acceptors to accept
	public record Accept(
		Proposal Proposal)
		: Message;

	public record Accepted(
		Proposal Proposal)
		: Message;
}

public class Node : LogicalProcess
{
	private readonly int _quorumSize;
	private readonly int _clusterSize;

	public Node(
		string id,
		int nodeIndex,
		int clusterSize)
		: base(id)
	{
		_currentProposal = nodeIndex;
		_clusterSize = clusterSize;
		_quorumSize = (clusterSize + 1) / 2;
	}

	public override void Handle(Message msg) => HandleImpl((dynamic)msg);

	public override void Start()
	{
		BeginRound();
	}

	//
	// Proposer
	//

	private int _currentProposal;
	private readonly Dictionary<string, Messages.Prepared> _prepareResponses = new();

	private void BeginRound()
	{
		_currentProposal += _clusterSize;
		_prepareResponses.Clear();

		SendToAll(
			new Messages.Prepare(ProposalNumber: _currentProposal),
			callbackIn: new Delay(50));
	}

	private void HandleImpl(Callback<Messages.Prepare> msg)
	{
		if (msg.Message.ProposalNumber != _currentProposal)
			return;

		if (ChosenValue is not null)
			return;

		BeginRound();
	}

	private void HandleImpl(Messages.Prepared msg)
	{
		if (msg.ProposalNumber != _currentProposal)
		{
			// a response from some other proposal. ignore it.
			return;
		}

		// A response for our proposal!
		_prepareResponses[msg.From] = msg;

		if (_prepareResponses.Count < _quorumSize)
			return;

		// Received quorum responses for our proposal. Send an accept
		var maxProposal = _prepareResponses.Values
			.Select(x => x.MaxAcceptedProposal)
			.Where(x => x != null)
			.MaxBy(x => x!.Number);

		var value = maxProposal?.Value ?? Id;

		SendToAll(new Messages.Accept(
			Proposal: new(msg.ProposalNumber, value)));
	}

	//
	// Acceptor
	//

	Proposal? _maxProposalAccepted;
	int _maxPrepareRespondedTo = -1;

	private void HandleImpl(Messages.Prepare msg)
	{
		if (msg.ProposalNumber <= _maxPrepareRespondedTo)
		{
			// todo: consider replying with Messages.NotPrepared
			return;
		}

		// The prepare response is a promise not to accept any proposal numbered
		// less than the prepare we are responding to
		_maxPrepareRespondedTo = msg.ProposalNumber;
		Send(
			to: msg.From,
			new Messages.Prepared(
				ProposalNumber: msg.ProposalNumber,
				MaxAcceptedProposal: _maxProposalAccepted));
	}

	private void HandleImpl(Messages.Accept msg)
	{
		if (msg.Proposal.Number < _maxPrepareRespondedTo)
		{
			// todo: consider sending a Messages.NotAccepted
			return;
		}

		if (_maxProposalAccepted is null ||
			msg.Proposal.Number > _maxProposalAccepted.Number)
		{
			_maxProposalAccepted = msg.Proposal;
		}

		SendToAll(new Messages.Accepted(msg.Proposal));
	}

	//
	// Learner
	//

	private readonly Dictionary<Proposal, HashSet<string>> _acceptances = new();
	public string? ChosenValue { get; private set; }

	private void HandleImpl(Messages.Accepted msg)
	{
		// a proposal is considered _chosen_ when it has been accepted by a majority of the acceptors
		if (!_acceptances.TryGetValue(msg.Proposal, out var acceptsForProposal))
		{
			acceptsForProposal = new HashSet<string>();
			_acceptances[msg.Proposal] = acceptsForProposal;
		}

		acceptsForProposal.Add(msg.From);

		if (acceptsForProposal.Count < _quorumSize)
			return;

		ChosenValue = msg.Proposal.Value;
	}
}
