# =============================================================================
# main.tf
# =============================================================================
# [OBJETIVO]: Crear el storage account y container donde se almacenarán las imágenes de los productos de la app.
# =============================================================================


# 8. Crear el Storage Account
resource "azurerm_storage_account" "sa" {
  name                     = var.storage_account_name
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = var.storage_account_tier
  account_replication_type = var.storage_account_replication_type
  account_kind             = var.storage_account_kind
  access_tier              = var.storage_access_tier
  min_tls_version          = var.storage_min_tls_version

  # Control de acceso público a los Blobs
  public_network_access_enabled = true

  # OBLIGATORIO: Permite que los contenedores hijos puedan heredar accesos públicos
  allow_nested_items_to_be_public = true

  tags = {
    environment = var.environment
  }
}

# 9. Crear un contenedor
resource "azurerm_storage_container" "container" {
  name                  = var.storage_container_name
  storage_account_name  = azurerm_storage_account.sa.name
  container_access_type = var.storage_container_access_type 
}