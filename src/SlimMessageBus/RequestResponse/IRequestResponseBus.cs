﻿namespace SlimMessageBus;

/// <summary>
/// Bus to work with request-response messages.
/// </summary>
public interface IRequestResponseBus
{
    /// <summary>
    /// Sends a request message.
    /// </summary>
    /// <typeparam name="TResponseMessage">The response message type</typeparam>
    /// <param name="request">The request message</param>
    /// <param name="path">Name of the topic (or queue) to send the request to. When null the default topic (or queue) for request message type (or global default) will be used.</param>
    /// <param name="headers">The headers to add to the message. When null no additional headers will be added.</param>
    /// <param name="cancellationToken">Cancellation token to notify if the client no longer is interested in the response.</param>
    /// <param name="timeout">The timespan after which the Send request will be cancelled if no response arrives.</param>
    /// <returns>Task that represents the pending request.</returns>
    /// <exception cref="SendMessageBusException">When sending of the message failed</exception>
    /// <exception cref="ProducerMessageBusException">When sending of the message failed</exception>
    /// <exception cref="OperationCanceledException">When the request timeout or the request was cancelled (via CancellationToken)</exception>
    Task<TResponseMessage> Send<TResponseMessage, TRequestMessage>(TRequestMessage request, string path = null, IDictionary<string, object> headers = null, CancellationToken cancellationToken = default, TimeSpan? timeout = null);

    /// <summary>
    /// Sends a request message
    /// </summary>
    /// <typeparam name="TResponseMessage">The response message type</typeparam>
    /// <param name="request">The request message</param>
    /// <param name="path">Name of the topic (or queue) to send the request to. When null the default topic for request message type (or global default) will be used.</param>
    /// <param name="headers">The headers to add to the message. When null no additional headers will be added.</param>
    /// <param name="cancellationToken">Cancellation token to notify if the client no longer is interested in the response.</param>
    /// <param name="timeout">The timespan after which the Send request will be cancelled if no response arrives.</param>
    /// <returns>Task that represents the pending request.</returns>
    /// <exception cref="SendMessageBusException">When sending of the message failed</exception>
    /// <exception cref="ProducerMessageBusException">When sending of the message failed</exception>
    /// <exception cref="OperationCanceledException">When the request timeout or the request was cancelled (via CancellationToken)</exception>
    Task<TResponseMessage> Send<TResponseMessage>(IRequestMessage<TResponseMessage> request, string path = null, IDictionary<string, object> headers = null, CancellationToken cancellationToken = default, TimeSpan? timeout = null);
}