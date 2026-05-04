CREATE TABLE IF NOT EXISTS scrapper_outbox_messages (
    id BIGSERIAL PRIMARY KEY,
    paylod TEXT NOT NULL,
    status INT NOT NULL DEFAULT 0,
    attempts INT NOT NULL DEFAULT 0,
    error TEXT NULL,
    created_at TIMESTAMPTZ NULL,
    sent_at TIMESTAMPTZ NULL
);