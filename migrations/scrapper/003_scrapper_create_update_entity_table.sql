CREATE TABLE IF NOT EXISTS scrapper_update_event (
    id BIGSERIAL PRIMARY KEY,
    link_id BIGINT NOT NULL,
    event_type TEXT NOT NULL,
    source TEXT NOT NULL,
    title TEXT NOT NULL,
    author TEXT NOT NULL,
    preview TEXT NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    detected_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_scrapper_update_event_link
        FOREIGN KEY (link_id) REFERENCES scrapper_links(id) ON DELETE CASCADE,
);