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
