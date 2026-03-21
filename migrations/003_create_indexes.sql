CREATE INDEX IF NOT EXISTS ix_scrapper_subscriptions_chat_id
    ON scrapper_subscriptions(chat_id);

CREATE INDEX IF NOT EXISTS ix_scrapper_subscriptions_link_id
    ON scrapper_subscriptions(link_id);

CREATE INDEX IF NOT EXISTS ix_scrapper_tags_subscription_id
    ON scrapper_tags(subscription_id);

CREATE INDEX IF NOT EXISTS ix_bot_processes_chat_id
    ON bot_processes(chat_id);

CREATE INDEX IF NOT EXISTS ix_bot_action_items_process_id
    ON bot_action_items(process_id);