#!/usr/bin/env bash
set -e

# nginx
sudo service nginx start || true
sudo nginx -t
sudo service nginx reload

# apps
dotnet run --project ws-service/WsService --urls http://127.0.0.1:5002 &
dotnet run --project server/ServerApp --urls http://127.0.0.1:5000 &
dotnet run --project client/ClientApp --urls http://127.0.0.1:5001 &

echo "Open: http://food.local/"
wait