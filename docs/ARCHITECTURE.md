# RemoteManager Architecture

This solution contains a WPF dashboard and an agent service communicating over gRPC on port 8443.

- **RemoteManager** – WPF client with MVVM structure.
- **RMAgent** – Worker service exposing gRPC endpoints and discovery.
- **RM.Shared** – shared helpers for TLS, discovery and utilities.
- **RM.Proto** – Protobuf contracts shared by client and agent.

All traffic uses TLS 1.3 and protobuf serialization.
