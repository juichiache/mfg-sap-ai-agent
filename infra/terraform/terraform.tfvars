environment = "dev"

#############################################################################################################
# Open AI Configuration
#############################################################################################################

openai_model_deployment = [{
    name = "gpt-4"
    model = {
        name = "gpt-4"
        version = "1106-Preview"
    }
    scale = {
        type = "Standard"
        capacity = 20
    }
    rai_policy_name = "gpt-4-rai-policy"
},
{
    name = "gpt-35-turbo"
    model = {
        name = "gpt-35-turbo"
        version = "1106"
    }
    scale = {
        type = "Standard"
        capacity = 20
    }
    rai_policy_name = "gpt-35-turbo-rai-policy"
}]
