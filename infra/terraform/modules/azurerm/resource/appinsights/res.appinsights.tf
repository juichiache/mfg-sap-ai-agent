resource "azurerm_application_insights" "appinsights" {
  name                = "${var.sap-agent-appinsights}-${var.suffix}"
  location            = var.location
  resource_group_name = var.resource_group_name
  application_type    = "web"

  identity {
    type = "SystemAssigned"
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}

output "appinsights_id" {
  value = azurerm_application_insights.appinsights.id
}
output "appinsights_instrumentation_key" {
  value = azurerm_application_insights.appinsights.instrumentation_key
}
output "appinsights_name" {
  value = azurerm_application_insights.appinsights.name
}
