# Security

Development certificates are loaded from `certificates/dev` with password `dev`. TLS 1.3 is used for all RPC communication.

Discovery packets are signed with HMAC-SHA256 using a pre-shared key.
