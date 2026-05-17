# =============================================================================
# outputs.tf
# =============================================================================
# [OBJETIVO]: Mostrar los valores de los recursos una vez creados.
# [DESCRIPCIÓN]:
# Los "outputs" son los DATOS que Terraform muestra al terminar el "apply".
# Son útiles para:
#   - Ver el hostname del servidor sin entrar al portal de Azure
#   - Pasar datos a otro módulo de Terraform que dependa de este
#   - Guardar datos en un sistema de CI/CD (ej: Azure DevOps)
# =============================================================================

output "sql_server_id" {
  description = "ID del SQL Server en Azure (útil para otros módulos Terraform)"
  value = azurerm_mssql_server.sqlserver.id
}

output "sql_server_fqdn" {
  description = "Hostname completo para conectarse al SQL Server"
  value = azurerm_mssql_server.sqlserver.fully_qualified_domain_name
}

output "database_name" {
  description = "Nombre de la base de datos creada"
  value       = azurerm_mssql_database.dbsales.name
}

output "connection_string" {
  description = "Cadena de conexión lista para usar en tu app"
  value = "Server=tcp:${azurerm_mssql_server.sqlserver.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.dbsales.name};Persist Security Info=False;User ID=${var.admin_login};Password=xxxxxxxx;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  # NOTA: nunca pongas la contraseña real en un output (quedaría en el state file)
}


