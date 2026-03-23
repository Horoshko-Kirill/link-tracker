CREATE INDEX IF NOT EXISTS ix_bot_processes_chat_id
    ON bot_processes(chat_id);

CREATE INDEX IF NOT EXISTS ix_bot_action_items_process_id
    ON bot_action_items(process_id);

CREATE INDEX IF NOT EXISTS ix_bot_processes_status
    ON bot_processes(status);