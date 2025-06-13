resource "azurerm_log_analytics_workspace" "loganalytics" {
  name                = "${var.environment}-${var.log_analytics_workspace_name}-${var.suffix}"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = var.log_analytics_sku

  retention_in_days = var.log_analytics_retention_in_days

  identity {
    type = "SystemAssigned"
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}

output "log_analytics_workspace_id" {
  value = azurerm_log_analytics_workspace.loganalytics.id
}
output "loganalytics_workspace_name" {
  value = azurerm_log_analytics_workspace.loganalytics.name
}
output "loganalytics_workspace_primary_shared_key" {
  value = azurerm_log_analytics_workspace.loganalytics.primary_shared_key
}

# resource "azurerm_log_analytics_solution" "loganalytics_solution" {
#   solution_name         = var.loganalytics_solution_name
#   location              = var.location
#   resource_group_name   = var.resource_group_name
#   workspace_resource_id = azurerm_log_analytics_workspace.loganalytics.id

#   plan {
#     publisher = "Microsoft"
#     product   = var.loganalytics_solution_product
#   }

#   identity {
#     type = "SystemAssigned"
#   }

#   tags = merge(var.tags, {
#     environment = var.environment
#   })
# }