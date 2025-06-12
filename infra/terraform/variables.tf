variable "environment" {
  description = "The environment for the resources (e.g., dev, staging, prod)"
  type        = string
  default     = "dev"
}

variable "openai_model_deployment" {
    description = "A map of deployment configurations"
    type        = map(object({
        model = object({
            name    = string
            version = string
        })
        scale = object({
            type    = string
            capcity = number
        })
    }))
}
