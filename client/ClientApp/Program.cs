var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/", () =>
{
    var htmlResponse = """
        <!doctype html>
        <html lang="ru">
        <head>
        <meta charset="utf-8">
        <title>Client</title>
        </head>
        <body>
        <h1>Food Delivery Service</h1>
        <h2 id="status"></h2>
        <p id="msg"></p>
        <button id="btn" disabled>Хороший сервис</button>

        <script src="/app.js"></script>
        </body>
        </html>
    """;

    return Results.Text(htmlResponse, "text/html; charset=utf-8");
});

app.Run();