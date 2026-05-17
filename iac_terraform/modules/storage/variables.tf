# =============================================================================
# variables.tf
# =============================================================================
# [OBJETIVO]: Definir variables que serán usadas para construir el Storage Account.
# En main.tf usarás estas variables así:  var.nombre_de_la_variable
# Similar a argumentos en una método.
# =============================================================================

variable "location" {
  description = "Región de Azure donde se crearán los recursos"
  type        = string
  default     = "westus3"   # Si el usuario no pone un valor en .tfvars, se usa este default
}

variable "environment" {
  description = "Entorno del proyecto: dev, staging o prod"
  type        = string
  default     = "Development"
}

variable "resource_group_name" {
  type        = string
  description = "Nombre del grupo de recursos"
}

variable "storage_account_name" {
  type = string
  description = "Nombre del storage account"
}

variable "storage_account_tier" {
  type      = string
  description = "Standard / otros"
}

variable "storage_account_replication_type" {
  type      = string
  description = "Tipo de redundancia (backups) en storage: LRS (Redundancia Local),GRS (Redundancia Geográfica)"
}

variable "storage_account_kind" {
  type      = string
}

variable "storage_access_tier" {
  type      = string
  description = "Tipo de acceso según lectura y aceso: Hot (altamente accedidos), Cool (esporádico), cold (archivos históricos"
}

variable "storage_min_tls_version" {
  type      = string
  description = "Configura la mínima seguridad para los protocolos de transferencia."
}

variable "storage_container_name" {
  type      = string
  description = "Nombre del contenedor del SA"
}

variable "storage_container_access_type" {
  type      = string
  description = "Tipo de accesop: blob (permite sólo acceso de lectura por URL del archivo), private, container"
}


