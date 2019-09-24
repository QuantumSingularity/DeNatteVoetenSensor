-- Table: public."Users"

-- DROP TABLE public."Users";

CREATE TABLE public."Users"
(
  "UserId" integer NOT NULL DEFAULT nextval('"Users_UserId_seq"'::regclass),
  "UniqueIdentifier" text,
  "Name" text,
  "Password" text,
  "EmailAddress" text,
  "LastLogOnDate" timestamp without time zone,
  "CreatedDate" timestamp without time zone NOT NULL,
  "LastModifiedDate" timestamp without time zone NOT NULL,
  "PasswordChangedDate" timestamp without time zone NOT NULL,
  "IsDisabled" boolean NOT NULL,
  "DisabledDate" timestamp without time zone,
  CONSTRAINT "PK_Users" PRIMARY KEY ("UserId")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."Users"
  OWNER TO nvs;


-- Table: public."UserLogItems"

-- DROP TABLE public."UserLogItems";

CREATE TABLE public."UserLogItems"
(
  "UserLogItemId" integer NOT NULL DEFAULT nextval('"UserLogItems_UserLogItemId_seq"'::regclass),
  "UserId" integer NOT NULL,
  "TimeStamp" timestamp without time zone NOT NULL,
  "LogItemType" text,
  "Value" text,
  CONSTRAINT "PK_UserLogItems" PRIMARY KEY ("UserLogItemId")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."UserLogItems"
  OWNER TO nvs;

-- Index: public."IX_UserLogItems_UserId"

-- DROP INDEX public."IX_UserLogItems_UserId";

CREATE INDEX "IX_UserLogItems_UserId"
  ON public."UserLogItems"
  USING btree
  ("UserId");



-- Table: public."Sensors"

-- DROP TABLE public."Sensors";

CREATE TABLE public."Sensors"
(
  "SensorId" integer NOT NULL DEFAULT nextval('"Sensors_SensorId_seq"'::regclass),
  "MacAddress" text NOT NULL,
  "Name" text,
  "Location" text,
  "RegisterDate" timestamp without time zone NOT NULL,
  "ReRegisterDate" timestamp without time zone,
  "UserId" integer,
  "HeartBeatDate" timestamp without time zone,
  "LastDetectionOnDate" timestamp without time zone,
  "LastDetectionOffDate" timestamp without time zone,
  "DetectionStatus" text,
  "DetectionStatusDate" timestamp without time zone,
  "BatteryVoltageDate" timestamp without time zone,
  "BatteryVoltage" double precision,
  CONSTRAINT "PK_Sensors" PRIMARY KEY ("SensorId"),
  CONSTRAINT "AlternateKey_MacAddress" UNIQUE ("MacAddress")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."Sensors"
  OWNER TO nvs;

-- Index: public."IX_Sensors_UserId"

-- DROP INDEX public."IX_Sensors_UserId";

CREATE INDEX "IX_Sensors_UserId"
  ON public."Sensors"
  USING btree
  ("UserId");




-- Table: public."SensorLogItems"

-- DROP TABLE public."SensorLogItems";

CREATE TABLE public."SensorLogItems"
(
  "SensorLogItemId" integer NOT NULL DEFAULT nextval('"SensorLogItems_SensorLogItemId_seq"'::regclass),
  "SensorId" integer NOT NULL,
  "LogType" integer NOT NULL,
  "Value" text,
  "TimeStamp" timestamp without time zone NOT NULL,
  "BatteryVoltage" double precision,
  CONSTRAINT "PK_SensorLogItems" PRIMARY KEY ("SensorLogItemId")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."SensorLogItems"
  OWNER TO nvs;

-- Index: public."IX_SensorLogItems_SensorId"

-- DROP INDEX public."IX_SensorLogItems_SensorId";

CREATE INDEX "IX_SensorLogItems_SensorId"
  ON public."SensorLogItems"
  USING btree
  ("SensorId");



/*
  new fields 2019-09-24

alter table public."Sensors"
  add column "LocationLatitude" double precision,
  add column "LocationLongitude" double precision
;

*/

