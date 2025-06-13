resource "azurerm_ai_foundry" "aifoundry" {
  name                 = "${var.environment}-${var.ai_foundry_name}-${var.suffix}"
  location             = var.location
  resource_group_name  = var.resource_group_name
  storage_account_name = var.ai_foundry_storage_account_name
  key_vault_id         = var.ai_foundry_key_vault_id

  tags = merge(var.tags, {
    environment = var.environment
  })

  identity {
    type = "SystemAssigned"
  }
}

output "ai_foundry_id" {
  value = azurerm_ai_foundry.aifoundry.id
}
output "ai_foundry_name" {
  value = azurerm_ai_foundry.aifoundry.name
}
output "ai_foundry_endpoint" {
  value = azurerm_ai_foundry.aifoundry.endpoint
}
output "ai_foundry_identity_principal_id" {
  value = azurerm_ai_foundry.aifoundry.identity[0].principal_id
}