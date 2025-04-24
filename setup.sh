# Install Azure CLI
# https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-linux?pivots=apt
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Install Azure Dev CLI
# https://learn.microsoft.com/en-us/azure/developer/dev-cli/install-azd
curl -fsSL https://aka.ms/install-azd.sh | bash

# Install vim
sudo apt install vim

# Install Terraform
# https://learn.microsoft.com/en-us/azure/developer/terraform/install-terraform
sudo apt-get update && sudo apt-get install -y gnupg software-properties-common
wget -O- https://apt.releases.hashicorp.com/gpg | \
gpg --dearmor | \
sudo tee /usr/share/keyrings/hashicorp-archive-keyring.gpg > /dev/null

gpg --no-default-keyring \
--keyring /usr/share/keyrings/hashicorp-archive-keyring.gpg \
--fingerprint

echo "deb [signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] \
https://apt.releases.hashicorp.com $(lsb_release -cs) main" | \
sudo tee /etc/apt/sources.list.d/hashicorp.list

sudo apt update
sudo apt-get install -y terraform

# Install DevTunnel CLI
# https://learn.microsoft.com/en-us/azure/developer/devtunnel/cli-install
curl -sL https://aka.ms/DevTunnelCliInstall | bash

sudo apt update -y && sudo apt upgrade -y
