CREATE TABLE IF NOT EXISTS bot_processes (
    id BIGSERIAL PRIMARY KEY,
    chat_id BIGINT NOT NULL,
    status TEXT NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS bot_action_items (
    id BIGSERIAL PRIMARY KEY,
    process_id BIGINT NOT NULL,
    action_type TEXT NOT NULL,
    payload_json TEXT NOT NULL DEFAULT '',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_bot_action_items_process
        FOREIGN KEY (process_id) REFERENCES bot_processes(id) ON DELETE CASCADE
);