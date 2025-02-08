CREATE TABLE memo (
    id SERIAL PRIMARY KEY,
    content VARCHAR
);

CREATE TABLE integration_event_outbox (
    id SERIAL PRIMARY KEY,
    content VARCHAR,
    pushed_at TIMESTAMP
);

CREATE INDEX idx_integration_event_outbox_pushed_at
ON integration_event_outbox(pushed_at ASC);

CREATE OR REPLACE FUNCTION set_pushed_at()
RETURNS TRIGGER AS
$$
BEGIN
    NEW.pushed_at := CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_integration_event_outbox_set_pushed_at
BEFORE INSERT ON integration_event_outbox
FOR EACH ROW EXECUTE FUNCTION set_pushed_at();

ALTER SYSTEM SET default_transaction_read_only TO ON;
SELECT pg_reload_conf();