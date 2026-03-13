using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();

const string best = "Отличный сервис по доставке еды. Или...";
const string change = "Может выбрать другой сервис...";
const string notchange = "Да? Окей. Спасибо за выбор!";

static async Task SendTextAsync(WebSocket ws, string text, CancellationToken ct)
{
    var bytes = Encoding.UTF8.GetBytes(text);
    await ws.SendAsync(bytes, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: ct);
}

static async Task<string?> ReceiveTextAsync(WebSocket ws, CancellationToken ct)
{
    var buffer = new byte[4096];
    using var ms = new MemoryStream();

    while (true)
    {
        var result = await ws.ReceiveAsync(buffer, ct);

        if (result.MessageType == WebSocketMessageType.Close)
            return null;

        ms.Write(buffer, 0, result.Count);

        if (result.EndOfMessage)
            break;
    }

    return Encoding.UTF8.GetString(ms.ToArray());
}

app.Map("/ws", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Expected WebSocket request");
        return;
    }

    using var ws = await context.WebSockets.AcceptWebSocketAsync();
    var ct = context.RequestAborted;

    var seen = new HashSet<string>();
    var rnd = new Random();
    var sendLock = new SemaphoreSlim(1, 1);

    var sendLoop = Task.Run(async () =>
    {
        while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
        {
            var delayMs = rnd.Next(2000, 5001);
            await Task.Delay(delayMs, ct);

            var msg = rnd.NextDouble() < 0.65 ? best : change;
            await SendTextAsync(ws, msg, ct);
        }
    }, ct);

    try
    {
        while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
        {
            var message = await ReceiveTextAsync(ws, ct);
            if (message is null) break;

            if (seen.Add(message))
                await SendTextAsync(ws, notchange, ct);
        }
    }
    catch (OperationCanceledException) {}
    catch (WebSocketException) {}
    finally
    {
        if (ws.State == WebSocketState.Open)
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);

        try
        {
            await sendLoop;
        }
        catch { }
    }
});

app.Run();