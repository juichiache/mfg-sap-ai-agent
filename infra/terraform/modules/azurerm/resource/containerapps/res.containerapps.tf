resource "azurerm_container_app_environment" "cap_environment" {
  name                       = "${var.environment}-${var.containerapp_environment_name}-${var.suffix}"
  location                   = var.location
  resource_group_name        = var.resource_group_name
  logs_destination           = "log-analytics"
  log_analytics_workspace_id = var.log_analytics_workspace_id
}

resource "azurerm_container_app" "container_app" {
  name                         = "${var.environment}-${var.container_app_name}-${var.suffix}"
  container_app_environment_id = azurerm_container_app_environment.cap_environment.id
  resource_group_name          = var.resource_group_name
  revision_mode                = var.revision_mode

  tags = merge(var.tags, {
    environment = var.environment
  })

  template {
    container {
      name   = var.container_name
      image  = var.container_image
      cpu    = var.container_cpu
      memory = var.container_memory
    }
  }
}

output "container_app_id" {
  value = azurerm_container_app.container_app.id
}
output "container_app_name" {
  value = azurerm_container_app.container_app.name
}
output "cap_environment_id" {
  value = azurerm_container_app_environment.cap_environment.id
}
output "cap_environment_name" {
  value = azurerm_container_app_environment.cap_environment.name
}
