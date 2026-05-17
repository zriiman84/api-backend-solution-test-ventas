# =============================================================================
# variables.tf
# =============================================================================
# [OBJETIVO]: Definir variables que serán usadas como parámetros a enviarse a los modules.
# Aquí definiremos todas las variables necesarias para la creación de todos los recursos.
# En main.tf usarás estas variables así:  var.nombre_de_la_variable
# =============================================================================

#===========================================
# Ambiente / entorno
#===========================================
variable "location_input" {
  type        = string
  default     = "westus3"   # Si el usuario no pone un valor en .tfvars, se usa este default
}

variable "environment_input" {
  type        = string
  default     = "Development"
}

#===========================================
# Resource group
#===========================================
variable "resource_group_name_input" {
  type        = string
}

#===========================================
# Server SQL
#===========================================
variable "server_db_name_input" {
  type        = string
}

variable "admin_login_input" {
  type        = string
}

variable "admin_password_input" {
  type        = string
  sensitive   = true  # Esto evita que la clave salga en los logs de consola
}

#===========================================
# Database
#===========================================
variable "database_name_input" {
  type        = string
}

variable "db_sku_name_input" {
  type      = string
  default     = "Basic"
}

variable "db_max_size_gb_input" {
  type      = number
  default = 2
}

variable "db_storage_account_type_input" {
  type      = string
}

# Firewall
variable "client_ip_input" {
  type        = string
}

#===========================================
# Storage account y container
#===========================================
variable "storage_account_name_input" {
  type = string
}

variable "storage_account_tier_input" {
  type      = string
}

variable "storage_account_replication_type_input" {
  type      = string
}

variable "storage_account_kind_input" {
  type      = string
}

variable "storage_access_tier_input" {
  type      = string
}

variable "storage_min_tls_version_input" {
  type      = string
}

variable "storage_container_name_input" {
  type      = string
}

variable "storage_container_access_type_input" {
  type      = string
}