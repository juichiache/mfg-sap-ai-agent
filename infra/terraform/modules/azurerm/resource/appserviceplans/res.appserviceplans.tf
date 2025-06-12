resource "azurerm_app_service_plan" "appserviceplans" {
  name                = var.appserviceplan-"${var.suffix}"
  location            = var.location
  resource_group_name = var.resource_group_name
  kind                = var.appserviceplan_kind
  reserved            = true

  sku {
    tier     = var.appserviceplan_tier
    size     = var.appserviceplan_size
    capacity = 1
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}

output "appserviceplan_id" {
  value = azurerm_app_service_plan.appserviceplans.id
}
output "appserviceplan_name" {
  value = azurerm_app_service_plan.appserviceplans.name
}