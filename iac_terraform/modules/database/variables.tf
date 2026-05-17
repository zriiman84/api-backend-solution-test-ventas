# =============================================================================
# variables.tf
# =============================================================================
# [OBJETIVO]: Definir variables que serán usadas para construir los recursos del servidor SQL y la base de datos.
# En main.tf usarás estas variables así:  var.nombre_de_la_variable
# Similar a argumentos en una método.
# =============================================================================

# Ambiente / entorno
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

# Server SQL
variable "resource_group_name" {
  type        = string
  description = "Nombre del grupo de recursos"
}

variable "server_db_name" {
  type        = string
  description = "Nombre único del servidor de base de datos"
}

variable "admin_login" {
  description = "Nombre de usuario administrador del SQL Server"
  type        = string
}

variable "admin_password" {
  description = "Contraseña del administrador"
  type        = string
  sensitive   = true  # Esto evita que la clave salga en los logs de consola
}


# Database
variable "database_name" {
  description = "Nombre de la base de datos a crear"
  type        = string
}

variable "db_sku_name" {
  description = "SKU de la base de datos. Opciones: Basic, S0, S1, GP_S_Gen5_1"
  type      = string
  default     = "Basic"
}

variable "db_max_size_gb" {
  description = "Tamaño máximo de la base de datos en GB"
  type      = number
  default = 2
}

variable "db_storage_account_type" {
  description = "Tipo de redundancia (backups) en storage: LRS (Redundancia Local),GRS (Redundancia Geográfica)"
  type      = string
}

# Firewall
variable "client_ip" {
  type        = string
  description = "Tu IP pública actual para la regla de Firewall"
}