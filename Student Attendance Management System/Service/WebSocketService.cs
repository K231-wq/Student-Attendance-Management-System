using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace Student_Attendance_Management_System.Service
{
    public class WebSocketService
    {
        private ClientWebSocket _clientWebSocket;
        private readonly Uri _serverUri = new Uri("ws://localhost:3000");

        public event Action OnConnected;
        public event Action OnDisconnected;

        public event Action<string> OnMessageReceived;

        public async Task ConnectAsync()
        {
            if (_clientWebSocket?.State == WebSocketState.Open) return;
            while (true) // Infinite loop for auto-reconnect
            {
                _clientWebSocket = new ClientWebSocket();

                try
                {
                    Debug.WriteLine("Attempting to connect to WebSocket...");
                    await _clientWebSocket.ConnectAsync(_serverUri, CancellationToken.None);
                    OnConnected?.Invoke();

                    // Start listening for messages
                    await ReceiveMessagesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Connection Error: {ex.Message}");
                    OnDisconnected?.Invoke();
                }
                await Task.Delay(5000);
            }
        }
        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];
            while (_clientWebSocket.State == WebSocketState.Open)
            {
                var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    OnMessageReceived?.Invoke(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
            }
        }
        public async Task SendMessageAsync(string message)
        {
            try
            {
                if (_clientWebSocket.State == WebSocketState.Open)
                {
                    var bytes = Encoding.UTF8.GetBytes(message);
                    await _clientWebSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WS Parse Error: " + ex.Message);
            }

        }
    }
}
