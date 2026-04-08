CREATE INDEX IF NOT EXISTS ix_scrapper_update_event_link_id
    ON scrapper_update_event(link_id);

CREATE INDEX IF NOT EXISTS ix_scrapper_update_event_status
    ON scrapper_update_event(status);
    
CREATE INDEX IF NOT EXISTS ix_scrapper_update_event_status_detected_at
    ON scrapper_update_event(status, detected_at);