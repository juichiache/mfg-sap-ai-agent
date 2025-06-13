variable "container_registry_name" {
  type        = string
  default     = "sap-agent-container-registry"
  description = "The name of the Azure Container Registry resource."
}
variable "container_registry_sku_name" {
  type        = string
  default     = "Standard"
  description = "The SKU for the Azure Container Registry resource."
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
  description = "The name of the environment for the Azure Container Registry resource."
}