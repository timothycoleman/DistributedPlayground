# Distributed Playground

This is a harness for deterministically simulating the execution of distributed algorithms.

See `DistributedPlayground.Algorithms/PingPongSimple` for a simple example algorithm
and `DistributedPlayground.Algorithms.Tests/PingPongSimple` for some simple tests.

The tests can instantiate processes, connect them together via a network and inspect the result.

The network can simulate
- the passage of time
- network latency
- network partitions
- network regions (coming soon)
- lost messages
- duplicated messages
- out of order messages

This is achieved via a configurable pipeline of interceptors for the scheduling and then delivery
of messages sent in the network. See `Interceptors` static class.

Processes derive from LogicalProcess which provides them with some helpers for easily sending messages
to themselves and others. When handling messages they are dispatched internally using `dynamic` for
brevity but I may reconsider this.

## Todo

- Other algorithms: Paxos State Machine Replication, VR, Raft, MESI, ...
- some way to avoid dynamic dispatch without having to write a lot of subscribing code
- perhaps a random delay that doesn't reorder messages between two processes
- some process to randomly partition the network
