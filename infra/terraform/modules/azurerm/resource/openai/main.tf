variable "openai_name" {
  type        = string
  default     = "sap-agent-openai"
  description = "The name of the OpenAI resource."
}
variable "openai_kind" {
  type        = string
  default     = "OpenAI"
  description = "The kind of the OpenAI resource."
}
variable "resource_group_name" {
  type        = string
  description = "The name of the resource group where the OpenAI resource will be created."
}
variable "location" {
  type        = string
  description = "The Azure region where the OpenAI resource will be created."
}
variable "openai_sku_name" {
  type        = string
  default     = "S0"
  description = "The SKU for the OpenAI resource."
}
variable "tags" {
  type        = map(string)
  default     = {}
  description = "A map of tags to assign to the OpenAI resource."
}
variable "environment" {
  type        = string
  default     = "sap-agent-environment"
  description = "The name of the environment for the OpenAI resource."
}
variable "suffix" {
  type        = string
  default     = ""
  description = "Suffix to append to resource names for uniqueness, typically derived from the environment or a random value."
}