resource "azurerm_linux_function_app" "linux_functions" {
  count                = var.function_app_kind == "Linux" ? 1 : 0
  name                 = "${var.environment}-${var.functionappname}-${var.suffix}"
  location             = var.location
  resource_group_name  = var.resource_group_name
  service_plan_id      = var.appserviceplan_id
  storage_account_name = var.storage_account_name
  #   storage_account_access_key = azurerm_storage_account.functions.primary_access_key


  https_only = true

  site_config {
    application_stack {

    }
  }

  identity {
    type = "SystemAssigned"
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}

# Compare this snippet from infra/terraform/modules/azurerm/resource/functions/res.functions.tf:
resource "azurerm_windows_function_app" "windows_functions" {
  count                = var.function_app_kind == "Windows" ? 1 : 0
  name                 = "${var.environment}-${var.functionappname}-${var.suffix}"
  location             = var.location
  resource_group_name  = var.resource_group_name
  service_plan_id      = var.appserviceplan_id
  storage_account_name = var.storage_account_name
  #   storage_account_access_key = azurerm_storage_account.functions.primary_access_key

  https_only = true

  site_config {
    application_stack {
    }
  }

  identity {
    type = "SystemAssigned"
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}

resource "azurerm_function_app_flex_consumption" "flex_consumption" {
  count                = var.function_app_kind == "FlexConsumption" ? 1 : 0
  name                 = "${var.environment}-${var.functionappname}-${var.suffix}"
  location             = var.location
  resource_group_name  = var.resource_group_name
  app_service_plan_id  = var.appserviceplan_id
  storage_account_name = var.storage_account_name
  #   storage_account_access_key = azurerm_storage_account.functions.primary_access_key

  version    = "~4"
  os_type    = "Linux"
  https_only = true

  site_config {
    application_stack {
      docker_image = var.docker_image
    }
  }

  identity {
    type = "SystemAssigned"
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}

output "flex_consumption_id" {
  value = azurerm_function_app_flex_consumption.flex_consumption.id
}
output "flex_consumption_name" {
  value = azurerm_function_app_flex_consumption.flex_consumption.name
}
output "flex_consumption_default_hostname" {
  value = azurerm_function_app_flex_consumption.flex_consumption.default_hostname
}
output "flex_consumption_identity_principal_id" {
  value = azurerm_function_app_flex_consumption.flex_consumption.identity[0].principal_id
}
output "linux_functionapp_id" {
  value = azurerm_linux_function_app.linux_functions.id
}
output "linux_functionapp_name" {
  value = azurerm_linux_function_app.linux_functions.name
}
output "linux_functionapp_default_hostname" {
  value = azurerm_linux_function_app.linux_functions.default_hostname
}
output "linux_functionapp_identity_principal_id" {
  value = azurerm_linux_function_app.linux_functions.identity[0].principal_id
}