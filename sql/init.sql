CREATE TABLE project (
  id serial primary key,
  name varchar(255) not null default '',
  phase integer not null default 0
);

INSERT INTO project (
  id,
  name,
  phase
) VALUES (
  default,
  '2024 project',
  0
);

INSERT INTO project (
  id,
  name,
  phase
) VALUES (
  default,
  'My loaded',
  3
);

INSERT INTO project (
  id,
  name,
  phase
) VALUES (
  default,
  'Grand New',
  1
);