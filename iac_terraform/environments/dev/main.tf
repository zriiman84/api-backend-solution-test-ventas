# =============================================================================
# main.tf
# =============================================================================
# [OBJETIVO]: Orquestar todos los recursos invocando a los módulos.
# Este es el archivo principal. No contiene lógica de recursos, sino consume a los módulos.
# environment/.../variable.tfvars > environment/.../variable.tf > moodulos
# Las variables de los módulos (izquierda) son las definidas en modules/../variables.tf
# Los valores de la derecha son los definidos en environents/../variables.tf
# Imaginemos que esta es una instancia de una clase que define variables con valores.
# Invocará a los métodos (que son los módulos) enviándoles los valores como parametros.
# =============================================================================

# 1. Definición del Proveedor
terraform {
  required_version = ">= 1.0.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}
# Configuración del Proveedor AzureRM
provider "azurerm" {
  features {}
}

# El Grupo de Recursos se administra desde la raíz del entorno
# Aquí creo el resoruce group (no lo hago en los módulos)
resource "azurerm_resource_group" "rg_nuevo" {
  name     = var.resource_group_name_input
  location = var.location_input
}

# Invocación del Módulo de Base de Datos
module "database_dev" {
  source              = "../../modules/database"
  resource_group_name = azurerm_resource_group.rg_nuevo.name
  location            = azurerm_resource_group.rg_nuevo.location
  server_db_name      = var.server_db_name_input
  admin_login         = var.admin_login_input
  admin_password      = var.admin_password_input
  database_name       = var.database_name_input
  db_sku_name         = var.db_sku_name_input
  db_max_size_gb      = var.db_max_size_gb_input
  db_storage_account_type = var.db_storage_account_type_input
  environment         = var.environment_input
  client_ip           = var.client_ip_input
}

# Invocación del Módulo de Almacenamiento
module "storage_dev" {
  source               = "../../modules/storage"
  resource_group_name  = azurerm_resource_group.rg_nuevo.name
  location             = azurerm_resource_group.rg_nuevo.location
  storage_account_name = var.storage_account_name_input
  storage_account_tier              = var.storage_account_tier_input
  storage_account_replication_type  = var.storage_account_replication_type_input
  storage_account_kind              = var.storage_account_kind_input
  storage_access_tier               = var.storage_access_tier_input
  storage_min_tls_version           = var.storage_min_tls_version_input
  environment                       = var.environment_input
  storage_container_name            = var.storage_container_name_input
  storage_container_access_type     = var.storage_container_access_type_input
}