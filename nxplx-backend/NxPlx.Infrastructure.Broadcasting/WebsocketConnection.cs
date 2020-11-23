using System;
using System.Buffers;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NxPlx.Application.Core;

namespace NxPlx.Infrastructure.Broadcasting
{
    public class WebsocketConnection : Connection
    {
        private readonly HttpContext _httpContext;
        private WebSocket? _websocket;

        public WebsocketConnection(HttpContext httpContext, IOperationContext operationContext) : base(operationContext)
        {
            _httpContext = httpContext;
        }

        public override Task SendMessage(Message message)
        {
            if (_websocket == null) throw new Exception("WebSocket has not been accepted yet");
            if (_websocket.State != WebSocketState.Open) throw new Exception("WebSocket connection is not open");
            return _websocket.SendAsync(JsonSerializer.SerializeToUtf8Bytes(message), WebSocketMessageType.Text, true, _httpContext.RequestAborted);
        }

        public override async Task KeepConnectionOpen()
        {
            if (_websocket != null) throw new Exception("WebSocket has already been accepted");

            try
            {
                _websocket = await _httpContext.WebSockets.AcceptWebSocketAsync();
            
                var buffer = ArrayPool<byte>.Shared.Rent(4096);
                while (_websocket.State == WebSocketState.Connecting || _websocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult message;
                    try
                    {
                        message = await _websocket.ReceiveAsync(new ArraySegment<byte>(buffer), _httpContext.RequestAborted);
                    }
                    catch (OperationCanceledException) { break; }
                    catch (WebSocketException) { break; }
                    catch (IOException) { break; }
                
                    if (message.MessageType == WebSocketMessageType.Close)
                        break;

                    if (message.EndOfMessage && message.MessageType == WebSocketMessageType.Text)
                    {
                        try
                        {
                            var parsed = JsonSerializer.Deserialize<Message>(buffer);
                            if (parsed != null)
                                OnMessageReceived(parsed);
                        }
                        catch (JsonException) { }
                    }
                }
            }
            finally
            {
                OnDisconnected();
            }
        }
    }
}