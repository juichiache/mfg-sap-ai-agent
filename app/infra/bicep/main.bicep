param name string
param location string = resourceGroup().location
param containers array

@secure()
param secrets object = {
  arrayValue: []
}
param registries array = []
param registryName string = ''
param registryResourceGroup string = resourceGroup().name
param registrySubscriptionId string = subscription().subscriptionId
param ingress object = {
  external: true
  transport: 'Auto'
  allowInsecure: false
  targetPort: 8080
  stickySessions: {
    affinity: 'none'
  }
  additionalPortMappings: []
}
param environmentName string
param workspaceName string
param workspaceLocation string = resourceGroup().location

resource workspace 'Microsoft.OperationalInsights/workspaces@2020-08-01' = {
  name: workspaceName
  location: workspaceLocation
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    workspaceCapping: {}
  }
}

resource managedEnv 'Microsoft.App/managedEnvironments@2024-08-02-preview' = {
  name: environmentName
  location: location
  dependsOn: [
    workspace
  ]
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: reference(workspace.id, '2020-08-01').customerId
        sharedKey: listKeys(workspace.id, '2020-08-01').primarySharedKey
      }
    }
    publicNetworkAccess: 'Enabled'
    workloadProfiles: [
      {
        name: 'Consumption'
        workloadProfileType: 'Consumption'
      }
    ]
  }
}

// Reference to the existing container registry
resource acr 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' existing = if (!empty(registryName)) {
  name: registryName
  scope: resourceGroup(registrySubscriptionId, registryResourceGroup)
}

// Dynamically get registry credentials if registry name is provided
var acrCredentials = !empty(registryName) ? listCredentials(acr.id, acr.apiVersion) : null
var dynamicRegistries = !empty(registryName) ? [
  {
    server: '${registryName}.azurecr.io'
    username: acrCredentials.username
    passwordSecretRef: 'acr-password'
  }
] : []

// Dynamically create secrets array for ACR
var acrSecrets = !empty(registryName) ? [
  {
    name: 'acr-password'
    value: acrCredentials.passwords[0].value
  }
] : []

// Combine provided secrets with dynamically generated secrets
var combinedSecrets = concat(secrets.arrayValue, acrSecrets)
var combinedRegistries = empty(registries) ? dynamicRegistries : registries

resource containerApp 'Microsoft.App/containerapps@2024-08-02-preview' = {
  name: name
  location: location
  kind: 'containerapps'
  dependsOn: [
    managedEnv
  ]
  properties: {
    environmentId: managedEnv.id
    configuration: {
      secrets: combinedSecrets
      registries: combinedRegistries
      activeRevisionsMode: 'Single'
      ingress: ingress
    }
    template: {
      containers: containers
      scale: {
        minReplicas: 0
      }
    }
    workloadProfileName: 'Consumption'
  }
}
