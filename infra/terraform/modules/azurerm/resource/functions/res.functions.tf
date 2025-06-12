resource "azurerm_function_app" "functions" {
  name                       = "${var.functionappname}-${var.suffix}"
  location                   = var.location
  resource_group_name        = var.resource_group_name
  app_service_plan_id        = var.appserviceplan_id
  storage_account_name       = var.storage_account_name
#   storage_account_access_key = azurerm_storage_account.functions.primary_access_key

  version                    = "~4"
  os_type                    = var.appserviceplan_kind == "Linux" ? "Linux" : "Windows"
  https_only                 = true

  site_config {
    application_stack {
      docker_image = "mcr.microsoft.com/azure-functions/dotnet:4"
    }
  }

  identity {
    type = "SystemAssigned"
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}

output "functionapp_id" {
  value = azurerm_function_app.functions.id
}
output "functionapp_name" {
  value = azurerm_function_app.functions.name
}
output "functionapp_default_hostname" {
  value = azurerm_function_app.functions.default_hostname
}
output "functionapp_identity_principal_id" {
  value = azurerm_function_app.functions.identity[0].principal_id
}