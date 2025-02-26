terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.14.0"
    }
    helm = {
      source  = "hashicorp/helm"
      version = "2.4.1" # Helm provider version
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

resource "azurerm_container_registry" "acr" {
  name                = "blogappacr"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "Standard"
  admin_enabled       = true
}

resource "azurerm_role_assignment" "acr_pull" {
  principal_id         = azurerm_kubernetes_cluster.aks.kubelet_identity[0].object_id
  role_definition_name = "AcrPull"
  scope                = azurerm_container_registry.acr.id
}

provider "kubernetes" {
  config_path = "C:/Users/karthikeya/.kube/config"
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
    
    upgrade_settings {
      max_surge                     = "33%"
      drain_timeout_in_minutes      = 30
      node_soak_duration_in_minutes = 0
    }
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

  lifecycle {
    ignore_changes = [
      # upgrade_settings,
    ]
  }
}

# resource "azurerm_kubernetes_cluster_node_pool" "spot_pool" {
#   name                  = "spotpool"
#   kubernetes_cluster_id = azurerm_kubernetes_cluster.aks.id
#   vm_size               = "Standard_B2s"
#   node_count            = 1
#   priority              = "Spot"
# }

resource "azurerm_kubernetes_cluster_node_pool" "linux_pool" {
  name                  = "linuxpool"
  kubernetes_cluster_id = azurerm_kubernetes_cluster.aks.id
  vm_size               = "Standard_B2s"
  node_count            = 1
  os_type               = "Linux"
  mode                  = "User"
  vnet_subnet_id        = azurerm_subnet.subnet.id

  lifecycle {
    ignore_changes = [
      // List the attributes to ignore changes
      node_count,
      // Add other attributes if necessary
    ]
  }
}

# Adding Helm Configuration
# provider "helm" {
#   kubernetes {
#     # config_path = "~/.kube/config"
#     config_path = "C:/Users/karthikeya/.kube/config"
#   }
# }

# provider "kubernetes" {
#   alias                  = "alias_name"
#   host                   = azurerm_kubernetes_cluster.aks.kube_config.0.host
#   client_certificate     = base64decode(azurerm_kubernetes_cluster.aks.kube_config.0.client_certificate)
#   client_key             = base64decode(azurerm_kubernetes_cluster.aks.kube_config.0.client_key)
#   cluster_ca_certificate = base64decode(azurerm_kubernetes_cluster.aks.kube_config.0.cluster_ca_certificate)
# }

provider "helm" {
  kubernetes {
    host                   = azurerm_kubernetes_cluster.aks.kube_config.0.host
    client_certificate     = base64decode(azurerm_kubernetes_cluster.aks.kube_config.0.client_certificate)
    client_key             = base64decode(azurerm_kubernetes_cluster.aks.kube_config.0.client_key)
    cluster_ca_certificate = base64decode(azurerm_kubernetes_cluster.aks.kube_config.0.cluster_ca_certificate)
  }
}

# data "azurerm_container_registry" "acr" {
#   name                = azurerm_container_registry.acr.name
#   resource_group_name = azurerm_resource_group.rg.name
# }

# data "azurerm_container_registry_token" "acr_token" {
#   name                  = "acr-token"
#   container_registry_name = azurerm_container_registry.acr.name
#   resource_group_name   = azurerm_resource_group.rg.name
# }

# output "acr_username" {
#   value = data.azurerm_container_registry_token.acr_token.username
# }

# output "acr_password" {
#   value = data.azurerm_container_registry_token.acr_token.password
# }

resource "helm_release" "blog_app_api" {
  # provider  = kubernetes.alias_name
  name      = "blog-app-api"
  chart      = "../Helm/blog-app-api"
  # version    = "0.1.0"  # Specify the version of the Helm chart
  namespace  = "default"
  repository = "https://${var.acr_name}.azurecr.io"
  repository_username = var.acr_user_name
  repository_password = var.acr_user_password
  # repository = "https://${data.azurerm_container_registry.acr.login_server}/helm/v1/repo"
  # repository_username = data.azurerm_container_registry_token.acr_token.username
  # repository_password = data.azurerm_container_registry_token.acr_token.password
  # repository  = "https://${var.acr_name}.azurecr.io/helm/v1/repo"
  # repository_username = var.acr_user_name
  # repository_password = var.acr_user_password

  set {
    name  = "image.repository"
    value = "blogappacr.azurecr.io/blog-app-api"
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
