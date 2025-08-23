# Dev run
Set DOTNET_ENVIRONMENT=Development

1) Запусти RMAgent (слушает 127.0.0.1:8443 tcp/udp)
2) Запусти RemoteManager (WPF). Сервер и discovery отключены, только gRPC-клиент.
3) В UI нажми Connect → используй endpoint из Client.DefaultAgentEndpoint (по умолчанию https://127.0.0.1:8443).
