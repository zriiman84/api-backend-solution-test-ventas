# =============================================================================
# terraform.tfvars
# =============================================================================
# [OBJETIVO]: Asignar los valores reales a las variables declaradas en variables.tf.
# IMPORTANTE: Este archivo puede contener datos sensibles (contraseñas).
# Agrégalo a .gitignore si trabajas con git.
# =============================================================================

#===========================================
# Ambiente / entorno
#===========================================
location_input = "westus3"
environment_input = "Development"

#===========================================
# Resource group
#===========================================
resource_group_name_input = "rg-solutiontestventas-sales-dmc"

#===========================================
# Servidor de Base de datos
#===========================================
server_db_name_input    = "sqlserverazure-sales-dmc"
admin_login_input       = "sa_admin"
admin_password_input    = "Pa@ssw0rd.2026*+" # Terraform sabe que es 'sensitive'

#===========================================
# Base de datos
#===========================================
database_name_input               = "SalesDB"
db_sku_name_input                 = "Basic"
db_max_size_gb_input              = 2
db_storage_account_type_input       = "Local"
client_ip_input                      = "38.253.148.13"

#===========================================
# Storage account y container
#===========================================
storage_account_name_input    = "solutiontestventassadmc"
storage_account_tier_input    = "Standard"
storage_account_replication_type_input  = "LRS"
storage_account_kind_input      =  "StorageV2"
storage_access_tier_input       =  "Hot"
storage_min_tls_version_input   =  "TLS1_2"
storage_container_name_input    = "productos"
storage_container_access_type_input = "blob"