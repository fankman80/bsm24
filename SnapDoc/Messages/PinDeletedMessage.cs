using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SnapDoc.Messages;

public class PinDeletedMessage(string pinId) : ValueChangedMessage<string>(pinId)
{
}
