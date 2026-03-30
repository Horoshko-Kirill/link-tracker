CREATE UNIQUE INDEX ux_scrapper_subscriptions_chat_link
    ON scrapper_subscriptions(chat_id, link_id);

CREATE INDEX IF NOT EXISTS ix_scrapper_subscriptions_chat_id
    ON scrapper_subscriptions(chat_id);

CREATE INDEX IF NOT EXISTS ix_scrapper_subscriptions_link_id
    ON scrapper_subscriptions(link_id);

CREATE INDEX IF NOT EXISTS ix_scrapper_tags_subscription_id
    ON scrapper_tags(subscription_id);

CREATE INDEX IF NOT EXISTS ix_scrapper_links_last_checked
    ON scrapper_links(last_checked);

CREATE INDEX IF NOT EXISTS ix_scrapper_links_url
    ON scrapper_links(url);