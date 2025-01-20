terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.14.0"
    }
  }

  #   required_version = ">= 1.1.0"
}


provider "azurerm" {
  features {}
  subscription_id = "5c9e6e10-72b2-4f08-9b3a-f35ede173009"
}

resource "azurerm_resource_group" "rg" {
  name     = "blog-app-api-resource-group"
  location = "southeastasia"
}

resource "azurerm_virtual_network" "vnet" {
  name                = "aks-blog-app-api-vnet"
  address_space       = ["10.0.0.0/8"]
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
}

resource "azurerm_subnet" "subnet" {
  name                 = "aks-blog-app-api-subnet"
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = ["10.240.0.0/16"]
}

resource "azurerm_kubernetes_cluster" "aks" {
  name                = "aks-blog-app-api-cluster"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  dns_prefix          = "akscluster"

  default_node_pool {
    name           = "agentpool"
    node_count     = 1
    vm_size        = "Standard_B2s" # Small instance for low cost
    vnet_subnet_id = azurerm_subnet.subnet.id
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin = "azure"
    network_policy = "azure"
    service_cidr   = "10.0.0.0/16"
    dns_service_ip = "10.0.0.10"
    # docker_bridge_cidr = "172.17.0.1/16"
  }

  tags = {
    environment = "development"
  }
}

resource "azurerm_kubernetes_cluster_node_pool" "spot_pool" {
  name                  = "spotpool"
  kubernetes_cluster_id = azurerm_kubernetes_cluster.aks.id
  vm_size               = "Standard_B2s"
  node_count            = 1
  priority              = "Spot"
}

# Adding Helm Configuration

resource "helm_release" "blog_app_api" {
  name      = "blog-app-api"
  chart     = "../Helm/blog-app-api"
  namespace = "default"

  set {
    name  = "image.repository"
    value = "blog-app-api"
  }

  set {
    name  = "image.tag"
    value = "latest"
  }
}

# Adding ingress controller configuration

resource "azurerm_public_ip" "ingress_ip" {
  name                = "aks-ingress-ip"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  allocation_method   = "Static"
  sku                 = "Standard"
}

resource "azurerm_network_interface" "ingress_nic" {
  name                = "aks-ingress-nic"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  ip_configuration {
    name                          = "internal"
    subnet_id                     = azurerm_subnet.subnet.id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = azurerm_public_ip.ingress_ip.id
  }
}

resource "azurerm_network_security_group" "ingress_nsg" {
  name                = "aks-ingress-nsg"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  security_rule {
    name                       = "Allow_HTTP"
    priority                   = 100
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "80"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }
}
