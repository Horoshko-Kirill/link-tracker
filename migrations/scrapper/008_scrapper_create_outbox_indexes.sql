CREATE INDEX IF NOT EXISTS ix_outbox_messages_status
    ON scrapper_outbox_messages(status);