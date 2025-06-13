terraform {
  backend "azurerm" {
    use_cli              = true
    use_azuread_auth     = true
    resource_group_name  = "mfg-sap-ai-agents"
    storage_account_name = "mfgsapaiagents8099"
    container_name       = "tfstate"
    key                  = "dev-sap-ai-agents-tfstate"
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.26.0"
      # version = "~> 4.0"
    }
  }
  required_version = "~> 1.11.0"
}

resource "random_integer" "random_0_to_10" {
  min = 0
  max = 10
}

locals {
  suffix = "${substr(md5(var.environment), random_integer.random_0_to_10.result, 4)}"
  # suffix = "-${substr(md5(var.environment), 5, 4)}"
}

data "azurerm_client_config" "current" {
  # This resource is used to get the current Azure client configuration
  # It provides the client ID and tenant ID for use in other resources
}

data "azurerm_resource_group" "sap_agent_rg" {
  name = var.resource_group_name
  location = var.location
}

# TODO: build out these services
# Azure Functions 
# Azure Bot 
# Azure AI Foundry 
# Azure OpenAI 
# Azure AI Services 
# Grounding with Bing 
# Azure Maps 
# Container Apps 
# Azure AI Search

######################################################################################################################################################################
# Azure Blob Storage and Event Grid with storage queue
# This module sets up Azure Blob Storage and Event Grid with a storage queue for the SAP Agent application.
######################################################################################################################################################################

module "storage_account" {
  source = "./modules/azurerm/resource/storage"

  storage_account_name        = var.storage_account_name
  suffix                      = local.suffix
  storage_container_name               = var.storage_container_name
  storage_account_tier = var.storage_account_access_tier
  storage_account_replication = var.storage_account_replication

  location                    = var.location
  resource_group_name         = var.resource_group_name
  tags                        = var.tags
}
module "eventgrid_system_topic" {
  source = "./modules/azurerm/resource/eventgrid"

  eventgrid_event_subscription_name = var.eventgrid_event_subscription_name
  eventgrid_system_topic_name       = var.eventgrid_name
  suffix                            = local.suffix
  location                          = var.location
  resource_group_name               = var.resource_group_name
  storage_account_id                = module.storage_account.storage_account_id
  eventgrid_topic_type              = var.eventgrid_topic_type
  tags                              = var.tags
  environment                       = var.environment
}

resource "azurerm_storage_queue" "sap_agent_queue" {
  name                 = "${var.environment}-${var.storage_queue_name}-${local.suffix}"
  storage_account_name = module.storage_account.storage_account_name
  metadata = {
    environment = var.environment
  }
}


######################################################################################################################################################################
# Azure Functions and App Service Plans
# This module sets up Azure Functions and App Service Plans for the SAP Agent application.
# It includes the creation of an App Service Plan and a Function App.
######################################################################################################################################################################

module "appserviceplans" {
  source = "./modules/azurerm/resource/appserviceplans"

  suffix              = local.suffix
  location            = var.location
  environment         = var.environment
  resource_group_name = var.resource_group_name
  appserviceplan_name = var.appserviceplan_name
  appserviceplan_kind = var.appserviceplan_kind
  appserviceplan_tier = var.appserviceplan_tier
  appserviceplan_size = var.appserviceplan_size
  tags                = var.tags
}

module "functions" {
  source = "./modules/azurerm/resource/functions"

  location             = azurerm_resource_group.sap_agent_rg.location
  resource_group_name  = azurerm_resource_group.sap_agent_rg.name
  environment          = var.environment
  suffix               = local.suffix
  functionappname      = var.function_app_name
  appserviceplan_kind  = var.function_app_os_type
  function_app_sku_name = var.function_app_sku_name 
  docker_image         = var.docker_image
  appserviceplan_id    = module.appserviceplans.appserviceplan_id
  storage_account_name = module.storage_account.storage_account_name
  tags                 = var.tags
}

######################################################################################################################################################################
# Azure Monitoring and Application Insights
# This module sets up Azure Monitoring and Application Insights for the SAP Agent application.
# Log Analytics Workspace is also created for monitoring and logging purposes.
######################################################################################################################################################################

module "application_insights" {
  source                     = "./modules/azurerm/resource/appinsights"
  sap_agent_appinsights_name = "${var.environment}-${var.sap_agent_appinsights_name}-${local.suffix}"
  suffix                     = local.suffix
  location                   = azurerm_resource_group.sap_agent_rg.location
  resource_group_name        = azurerm_resource_group.sap_agent_rg.name
  tags                       = var.tags
}

module "log_analytics_workspace" {
  source                          = "./modules/azurerm/resource/loganalytics"
  log_analytics_workspace_name    = var.sap_agent_log_analytics_workspace_name
  suffix                          = local.suffix
  location                        = azurerm_resource_group.sap_agent_rg.location
  resource_group_name             = azurerm_resource_group.sap_agent_rg.name
  log_analytics_sku               = var.sap_agent_log_analytics_sku
  log_analytics_retention_in_days = var.sap_agent_log_analytics_retention_in_days
  tags                            = var.tags
}

