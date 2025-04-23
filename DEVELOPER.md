# devcontainers

The developer container has the following applications that can be installed in your container.
- terraform
- azure CLI
- azure developer CLI
- vim

The setup.sh script has all the installs if your container is missing the applications.  

## How to use setup.sh

1. at the command prompt type `chmod +x ./setup.sh` to make this script executable
2. at the command prompt type `./setup.sh` to run the script.
3. answer any prompts that come up

## Create an alias for terraform (optional)

update the .bashrc file located at `~/` (home folder)  

add the following to the file
```
alias tf='terraform'
```

at the command prompt you can type `source ~/.bashrc` to force a rebuild of the terminal using the new settings.


## Validate installation of setup.sh  

Azure CLI
```
az version
```
output  
```plaintext
{
  "azure-cli": "2.71.0",
  "azure-cli-core": "2.71.0",
  "azure-cli-telemetry": "1.1.0",
  "extensions": {
    "account": "0.2.5"
  }
}
```

Azure Developer CLI
```
azd version
```
output
```plaintext
azd version 1.14.0 (commit c928795c47f27d1e997c217147dc649054ac05c8)
```

terraform
```
tf --version
```
output
```plaintext
Terraform v1.11.4
on linux_amd64
```
output
```plaintext
VIM - Vi IMproved 9.1 (2024 Jan 02, compiled Apr 01 2025 20:12:31)
Included patches: 1-16, 647, 678, 697
Modified by team+vim@tracker.debian.org
Compiled by team+vim@tracker.debian.org
....



