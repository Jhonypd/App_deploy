-- ============================================
-- Script de Criação do Banco de Dados SQLite
-- Aplicação App Deploy
-- ============================================

-- Tabela SiteDeployConfig
CREATE TABLE IF NOT EXISTS SiteDeployConfig (
    Id TEXT NOT NULL PRIMARY KEY,
    ProjetoIis TEXT NOT NULL,
    Porta INTEGER NOT NULL DEFAULT 80,
    Svn TEXT,
    Destino TEXT NOT NULL,
    UrlManual TEXT,
    Atualizada INTEGER NOT NULL DEFAULT 0
);

-- Tabela SiteOrigemConfig
CREATE TABLE IF NOT EXISTS SiteOrigemConfig (
    Id TEXT NOT NULL PRIMARY KEY,
    SiteDeployConfigId TEXT NOT NULL,
    Path TEXT NOT NULL,
    Conteudo INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT FK_SiteOrigemConfig_SiteDeployConfig FOREIGN KEY (SiteDeployConfigId) 
        REFERENCES SiteDeployConfig(Id) ON DELETE CASCADE
);

-- Índices para melhor performance
CREATE INDEX IF NOT EXISTS IX_SiteOrigemConfig_SiteDeployConfigId 
    ON SiteOrigemConfig(SiteDeployConfigId);

-- Exemplo de dados iniciais (descomente se necessário)
-- INSERT INTO SiteDeployConfig (Id, ProjetoIis, Porta, Destino) 
-- VALUES ('site-001', 'MeuSite', 80, 'C:\inetpub\wwwroot\MeuSite');
