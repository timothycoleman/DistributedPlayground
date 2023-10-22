namespace DistributedPlayground.Infrastructure;

public delegate void Send(Delay delay, Message message);
public delegate void InterceptSending(Delay delay, Message message, Send next);
public delegate void Deliver(Message message);
public delegate void InterceptDelivery(Message message, Deliver next);
