using System;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace JustBehave.Core;

public static class MessageBusExtensions
{
    public static void Queue(this IMessageBus messageBus, ITest test, Func<ITest, IMessageSinkMessage> createTestResultMessage, CancellationTokenSource? cancellationTokenSource)
    {
        if (messageBus == null)
        {
            throw new ArgumentNullException(nameof(messageBus));
        }

        if (messageBus.QueueMessage(new TestStarting(test)) == false)
        {
            cancellationTokenSource?.Cancel();
        }
        else
        {
            if (messageBus.QueueMessage(createTestResultMessage(test)) == false)
            {
                cancellationTokenSource?.Cancel();
            }
        }

        if (messageBus.QueueMessage(new TestFinished(test, 0, null)))
        {
            cancellationTokenSource?.Cancel();
        }
    }
}