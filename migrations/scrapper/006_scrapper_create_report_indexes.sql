CREATE INDEX IF NOT EXISTS ix_scrapper_chat_link_scan_report_chat_id
    ON scrapper_link_scan_report(chat_id);

CREATE INDEX IF NOT EXISTS ix_scrapper_chat_link_scan_report_status
    ON scrapper_link_scan_report(status);
    