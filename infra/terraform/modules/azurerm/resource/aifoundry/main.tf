variable "aifoundry_name" {
  type        = string
  default     = "sap-agent-aifoundry"
  description = "The name of the AI Foundry resource."
}
variable "aifoundry_storage_account_name" {
  type        = string
  description = "The name of the storage account to be used by AI Foundry."
}
variable "aifoundry_key_vault_id" {
  type        = string
  description = "The ID of the Key Vault to be used by AI Foundry."
}
variable "location" {
  type        = string
  default     = "eastus"
  description = "The Azure region where resources will be created."
}
variable "resource_group_name" {
  type        = string
  default     = "sap-agent-rg"
  description = "The name of the resource group where resources will be created."
}
variable "tags" {
  type        = map(string)
  default     = {}
  description = "A map of tags to assign to the resources."
}
variable "suffix" {
  type        = string
  default     = ""
  description = "Suffix to append to resource names for uniqueness, typically derived from the environment or a random value."
}
variable "environment" {
  type        = string
  default     = "sap-agent-environment"
  description = "The name of the environment for the AI Foundry resource."
}