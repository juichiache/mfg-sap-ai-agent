resource "azurerm_container_registry" "container_registry" {
  name                = "${var.environment}-${var.container_registry_name}-${var.suffix}"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = var.container_registry_sku_name
  admin_enabled       = var.container_registry_admin_enabled

  identity {
    type = "SystemAssigned"
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}
output "containerregistry_id" {
  value = azurerm_container_registry.container_registry.id
}
output "containerregistry_name" {
  value = azurerm_container_registry.container_registry.name
}