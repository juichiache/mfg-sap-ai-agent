resource "azurerm_key_vault" "keyvault" {
  name                = "${var.environment}-${var.keyvault_name}-${var.suffix}"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku_name            = var.keyvault_sku_name
  tenant_id           = var.tenant_id

  soft_delete_retention_days = var.keyvault_soft_delete_retention_days
  purge_protection_enabled   = var.keyvault_purge_protection_enabled

  tags = merge(var.tags, {
    environment = var.environment
  })
}

resource "azurerm_key_vault_access_policy" "keyvault_access_policy" {
  count        = var.keyvault_access_policy_enabled ? 1 : 0
  key_vault_id = azurerm_key_vault.keyvault.id
  tenant_id    = var.tenant_id
  object_id    = var.keyvault_access_policy_object_id

  key_permissions         = var.keyvault_key_permissions
  secret_permissions      = var.keyvault_secret_permissions
  certificate_permissions = var.keyvault_certificate_permissions

  depends_on = [azurerm_key_vault.keyvault]
}

output "keyvault_id" {
  value = azurerm_key_vault.keyvault.id
}
output "keyvault_name" {
  value = azurerm_key_vault.keyvault.name
}
output "keyvault_uri" {
  value = azurerm_key_vault.keyvault.vault_uri
}
output "keyvault_identity_principal_id" {
  value = azurerm_key_vault.keyvault.identity[0].principal_id
}