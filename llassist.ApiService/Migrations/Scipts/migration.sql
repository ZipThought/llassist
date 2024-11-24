CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE TABLE "EstimateRelevanceJobs" (
        "Id" text NOT NULL,
        "ModelName" text NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "TotalArticles" integer NOT NULL,
        "ProjectId" text NOT NULL,
        CONSTRAINT "PK_EstimateRelevanceJobs" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE TABLE "Projects" (
        "Id" text NOT NULL,
        "Name" text NOT NULL,
        "Description" text NOT NULL,
        CONSTRAINT "PK_Projects" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE TABLE "ProjectDefinitions" (
        "Id" text NOT NULL,
        "Definition" text NOT NULL,
        "ProjectId" text NOT NULL,
        CONSTRAINT "PK_ProjectDefinitions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ProjectDefinitions_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE TABLE "ResearchQuestions" (
        "Id" text NOT NULL,
        "QuestionText" text NOT NULL,
        "ProjectId" text NOT NULL,
        CONSTRAINT "PK_ResearchQuestions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ResearchQuestions_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE TABLE "ArticleKeySemantics" (
        "Id" text NOT NULL,
        "KeySemantics" jsonb NOT NULL,
        CONSTRAINT "PK_ArticleKeySemantics" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ArticleKeySemantics_Articles_Id" FOREIGN KEY ("Id") REFERENCES "Articles" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE TABLE "QuestionDefinitions" (
        "Id" text NOT NULL,
        "Definition" text NOT NULL,
        "ResearchQuestionId" text NOT NULL,
        CONSTRAINT "PK_QuestionDefinitions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_QuestionDefinitions_ResearchQuestions_ResearchQuestionId" FOREIGN KEY ("ResearchQuestionId") REFERENCES "ResearchQuestions" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE UNIQUE INDEX "IX_ArticleRelevances_ArticleId" ON "ArticleRelevances" ("ArticleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE UNIQUE INDEX "IX_ArticleRelevances_ArticleId_EstimateRelevanceJobId" ON "ArticleRelevances" ("ArticleId", "EstimateRelevanceJobId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE INDEX "IX_Articles_ProjectId" ON "Articles" ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE INDEX "IX_EstimateRelevanceJobs_ProjectId" ON "EstimateRelevanceJobs" ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE INDEX "IX_ProjectDefinitions_ProjectId" ON "ProjectDefinitions" ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE INDEX "IX_QuestionDefinitions_ResearchQuestionId" ON "QuestionDefinitions" ("ResearchQuestionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE INDEX "IX_ResearchQuestions_ProjectId" ON "ResearchQuestions" ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    CREATE INDEX "IX_Snapshots_EstimateRelevanceJobId" ON "Snapshots" ("EstimateRelevanceJobId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004060013_InitDatabase') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20241004060013_InitDatabase', '8.0.8');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004093219_AddCompletedArticleCountToJob') THEN
    ALTER TABLE "EstimateRelevanceJobs" ADD "CompletedArticles" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241004093219_AddCompletedArticleCountToJob') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20241004093219_AddCompletedArticleCountToJob', '8.0.8');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241006032228_AllowMultipleRelevances') THEN
    DROP INDEX "IX_ArticleRelevances_ArticleId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241006032228_AllowMultipleRelevances') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20241006032228_AllowMultipleRelevances', '8.0.8');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007043522_ConvertArticlesWeakEntity') THEN
    ALTER TABLE "ArticleKeySemantics" DROP CONSTRAINT "FK_ArticleKeySemantics_Articles_Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007043522_ConvertArticlesWeakEntity') THEN
    ALTER TABLE "ArticleRelevances" DROP CONSTRAINT "PK_ArticleRelevances";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007043522_ConvertArticlesWeakEntity') THEN
    DROP INDEX "IX_ArticleRelevances_ArticleId_EstimateRelevanceJobId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007043522_ConvertArticlesWeakEntity') THEN
    ALTER TABLE "ArticleRelevances" DROP COLUMN "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007043522_ConvertArticlesWeakEntity') THEN
    ALTER TABLE "ArticleKeySemantics" RENAME COLUMN "Id" TO "ArticleId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007043522_ConvertArticlesWeakEntity') THEN
    ALTER TABLE "ArticleRelevances" ADD CONSTRAINT "PK_ArticleRelevances" PRIMARY KEY ("ArticleId", "EstimateRelevanceJobId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007043522_ConvertArticlesWeakEntity') THEN
    ALTER TABLE "ArticleKeySemantics" ADD CONSTRAINT "FK_ArticleKeySemantics_Articles_ArticleId" FOREIGN KEY ("ArticleId") REFERENCES "Articles" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007043522_ConvertArticlesWeakEntity') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20241007043522_ConvertArticlesWeakEntity', '8.0.8');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleRelevances" DROP CONSTRAINT "PK_ArticleRelevances";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleKeySemantics" DROP CONSTRAINT "PK_ArticleKeySemantics";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleRelevances" DROP COLUMN "Relevances";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleKeySemantics" DROP COLUMN "KeySemantics";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleRelevances" RENAME COLUMN "MustRead" TO "IsRelevant";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "Articles" ADD "MustRead" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleRelevances" ADD "RelevanceIndex" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleRelevances" ADD "ContributionReason" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleRelevances" ADD "ContributionScore" double precision NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleRelevances" ADD "IsContributing" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleRelevances" ADD "Question" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleRelevances" ADD "RelevanceReason" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleRelevances" ADD "RelevanceScore" double precision NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleKeySemantics" ADD "KeySemanticIndex" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleKeySemantics" ADD "Type" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleKeySemantics" ADD "Value" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleRelevances" ADD CONSTRAINT "PK_ArticleRelevances" PRIMARY KEY ("ArticleId", "EstimateRelevanceJobId", "RelevanceIndex");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    ALTER TABLE "ArticleKeySemantics" ADD CONSTRAINT "PK_ArticleKeySemantics" PRIMARY KEY ("ArticleId", "KeySemanticIndex");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241007062429_FixArticleKeySemanticsAndRelevances') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20241007062429_FixArticleKeySemanticsAndRelevances', '8.0.8');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241031205714_AppSetting') THEN
    CREATE TABLE "AppSettings" (
        "Key" character varying(100) NOT NULL,
        "Value" text NOT NULL,
        "Description" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        CONSTRAINT "PK_AppSettings" PRIMARY KEY ("Key")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241031205714_AppSetting') THEN
    INSERT INTO "AppSettings" ("Key", "Value", "Description", "CreatedAt")
    VALUES ('OpenAI:ApiKey', '', 'OpenAI API Key for LLM operations', TIMESTAMPTZ '2024-11-24T12:11:47.769456Z');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241031205714_AppSetting') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20241031205714_AppSetting', '8.0.8');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE TABLE "Catalogs" (
        "Id" text NOT NULL,
        "Name" character varying(200) NOT NULL,
        "Description" character varying(2000) NOT NULL,
        "Owner" character varying(100) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        CONSTRAINT "PK_Catalogs" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE TABLE "Entries" (
        "Id" text NOT NULL,
        "EntryType" character varying(50) NOT NULL,
        "Title" character varying(500) NOT NULL,
        "Description" character varying(5000) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "PublishedAt" timestamp with time zone,
        "CatalogId" text NOT NULL,
        "CitationFields" jsonb NOT NULL,
        "MetadataFields" jsonb NOT NULL,
        CONSTRAINT "PK_Entries" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Entries_Catalogs_CatalogId" FOREIGN KEY ("CatalogId") REFERENCES "Catalogs" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE TABLE "Labels" (
        "Id" text NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Description" character varying(500) NOT NULL,
        "Color" character varying(7) NOT NULL,
        "CatalogId" text NOT NULL,
        CONSTRAINT "PK_Labels" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Labels_Catalogs_CatalogId" FOREIGN KEY ("CatalogId") REFERENCES "Catalogs" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE TABLE "ArticleReferences" (
        "ArticleId" text NOT NULL,
        "EntryId" text NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_ArticleReferences" PRIMARY KEY ("ArticleId", "EntryId"),
        CONSTRAINT "FK_ArticleReferences_Articles_ArticleId" FOREIGN KEY ("ArticleId") REFERENCES "Articles" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ArticleReferences_Entries_EntryId" FOREIGN KEY ("EntryId") REFERENCES "Entries" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE TABLE "CategoryEntries" (
        "CategoryId" text NOT NULL,
        "EntryId" text NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_CategoryEntries" PRIMARY KEY ("CategoryId", "EntryId"),
        CONSTRAINT "FK_CategoryEntries_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_CategoryEntries_Entries_EntryId" FOREIGN KEY ("EntryId") REFERENCES "Entries" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE TABLE "CollectionEntries" (
        "CollectionId" text NOT NULL,
        "EntriesId" text NOT NULL,
        CONSTRAINT "PK_CollectionEntries" PRIMARY KEY ("CollectionId", "EntriesId"),
        CONSTRAINT "FK_CollectionEntries_Collections_CollectionId" FOREIGN KEY ("CollectionId") REFERENCES "Collections" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_CollectionEntries_Entries_EntriesId" FOREIGN KEY ("EntriesId") REFERENCES "Entries" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE TABLE "EntryLabels" (
        "EntryId" text NOT NULL,
        "LabelId" text NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_EntryLabels" PRIMARY KEY ("EntryId", "LabelId"),
        CONSTRAINT "FK_EntryLabels_Entries_EntryId" FOREIGN KEY ("EntryId") REFERENCES "Entries" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_EntryLabels_Labels_LabelId" FOREIGN KEY ("LabelId") REFERENCES "Labels" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE INDEX "IX_ArticleReferences_EntryId" ON "ArticleReferences" ("EntryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE INDEX "IX_Categories_CatalogId" ON "Categories" ("CatalogId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE INDEX "IX_Categories_ParentId" ON "Categories" ("ParentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE INDEX "IX_CategoryEntries_EntryId" ON "CategoryEntries" ("EntryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE INDEX "IX_CollectionEntries_EntriesId" ON "CollectionEntries" ("EntriesId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE INDEX "IX_Collections_CatalogId" ON "Collections" ("CatalogId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE INDEX "IX_Entries_CatalogId" ON "Entries" ("CatalogId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE INDEX "IX_EntryLabels_LabelId" ON "EntryLabels" ("LabelId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE INDEX "IX_Labels_CatalogId" ON "Labels" ("CatalogId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    CREATE INDEX "IX_Resources_EntryId" ON "Resources" ("EntryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20241124121113_AddLibraryModels') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20241124121113_AddLibraryModels', '8.0.8');
    END IF;
END $EF$;
COMMIT;

