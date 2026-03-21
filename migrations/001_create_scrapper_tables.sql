CREATE TABLE IF NOT EXISTS scrapper_chats (
    id BIGSERIAL PRIMARY KEY,
    chat_id BIGINT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS scrapper_links (
    id BIGSERIAL PRIMARY KEY,
    url TEXT NOT NULL UNIQUE,
    last_checked TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS scrapper_subscriptions (
    id BIGSERIAL PRIMARY KEY,
    link_id BIGINT NOT NULL,
    chat_id BIGINT NOT NULL,
    CONSTRAINT fk_scrapper_subscriptions_link
        FOREIGN KEY (link_id) REFERENCES scrapper_links(id) ON DELETE CASCADE,
    CONSTRAINT fk_scrapper_subscriptions_chat
        FOREIGN KEY (chat_id) REFERENCES scrapper_chats(id) ON DELETE CASCADE,
    CONSTRAINT uq_scrapper_subscriptions_chat_link UNIQUE (chat_id, link_id)
);

CREATE TABLE IF NOT EXISTS scrapper_tags (
    id BIGSERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    subscription_id BIGINT NOT NULL,
    CONSTRAINT fk_scrapper_tags_subscription
        FOREIGN KEY (subscription_id) REFERENCES scrapper_subscriptions(id) ON DELETE CASCADE,
    CONSTRAINT uq_scrapper_tags_subscription_name UNIQUE (subscription_id, name)
);