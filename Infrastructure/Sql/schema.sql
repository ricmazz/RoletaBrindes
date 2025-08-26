CREATE TABLE IF NOT EXISTS gifts (
                                     id         SERIAL PRIMARY KEY,
                                     name       TEXT NOT NULL,
                                     stock      INT  NOT NULL CHECK (stock >= 0),
    weight     INT  NOT NULL CHECK (weight >= 0),
    is_active  BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
    );

CREATE OR REPLACE FUNCTION set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
RETURN NEW;
END; $$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_gifts_updated_at ON gifts;
CREATE TRIGGER trg_gifts_updated_at
    BEFORE UPDATE ON gifts
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

CREATE TABLE IF NOT EXISTS participants (
                                            id         SERIAL PRIMARY KEY,
                                            name       TEXT NOT NULL,
                                            phone      TEXT,
                                            created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE (phone)
    );

CREATE TABLE IF NOT EXISTS spins (
                                     id             SERIAL PRIMARY KEY,
                                     participant_id INT NOT NULL REFERENCES participants(id),
    gift_id        INT REFERENCES gifts(id),
    won            BOOLEAN NOT NULL DEFAULT FALSE,
    created_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
    );