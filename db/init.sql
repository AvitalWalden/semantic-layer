-- ==========================================================================
--  Source database for the Semantic Layer demo (business domain: HR).
--
--  This script represents "the organization's existing relational database".
--  The Semantic Layer application connects to it, reads its structure
--  dynamically (via information_schema) and manages business metadata on top.
--
--  Column/table names here are intentionally technical (snake_case,
--  abbreviations, sensitive columns) so the semantic layer has real value
--  to add: friendly names, descriptions, hiding sensitive data, etc.
-- ==========================================================================

CREATE SCHEMA IF NOT EXISTS hr;

-- --------------------------------------------------------------------------
-- Tables
-- --------------------------------------------------------------------------
CREATE TABLE hr.departments (
    id          SERIAL PRIMARY KEY,
    name        VARCHAR(100) NOT NULL,
    location    VARCHAR(100),
    budget      NUMERIC(14, 2)
);

CREATE TABLE hr.job_titles (
    id          SERIAL PRIMARY KEY,
    title       VARCHAR(100) NOT NULL,
    level       INTEGER NOT NULL
);

CREATE TABLE hr.employees (
    id             SERIAL PRIMARY KEY,
    first_name     VARCHAR(60) NOT NULL,
    last_name      VARCHAR(60) NOT NULL,
    email          VARCHAR(150) NOT NULL,
    ssn            VARCHAR(20),                 -- sensitive: national ID
    phone          VARCHAR(30),
    department_id  INTEGER REFERENCES hr.departments(id),
    job_title_id   INTEGER REFERENCES hr.job_titles(id),
    manager_id     INTEGER REFERENCES hr.employees(id),
    hire_date      DATE NOT NULL,
    is_active      BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE hr.salaries (
    id             SERIAL PRIMARY KEY,
    employee_id    INTEGER NOT NULL REFERENCES hr.employees(id),
    amount         NUMERIC(12, 2) NOT NULL,     -- sensitive: monthly gross
    currency       VARCHAR(3) NOT NULL DEFAULT 'USD',
    effective_date DATE NOT NULL
);

-- --------------------------------------------------------------------------
-- Seed data
-- --------------------------------------------------------------------------
INSERT INTO hr.departments (name, location, budget) VALUES
    ('Engineering', 'Tel Aviv',  2500000.00),
    ('Human Resources', 'Tel Aviv', 600000.00),
    ('Sales', 'New York', 1800000.00),
    ('Finance', 'London', 900000.00),
    ('Product', 'Tel Aviv', 1200000.00);

INSERT INTO hr.job_titles (title, level) VALUES
    ('Software Engineer', 2),
    ('Senior Software Engineer', 3),
    ('Engineering Manager', 4),
    ('HR Specialist', 2),
    ('Sales Representative', 2),
    ('Sales Manager', 4),
    ('Financial Analyst', 2),
    ('Product Manager', 3);

-- Managers first (so manager_id can reference them).
INSERT INTO hr.employees (first_name, last_name, email, ssn, phone, department_id, job_title_id, manager_id, hire_date, is_active) VALUES
    ('Dana',   'Cohen',   'dana.cohen@example.com',   '111-11-1111', '+972-50-1112223', 1, 3, NULL, '2018-03-01', TRUE),
    ('Amir',   'Levi',    'amir.levi@example.com',    '222-22-2222', '+972-50-2223334', 3, 6, NULL, '2017-06-15', TRUE),
    ('Noa',    'Mizrahi', 'noa.mizrahi@example.com',  '333-33-3333', '+972-50-3334445', 2, 4, NULL, '2019-01-20', TRUE);

INSERT INTO hr.employees (first_name, last_name, email, ssn, phone, department_id, job_title_id, manager_id, hire_date, is_active) VALUES
    ('Yossi',  'Bar',     'yossi.bar@example.com',    '444-44-4444', '+972-52-4445556', 1, 2, 1, '2020-09-10', TRUE),
    ('Maya',   'Katz',    'maya.katz@example.com',    '555-55-5555', '+972-52-5556667', 1, 1, 1, '2021-11-05', TRUE),
    ('Tom',    'Green',   'tom.green@example.com',    '666-66-6666', '+1-212-555-0101',  3, 5, 2, '2022-02-14', TRUE),
    ('Rachel', 'Adler',   'rachel.adler@example.com', '777-77-7777', '+1-212-555-0102',  3, 5, 2, '2020-07-22', TRUE),
    ('Eli',    'Shapiro', 'eli.shapiro@example.com',  '888-88-8888', '+44-20-7946-0011', 4, 7, NULL, '2019-05-30', TRUE),
    ('Lior',   'Peretz',  'lior.peretz@example.com',  '999-99-9999', '+972-54-9990001',  5, 8, NULL, '2021-03-18', TRUE),
    ('Gili',   'Ronen',   'gili.ronen@example.com',   '123-45-6789', '+972-54-1230004',  1, 1, 1, '2023-08-01', FALSE);

-- Current salaries (one active row per employee for this demo).
INSERT INTO hr.salaries (employee_id, amount, currency, effective_date) VALUES
    (1, 32000.00, 'USD', '2023-01-01'),
    (2, 28000.00, 'USD', '2023-01-01'),
    (3, 22000.00, 'USD', '2023-01-01'),
    (4, 24000.00, 'USD', '2023-01-01'),
    (5, 18000.00, 'USD', '2023-01-01'),
    (6, 16000.00, 'USD', '2023-01-01'),
    (7, 17000.00, 'USD', '2023-01-01'),
    (8, 20000.00, 'USD', '2023-01-01'),
    (9, 21000.00, 'USD', '2023-01-01'),
    (10, 15000.00, 'USD', '2023-01-01');
