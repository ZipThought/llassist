CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "EstimateRelevanceJobs" (
    "Id" text NOT NULL,
    "ModelName" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "TotalArticles" integer NOT NULL,
    "ProjectId" text NOT NULL,
    CONSTRAINT "PK_EstimateRelevanceJobs" PRIMARY KEY ("Id")
);

CREATE TABLE "Projects" (
    "Id" text NOT NULL,
    "Name" text NOT NULL,
    "Description" text NOT NULL,
    CONSTRAINT "PK_Projects" PRIMARY KEY ("Id")
);

CREATE TABLE "Snapshots" (
    "Id" text NOT NULL,
    "EntityType" text NOT NULL,
    "EntityId" text NOT NULL,
    "SerializedEntity" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "EstimateRelevanceJobId" text NOT NULL,
    CONSTRAINT "PK_Snapshots" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Snapshots_EstimateRelevanceJobs_EstimateRelevanceJobId" FOREIGN KEY ("EstimateRelevanceJobId") REFERENCES "EstimateRelevanceJobs" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Articles" (
    "Id" text NOT NULL,
    "Authors" text NOT NULL,
    "Year" integer NOT NULL,
    "Title" text NOT NULL,
    "DOI" text NOT NULL,
    "Link" text NOT NULL,
    "Abstract" text NOT NULL,
    "ProjectId" text NOT NULL,
    CONSTRAINT "PK_Articles" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Articles_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProjectDefinitions" (
    "Id" text NOT NULL,
    "Definition" text NOT NULL,
    "ProjectId" text NOT NULL,
    CONSTRAINT "PK_ProjectDefinitions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProjectDefinitions_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ResearchQuestions" (
    "Id" text NOT NULL,
    "QuestionText" text NOT NULL,
    "ProjectId" text NOT NULL,
    CONSTRAINT "PK_ResearchQuestions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ResearchQuestions_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ArticleKeySemantics" (
    "Id" text NOT NULL,
    "KeySemantics" jsonb NOT NULL,
    CONSTRAINT "PK_ArticleKeySemantics" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ArticleKeySemantics_Articles_Id" FOREIGN KEY ("Id") REFERENCES "Articles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ArticleRelevances" (
    "Id" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "MustRead" boolean NOT NULL,
    "ArticleId" text NOT NULL,
    "EstimateRelevanceJobId" text NOT NULL,
    "Relevances" jsonb,
    CONSTRAINT "PK_ArticleRelevances" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ArticleRelevances_Articles_ArticleId" FOREIGN KEY ("ArticleId") REFERENCES "Articles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "QuestionDefinitions" (
    "Id" text NOT NULL,
    "Definition" text NOT NULL,
    "ResearchQuestionId" text NOT NULL,
    CONSTRAINT "PK_QuestionDefinitions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_QuestionDefinitions_ResearchQuestions_ResearchQuestionId" FOREIGN KEY ("ResearchQuestionId") REFERENCES "ResearchQuestions" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_ArticleRelevances_ArticleId" ON "ArticleRelevances" ("ArticleId");

CREATE UNIQUE INDEX "IX_ArticleRelevances_ArticleId_EstimateRelevanceJobId" ON "ArticleRelevances" ("ArticleId", "EstimateRelevanceJobId");

CREATE INDEX "IX_Articles_ProjectId" ON "Articles" ("ProjectId");

CREATE INDEX "IX_EstimateRelevanceJobs_ProjectId" ON "EstimateRelevanceJobs" ("ProjectId");

CREATE INDEX "IX_ProjectDefinitions_ProjectId" ON "ProjectDefinitions" ("ProjectId");

CREATE INDEX "IX_QuestionDefinitions_ResearchQuestionId" ON "QuestionDefinitions" ("ResearchQuestionId");

CREATE INDEX "IX_ResearchQuestions_ProjectId" ON "ResearchQuestions" ("ProjectId");

CREATE INDEX "IX_Snapshots_EstimateRelevanceJobId" ON "Snapshots" ("EstimateRelevanceJobId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241004060013_InitDatabase', '8.0.8');

COMMIT;

START TRANSACTION;

ALTER TABLE "EstimateRelevanceJobs" ADD "CompletedArticles" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241004093219_AddCompletedArticleCountToJob', '8.0.8');

COMMIT;

START TRANSACTION;

DROP INDEX "IX_ArticleRelevances_ArticleId";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241006032228_AllowMultipleRelevances', '8.0.8');

COMMIT;

START TRANSACTION;

ALTER TABLE "ArticleKeySemantics" DROP CONSTRAINT "FK_ArticleKeySemantics_Articles_Id";

ALTER TABLE "ArticleRelevances" DROP CONSTRAINT "PK_ArticleRelevances";

DROP INDEX "IX_ArticleRelevances_ArticleId_EstimateRelevanceJobId";

ALTER TABLE "ArticleRelevances" DROP COLUMN "Id";

ALTER TABLE "ArticleKeySemantics" RENAME COLUMN "Id" TO "ArticleId";

ALTER TABLE "ArticleRelevances" ADD CONSTRAINT "PK_ArticleRelevances" PRIMARY KEY ("ArticleId", "EstimateRelevanceJobId");

ALTER TABLE "ArticleKeySemantics" ADD CONSTRAINT "FK_ArticleKeySemantics_Articles_ArticleId" FOREIGN KEY ("ArticleId") REFERENCES "Articles" ("Id") ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241007043522_ConvertArticlesWeakEntity', '8.0.8');

COMMIT;

START TRANSACTION;

ALTER TABLE "ArticleRelevances" DROP CONSTRAINT "PK_ArticleRelevances";

ALTER TABLE "ArticleKeySemantics" DROP CONSTRAINT "PK_ArticleKeySemantics";

ALTER TABLE "ArticleRelevances" DROP COLUMN "Relevances";

ALTER TABLE "ArticleKeySemantics" DROP COLUMN "KeySemantics";

ALTER TABLE "ArticleRelevances" RENAME COLUMN "MustRead" TO "IsRelevant";

ALTER TABLE "Articles" ADD "MustRead" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "ArticleRelevances" ADD "RelevanceIndex" integer NOT NULL DEFAULT 0;

ALTER TABLE "ArticleRelevances" ADD "ContributionReason" text NOT NULL DEFAULT '';

ALTER TABLE "ArticleRelevances" ADD "ContributionScore" double precision NOT NULL DEFAULT 0.0;

ALTER TABLE "ArticleRelevances" ADD "IsContributing" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "ArticleRelevances" ADD "Question" text NOT NULL DEFAULT '';

ALTER TABLE "ArticleRelevances" ADD "RelevanceReason" text NOT NULL DEFAULT '';

ALTER TABLE "ArticleRelevances" ADD "RelevanceScore" double precision NOT NULL DEFAULT 0.0;

ALTER TABLE "ArticleKeySemantics" ADD "KeySemanticIndex" integer NOT NULL DEFAULT 0;

ALTER TABLE "ArticleKeySemantics" ADD "Type" text NOT NULL DEFAULT '';

ALTER TABLE "ArticleKeySemantics" ADD "Value" text NOT NULL DEFAULT '';

ALTER TABLE "ArticleRelevances" ADD CONSTRAINT "PK_ArticleRelevances" PRIMARY KEY ("ArticleId", "EstimateRelevanceJobId", "RelevanceIndex");

ALTER TABLE "ArticleKeySemantics" ADD CONSTRAINT "PK_ArticleKeySemantics" PRIMARY KEY ("ArticleId", "KeySemanticIndex");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241007062429_FixArticleKeySemanticsAndRelevances', '8.0.8');

COMMIT;

START TRANSACTION;

CREATE TABLE "AppSettings" (
    "Key" character varying(100) NOT NULL,
    "Value" text NOT NULL,
    "Description" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_AppSettings" PRIMARY KEY ("Key")
);

INSERT INTO "AppSettings" ("Key", "Value", "Description", "CreatedAt")
VALUES ('OpenAI:ApiKey', '', 'OpenAI API Key for LLM operations', TIMESTAMPTZ '2024-11-17T04:59:45.810456Z');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241031205714_AppSetting', '8.0.8');

COMMIT;

START TRANSACTION;

CREATE TABLE "Catalogs" (
    "Id" text NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" character varying(2000) NOT NULL,
    "Owner" character varying(100) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Catalogs" PRIMARY KEY ("Id")
);

CREATE TABLE "Categories" (
    "Id" text NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" character varying(2000) NOT NULL,
    "Path" character varying(1000) NOT NULL,
    "Depth" integer NOT NULL,
    "CatalogId" text NOT NULL,
    "ParentId" text,
    "SchemaType" character varying(100) NOT NULL,
    CONSTRAINT "PK_Categories" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Categories_Catalogs_CatalogId" FOREIGN KEY ("CatalogId") REFERENCES "Catalogs" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Categories_Categories_ParentId" FOREIGN KEY ("ParentId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Collections" (
    "Id" text NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" character varying(2000) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "CatalogId" text NOT NULL,
    CONSTRAINT "PK_Collections" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Collections_Catalogs_CatalogId" FOREIGN KEY ("CatalogId") REFERENCES "Catalogs" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Entries" (
    "Id" text NOT NULL,
    "EntryType" character varying(50) NOT NULL,
    "Title" character varying(500) NOT NULL,
    "Description" character varying(5000) NOT NULL,
    "Citation" character varying(2000) NOT NULL,
    "Source" character varying(200) NOT NULL,
    "Identifier" character varying(200) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "PublishedAt" timestamp with time zone,
    "CatalogId" text NOT NULL,
    "Metadata" jsonb NOT NULL,
    CONSTRAINT "PK_Entries" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Entries_Catalogs_CatalogId" FOREIGN KEY ("CatalogId") REFERENCES "Catalogs" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Labels" (
    "Id" text NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(500) NOT NULL,
    "Color" character varying(7) NOT NULL,
    "CatalogId" text NOT NULL,
    CONSTRAINT "PK_Labels" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Labels_Catalogs_CatalogId" FOREIGN KEY ("CatalogId") REFERENCES "Catalogs" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ArticleReferences" (
    "ArticleId" text NOT NULL,
    "EntryId" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ArticleReferences" PRIMARY KEY ("ArticleId", "EntryId"),
    CONSTRAINT "FK_ArticleReferences_Articles_ArticleId" FOREIGN KEY ("ArticleId") REFERENCES "Articles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ArticleReferences_Entries_EntryId" FOREIGN KEY ("EntryId") REFERENCES "Entries" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CategoryEntries" (
    "CategoryId" text NOT NULL,
    "EntryId" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_CategoryEntries" PRIMARY KEY ("CategoryId", "EntryId"),
    CONSTRAINT "FK_CategoryEntries_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CategoryEntries_Entries_EntryId" FOREIGN KEY ("EntryId") REFERENCES "Entries" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CollectionEntries" (
    "CollectionId" text NOT NULL,
    "EntriesId" text NOT NULL,
    CONSTRAINT "PK_CollectionEntries" PRIMARY KEY ("CollectionId", "EntriesId"),
    CONSTRAINT "FK_CollectionEntries_Collections_CollectionId" FOREIGN KEY ("CollectionId") REFERENCES "Collections" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CollectionEntries_Entries_EntriesId" FOREIGN KEY ("EntriesId") REFERENCES "Entries" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Resources" (
    "Id" text NOT NULL,
    "EntryId" text NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Path" character varying(2000) NOT NULL,
    "Type" character varying(50) NOT NULL,
    "ContentType" character varying(100) NOT NULL,
    "Size" bigint NOT NULL,
    "Hash" character varying(128) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Resources" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Resources_Entries_EntryId" FOREIGN KEY ("EntryId") REFERENCES "Entries" ("Id") ON DELETE CASCADE
);

CREATE TABLE "EntryLabels" (
    "EntryId" text NOT NULL,
    "LabelId" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_EntryLabels" PRIMARY KEY ("EntryId", "LabelId"),
    CONSTRAINT "FK_EntryLabels_Entries_EntryId" FOREIGN KEY ("EntryId") REFERENCES "Entries" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EntryLabels_Labels_LabelId" FOREIGN KEY ("LabelId") REFERENCES "Labels" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_ArticleReferences_EntryId" ON "ArticleReferences" ("EntryId");

CREATE INDEX "IX_Categories_CatalogId" ON "Categories" ("CatalogId");

CREATE INDEX "IX_Categories_ParentId" ON "Categories" ("ParentId");

CREATE INDEX "IX_CategoryEntries_EntryId" ON "CategoryEntries" ("EntryId");

CREATE INDEX "IX_CollectionEntries_EntriesId" ON "CollectionEntries" ("EntriesId");

CREATE INDEX "IX_Collections_CatalogId" ON "Collections" ("CatalogId");

CREATE INDEX "IX_Entries_CatalogId" ON "Entries" ("CatalogId");

CREATE INDEX "IX_EntryLabels_LabelId" ON "EntryLabels" ("LabelId");

CREATE INDEX "IX_Labels_CatalogId" ON "Labels" ("CatalogId");

CREATE INDEX "IX_Resources_EntryId" ON "Resources" ("EntryId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241117045717_AddLibraryModels', '8.0.8');

COMMIT;

