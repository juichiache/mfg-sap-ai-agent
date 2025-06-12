resource "azurerm_ai_foundry" "aifoundry" {
  name                = "${var.aifoundry_name}-${var.suffix}"
  location            = var.location
  resource_group_name = var.resource_group_name
  storage_account_name = var.aifoundry_storage_account_name
  key_vault_id = var.aifoundry_key_vault_id

  tags = merge(var.tags, {
    environment = var.environment
  })

  identity {
    type = "SystemAssigned"
  }
}


output "aifoundry_id" {
  value = azurerm_ai_foundry.aifoundry.id
}
output "aifoundry_name" {
  value = azurerm_ai_foundry.aifoundry.name
}
output "aifoundry_endpoint" {
  value = azurerm_ai_foundry.aifoundry.endpoint
}
output "aifoundry_identity_principal_id" {
  value = azurerm_ai_foundry.aifoundry.identity[0].principal_id
}