######################################################################################################################################################################
# Azure Bot Service
# This module sets up an Azure Bot Service.
######################################################################################################################################################################

resource "azurerm_bot_service_azure_bot" "bot_service" {
  name                = "${var.environment}-${var.bot_service_name}-${local.suffix}"
  resource_group_name = azurerm_resource_group.sap_agent_rg.name
  location            = "global"
  microsoft_app_id    = data.azurerm_client_config.current.client_id
  sku                 = var.bot_service_sku

  endpoint                              = "https://example.com"
  developer_app_insights_api_key        = azurerm_application_insights_api_key.example.api_key
  developer_app_insights_application_id = azurerm_application_insights.example.app_id

  tags = {
    environment = "test"
  }
}

######################################################################################################################################################################
# Azure AI Foundry and Related Services
# This module sets up Azure AI Foundry and its related services such as OpenAI, AI Services, Bing Grounding, Maps, Container Apps, and Search.
#######################################################################################################################################################################

#--- Azure AI Foundry Configuration ---
module "ai_foundry" {
  source = "./modules/azurerm/resource/aifoundry"

  ai_foundry_key_vault_id = module.keyvault.key_vault_id
  ai_foundry_storage_account_name = module.storage_account.storage_account_name
  location            = azurerm_resource_group.sap_agent_rg.location
  resource_group_name = azurerm_resource_group.sap_agent_rg.name
  tags                = var.tags
}

#--- Azure OpenAI Configuration ---
module "openai" {
  source          = "./modules/azurerm/resource/openai"
  openai_name     = var.openai_name
  openai_kind     = var.openai_kind
  openai_sku_name = var.openai_sku_name

  location                = azurerm_resource_group.sap_agent_rg.location
  resource_group_name     = azurerm_resource_group.sap_agent_rg.name
  tags                    = var.tags
}

#--- OpenAI Model Deployment ---
resource "azurerm_cognitive_deployment" "model_deployment" {
  for_each             = { for key, value in var.openai_model_deployment : deployment.name => deployment }
  name                 = each.key
  cognitive_account_id = module.openai.openai_id

  model {
    format  = each.value.model.format
    name    = each.value.model.name
    version = each.value.model.version
  }
  
  sku {
    name    = each.value.sku.name
  }
  lifecycle {
    # ignore_changes = [ tags]
  }
}

#--- Container App Configuration ---
module "container_apps" {
  source              = "./modules/azurerm/resource/containerapps"
  container_app_name  = var.containerappname
  location            = data.azurerm_resource_group.sap_agent_rg.location
  resource_group_name = data.azurerm_resource_group.sap_agent_rg.name
  log_analytics_workspace_id = module.log_analytics_workspace.log_analytics_workspace_id
  container_image = var.container_image
  tags                = var.tags
}

#--- Azure AI Services Configuration ---
module "ai_services" {
  source              = "./modules/azurerm/resource/aiservices"
  ai_services_name    = var.ai_services_name
  ai_services_sku     = var.ai_services_sku
  ai_services_kind    = var.ai_services_kind
  location            = azurerm_resource_group.sap_agent_rg.location
  resource_group_name = azurerm_resource_group.sap_agent_rg.name
  tags                = var.tags
}

######################################################################################################################################################################
# Azure Maps Configuration
# This module sets up Azure Maps with CORS rules, encryption, and linked resources.
######################################################################################################################################################################

# #--- Azure Maps Configuration ---
# resource "azapi_resource" "azure_maps" {
#   type      = "Microsoft.Maps/accounts@2024-07-01-preview"
#   name      = "string"
#   parent_id = "string"
#   identity {
#     type = "string"
#     identity_ids = [
#       "string"
#     ]
#   }
#   location = "string"
#   tags = {
#     environment = var.environment
#   }
#   body = {
#     kind = "string"
#     properties = {
#       cors = {
#         corsRules = [
#           {
#             allowedOrigins = [
#               "string"
#             ]
#           }
#         ]
#       }
#       disableLocalAuth = bool
#       encryption = {
#         customerManagedKeyEncryption = {
#           keyEncryptionKeyIdentity = {
#             delegatedIdentityClientId      = "string"
#             federatedClientId              = "string"
#             identityType                   = "string"
#             userAssignedIdentityResourceId = "string"
#           }
#           keyEncryptionKeyUrl = "string"
#         }
#         infrastructureEncryption = "string"
#       }
#       linkedResources = [
#         {
#           id         = "string"
#           uniqueName = "string"
#         }
#       ]
#       locations = [
#         {
#           locationName = "string"
#         }
#       ]
#     }
#     sku = {
#       name = "string"
#     }
#   }
# }



