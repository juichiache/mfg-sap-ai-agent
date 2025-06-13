resource "azurerm_cognitive_account" "openai" {
  name                          = "${var.environment}-${var.openai_name}-${var.suffix}"
  location                      = var.location
  resource_group_name           = var.resource_group_name
  kind                          = var.openai_kind
  sku_name                      = var.openai_sku_name
  public_network_access_enabled = true

  identity {
    type = "SystemAssigned"
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}

output "openai_id" {
  value = azurerm_cognitive_account.openai.id
}
output "openai_name" {
  value = azurerm_cognitive_account.openai.name
}
output "openai_endpoint" {
  value = azurerm_cognitive_account.openai.endpoint
}
output "openai_identity_principal_id" {
  value = azurerm_cognitive_account.openai.identity[0].principal_id
}