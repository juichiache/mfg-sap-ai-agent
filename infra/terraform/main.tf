terraform {
  backend "azurerm" {
    use_cli              = true
    use_azuread_auth     = true
    resource_group_name  = "templates"
    storage_account_name = "tfstate4codingforge"
    container_name       = "tfstate"
    key                  = "dev.sap-agent.tfstate"
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
  suffix = "-${substr(md5(var.environment), random_integer.random_0_to_10.result, 4)}"
  # suffix = "-${substr(md5(var.environment), 5, 4)}"
}

resource "azurerm_resource_group" "sap_agent_rg" {
  name     = var.resource_group_name
  location = var.location

  tags = merge(var.tags, {
    environment = var.environment
  })
}

# TODO: build out these services
# Azure Functions, Azure Bot, Azure AI Foundry, Azure OpenAI, Azure AI Services, Grounding with Bing, Azure Maps, Container Apps, Azure AI Search

module "appserviceplans" {
  source = "./modules/azurerm/resource/appserviceplans"

  suffix                = local.suffix
  location             = var.location
  resource_group_name  = var.resource_group_name
  appserviceplan       = "sap-agent-appserviceplan"
  appserviceplan_kind  = "Linux"
  appserviceplan_tier  = "PremiumV2"
  appserviceplan_size  = "P2v2"
  tags                 = var.tags
}

module "functions" {
  source = "./modules/azurerm/resource/functions"

  functionappname        = "sap-agent-functionapp"
  appserviceplan_kind    = "Linux"
  location               = azurerm_resource_group.sap_agent_rg.location
  resource_group_name    = azurerm_resource_group.sap_agent_rg.name
  tags                   = var.tags
  suffix                 = local.suffix
  appserviceplan_id      = azurerm_app_service_plan.functions.id
  storage_account_name   = "sapagentstorage${local.suffix}"
  environment            = var.environment
}



resource "azurerm_bot_service_azure_bot" "bot_service" {
  name                = var.bot_name
  resource_group_name = azurerm_resource_group.sap_agent_rg.name
  location            = "global"
  microsoft_app_id    = data.azurerm_client_config.current.client_id
  sku                 = "F0"

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
  source = "./modules/azurerm/resource/ai_foundry"

  location             = azurerm_resource_group.sap_agent_rg.location
  resource_group_name  = azurerm_resource_group.sap_agent_rg.name
  tags                 = var.tags
}

#--- Azure OpenAI Configuration ---
module "openai" {
  source = "./modules/azurerm/resource/openai"

  location             = azurerm_resource_group.sap_agent_rg.location
  resource_group_name  = azurerm_resource_group.sap_agent_rg.name
  tags                 = var.tags
  openai_model_deployment = var.openai_model_deployment
}

#--- OpenAI Model Deployment ---
resource "azurerm_cognitive_deployment" "deployment" {
    for_each = { for key, value in var.openai_model_deployment : deployment.name => deployment }
    name                = each.key
    cognitive_account_id = module.openai.openai_id

    model {
        format = var.kind
        name = each.value.model.name
        version = each.value.model.version
    }
    scale {
        type = each.value.scale.type
        capcity = each.value.scale.capcity
    }
    lifecycle {
        # ignore_changes = [ tags]
    }
}

#--- Container App Configuration ---
module "container_apps" {
  source = "./modules/azurerm/resource/container_apps"

  location             = azurerm_resource_group.sap_agent_rg.location
  resource_group_name  = azurerm_resource_group.sap_agent_rg.name
  tags                 = var.tags
}

#--- Azure AI Services Configuration ---
module "ai_services" {
  source = "./modules/azurerm/resource/ai_services"

  location             = azurerm_resource_group.sap_agent_rg.location
  resource_group_name  = azurerm_resource_group.sap_agent_rg.name
  tags                 = var.tags
}

#--- Azure Maps Configuration ---
resource "azapi_resource" "azure_maps" {
  type = "Microsoft.Maps/accounts@2024-07-01-preview"
  name = "string"
  parent_id = "string"
  identity {
    type = "string"
    identity_ids = [
      "string"
    ]
  }
  location = "string"
  tags = {
    {customized property} = "string"
  }
  body = {
    kind = "string"
    properties = {
      cors = {
        corsRules = [
          {
            allowedOrigins = [
              "string"
            ]
          }
        ]
      }
      disableLocalAuth = bool
      encryption = {
        customerManagedKeyEncryption = {
          keyEncryptionKeyIdentity = {
            delegatedIdentityClientId = "string"
            federatedClientId = "string"
            identityType = "string"
            userAssignedIdentityResourceId = "string"
          }
          keyEncryptionKeyUrl = "string"
        }
        infrastructureEncryption = "string"
      }
      linkedResources = [
        {
          id = "string"
          uniqueName = "string"
        }
      ]
      locations = [
        {
          locationName = "string"
        }
      ]
    }
    sku = {
      name = "string"
    }
  }
}

resource "azapi_resource" "bing_grounding" {
  # Grounding with Bing placeholder
}


resource "azurerm_search_service" "search" {
  # Azure AI Search placeholder
}

