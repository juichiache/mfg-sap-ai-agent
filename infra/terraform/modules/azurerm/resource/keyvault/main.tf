variable "keyvault_name" {
  type        = string
  default     = "sap-agent-keyvault"
  description = "The name of the Key Vault resource."
}
variable "keyvault_sku_name" {
  type        = string
  default     = "standard"
  description = "The SKU for the Key Vault resource."
}
variable "keyvault_soft_delete_retention_days" {
  type        = number
  default     = 7
  description = "The number of days to retain soft-deleted keys in the Key Vault."
}
variable "keyvault_purge_protection_enabled" {
  type        = bool
  default     = true
  description = "Whether to enable purge protection for the Key Vault."
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
  description = "The name of the environment for the Key Vault resource."
}
variable "tenant_id" {
  type        = string
  description = "The tenant ID for the Azure subscription."
}