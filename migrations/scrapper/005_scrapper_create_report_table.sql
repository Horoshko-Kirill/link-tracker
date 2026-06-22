CREATE TABLE IF NOT EXISTS scrapper_link_scan_report (
    id BIGSERIAL PRIMARY KEY,
    chat_id BIGINT NOT NULL,
    scan_started_at TIMESTAMPTZ NOT NULL,
    scan_finished_at TIMESTAMPTZ NOT NULL,
    failed_count INT NOT NULL DEFAULT 0,
    message TEXT NOT NULL,
    status INT NOT NULL DEFAULT 0,
    sent_at TIMESTAMPTZ NULL,
    CONSTRAINT fk_scrapper_link_scan_report_chats
        FOREIGN KEY (chat_id) REFERENCES scrapper_chats(id) ON DELETE CASCADE
);