param location string = resourceGroup().location
@minLength(3)
param webSiteName string
param sku string = 'F1'

resource appServicePlan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: '${webSiteName}Plan'
  location: location
  kind: 'app'
  sku: {
    name: sku
  }
  properties: {
    reserved: false
  }
}

resource appService 'Microsoft.Web/sites@2024-11-01' = {
  name: webSiteName
  location: location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: true
    siteConfig: {
      alwaysOn: false
      http20Enabled: true
      minTlsVersion: '1.2'
      webSocketsEnabled: true
      use32BitWorkerProcess: true
      netFrameworkVersion: 'v9.0'
    }
  }
}
