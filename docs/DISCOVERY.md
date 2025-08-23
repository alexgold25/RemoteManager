# Discovery

Agents and dashboards discover each other on UDP port 8443 using a magic prefix `RM01` followed by a protobuf encoded `Discovery` message and an HMAC-SHA256 signature.

The dashboard sends `DISCOVER` packets to the broadcast address and agents reply with `OFFER`. When an agent starts it also sends a `HELLO` announcement.
