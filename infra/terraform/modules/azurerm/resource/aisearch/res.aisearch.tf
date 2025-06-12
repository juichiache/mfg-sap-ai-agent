resource "azurerm_search_service" "aisearch" {
  name                   = "${var.aisearch_name}-${var.suffix}"
  location               = var.location
  resource_group_name    = var.resource_group_name
  custom_sub_domain_name = "${var.environment}-${var.aisearch_name}-${var.suffix}"

  sku                           = var.aisearch_sku_name
  public_network_access_enabled = true

  identity {
    type = "SystemAssigned"
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}

output "aisearch_id" {
  value = azurerm_search_service.aisearch.id
}
output "aisearch_name" {
  value = azurerm_search_service.aisearch.name
}
output "aisearch_endpoint" {
  value = azurerm_search_service.aisearch.endpoint
}
output "aisearch_identity_principal_id" {
  value = azurerm_search_service.aisearch.identity[0].principal_id
}
output "aisearch_sku_name" {
  value = azurerm_search_service.aisearch.sku[0].name
}