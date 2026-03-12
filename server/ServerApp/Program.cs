using Microsoft.AspNetCore.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", (HttpRequest request) =>
{
    var accept = request.Headers.Accept.ToString();

    if (string.IsNullOrWhiteSpace(accept) || !accept.Contains("text/html", StringComparison.OrdinalIgnoreCase))
        return Results.Text("Not Acceptable (нужен Accept: text/html)", "text/plain; charset=utf-8", statusCode: 406);

    var htmlResponse = """
        <!doctype html>
        <html lang="ru">
        <head><meta charset="utf-8"><title>Server</title></head>
        <body>
        <h1>Food Delivery Service</h1>
        <a href="http://food-client.local/">Перейти на сервер-клиент</a>
        </body>
        </html>
    """;

    return Results.Text(htmlResponse, "text/html; charset=utf-8");
});

app.Run();