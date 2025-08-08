using CommunityToolkit.Mvvm.Messaging.Messages;

namespace bsm24.Messages;

public class PinChangedMessage(string pinId) : ValueChangedMessage<string>(pinId)
{
}
