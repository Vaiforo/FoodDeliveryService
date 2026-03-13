const statusEl = document.getElementById("status");
const msgEl = document.getElementById("msg");
const btn = document.getElementById("btn");

const ws = new WebSocket("ws://food-ws.local/ws");

ws.addEventListener("open", () => {
    statusEl.textContent = "WebSocket: соединение успешно установлено";
    btn.disabled = false;
});

ws.addEventListener("message", (event) => {
    msgEl.textContent = "";
    msgEl.textContent = event.data;
});

ws.addEventListener("close", () => {
    statusEl.textContent = "WebSocketL соедиенение потеряно";
    btn.disabled = true;
});

ws.addEventListener("error", () => {
    statusEl.textContent = "WebSocket: ошибка соединения";
    btn.disabled = true;
});

btn.addEventListener("click", () => {
    const unique = "support:" + Math.random().toString(36).substring(2);
    ws.send(unique);
});