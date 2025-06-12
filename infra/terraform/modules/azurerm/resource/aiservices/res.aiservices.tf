resource "azurerm_ai_services" "ai_services" {
  name                = "${var.ai_services_name}-${var.suffix}"
  location            = var.location
  resource_group_name = var.resource_group_name
  kind                = "AI"
  sku_name            = var.ai_services_sku_name

  public_network_access_enabled = true
  custom_sub_domain_name        = "${var.environment}-${var.ai_services_name}-${var.suffix}"

  identity {
    type = "SystemAssigned"
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}

output "ai_services_id" {
  value = azurerm_ai_services.ai_services.id
}
output "ai_services_name" {
  value = azurerm_ai_services.ai_services.name
}
output "ai_services_endpoint" {
  value = azurerm_ai_services.ai_services.endpoint
}
output "ai_services_identity_principal_id" {
  value = azurerm_ai_services.ai_services.identity[0].principal_id
}