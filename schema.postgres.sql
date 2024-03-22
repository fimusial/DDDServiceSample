CREATE TABLE Memo(
    id serial PRIMARY KEY,
    content VARCHAR);

CREATE TABLE IntegrationEventOutbox(
    id serial PRIMARY KEY,
    content VARCHAR,
    pushedAt TIMESTAMP);

CREATE INDEX IntegrationEventOutbox_Index_PushedAt
ON IntegrationEventOutbox(pushedAt ASC);

CREATE OR REPLACE FUNCTION setPushedAt()
RETURNS TRIGGER AS
$$
BEGIN
    NEW.pushedAt := CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER IntegrationEventOutbox_OnInsert_SetPushedAt
BEFORE INSERT ON IntegrationEventOutbox
FOR EACH ROW EXECUTE FUNCTION setPushedAt();

ALTER SYSTEM SET default_transaction_read_only TO ON;
SELECT pg_reload_conf();