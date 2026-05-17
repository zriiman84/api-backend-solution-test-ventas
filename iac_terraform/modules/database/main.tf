# =============================================================================
# main.tf
# =============================================================================
# [OBJETIVO]: Crear los recursos de base de datos (servidor y base de datos). 
# [DESCRIPCIÓN]:
# Module: Es como la definición de métodos con lógica (crear recursos) que serán 
# consumidos por los environments.
# Aquí consumimos:
#   - var.X        → valores del usuario (declarados en variables.tf)
#   - local.X      → valores calculados (declarados en locals.tf)
# =============================================================================

# Servidor de BD
resource "azurerm_mssql_server" "sqlserver" {
  name                         = var.server_db_name
  resource_group_name          = var.resource_group_name
  location                     = var.location
  version                      = "12.0"  # Corresponde a SQL Server 2022 en Azure
  administrator_login          = var.admin_login
  administrator_login_password = var.admin_password
  
  public_network_access_enabled = true
}

# Base de datos
resource "azurerm_mssql_database" "dbsales" {
  name                 = var.database_name
  server_id            = azurerm_mssql_server.sqlserver.id
  collation            = "SQL_Latin1_General_CP1_CI_AS"
  # SKU Básico 
  sku_name              = var.db_sku_name
  max_size_gb           = var.db_max_size_gb

  # Backup con redundancia local (LRS)
  storage_account_type  = var.db_storage_account_type

  tags = {
    environment = var.environment #workload environment
  }
}

# Firewall
# Regla para tu IP actual
resource "azurerm_mssql_firewall_rule" "current_client_ip" {
  name             = "AccesoLocal"
  server_id        = azurerm_mssql_server.sqlserver.id
  start_ip_address = var.client_ip
  end_ip_address   = var.client_ip
}

resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAllAzureServices"
  server_id        = azurerm_mssql_server.sqlserver.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}