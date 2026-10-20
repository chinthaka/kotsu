// Skeleton only: Azure resources are added from phase 17 onwards.
targetScope = 'resourceGroup'

@description('Azure region for every resource. UK South keeps data in the UK.')
param location string = 'uksouth'

output location string = location
