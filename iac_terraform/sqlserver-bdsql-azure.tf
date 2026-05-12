# 1. Definición del Proveedor
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# 2. Variables
variable "db_user" {
  type      = string
}

variable "db_password" {
  type      = string
  sensitive = true  # Esto evita que la clave salga en los logs de consola
}

variable "db_name" {
  type      = string
}

# 3. Creo un Grupo de Recursos: El contenedor lógico en Azure
resource "azurerm_resource_group" "rg_nuevo" {
  name     = "rg-sales-sales-cvargas"
  location = "West US 3"
}

# 4. Servidor SQL Lógico: Reemplaza al motor del contenedor
resource "azurerm_mssql_server" "sqlserver" {
  name                         = "sqlserverazure-sales-cvargas" # Debe ser único en todo Azure
  resource_group_name          = azurerm_resource_group.rg_nuevo.name
  location                     = azurerm_resource_group.rg_nuevo.location
  version                      = "12.0" # Corresponde a SQL Server 2022 en Azure
  administrator_login          = var.db_user
  administrator_login_password = var.db_password

  # Asegurar que el endpoint sea público
  public_network_access_enabled = true
}

# 5. Base de Datos: Reemplaza a 'FakeDB'
resource "azurerm_mssql_database" "dbsales" {
  name           = var.db_name
  server_id      = azurerm_mssql_server.sqlserver.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  
  # SKU Básico 
  sku_name       = "Basic" 
  max_size_gb    = 2

  # Backup con redundancia local (LRS)
  storage_account_type = "Local" 

  tags = {
    environment = "Development" #workload environment
  }
}
# 6. Punto 4: Regla para tu IP actual (Sustituye 'tu_ip_aqui')
resource "azurerm_mssql_firewall_rule" "current_client_ip" {
  name             = "AccesoLocal"
  server_id        = azurerm_mssql_server.sqlserver.id
  start_ip_address = "38.253.148.13" # Mi Ip actual
  end_ip_address   = "38.253.148.13" # Mi Ip actual
}

# 7. Regla adicional para que otros servicios INTERNOS de Azure conecten
resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAllAzureServices"
  server_id        = azurerm_mssql_server.sqlserver.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}