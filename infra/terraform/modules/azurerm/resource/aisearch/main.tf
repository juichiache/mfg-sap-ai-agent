variable "aisearch_name" {
  description = "The name of the Azure AI Search service"
  type        = string
}
variable "aisearch_sku_name" {
  description = "The SKU name for the Azure AI Search service"
  type        = string
  default     = "standard3"
}
variable "resource_group_name" {
  description = "The name of the resource group where the Azure AI Search service will be created"
  type        = string
}
variable "location" {
  description = "The Azure region where the Azure AI Search service will be created"
  type        = string
}
variable "tags" {
  description = "A map of tags to assign to the Azure AI Search service"
  type        = map(string)
  default     = {}
}