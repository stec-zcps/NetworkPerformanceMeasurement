# Gateway

## Concept
The gateway concept is depicted in the figure below. The packets between client and server are routet via a client-side
and server-side gateway. Depending on the destination port, iptables routes the packet over the appropriate network 
interface. The Kunbus TAP CURIOUS captures all packets and adds 20 bytes with additional information (e.g. timestamp)
to each packet. the client can capture these packets on the `Out` port of the Kunbus TAP CURIOUS. The switch is required
because the Kunbus TAP CURIOUS is sometimes not able to capture packets on a direct connection. This may be different 
for your devices. The depicted concept can be automatically setup up using the provided 
[Ansible Playbook](/ansible/playbook.yml).
![Gateway Concept](images/gateway_concept.svg)

## Setup
### Configure Ansible
Configuration of gateways can be done via SSH and Ansible. In the [Ansible](Ansible) sub-directory of this repository 
configure your hosts and routing in `inventory.yml` and create a `vault.yml` under `Ansible/group_vars/all` using the 
command `ansible-vault create vault.yml` in this directory. For the provided inventory file the vault file could look
like this:
```yml
vault_host_client_ip: 172.21.5.192
vault_host_client_user: YourUsername
vault_host_client_password: YourPassword

vault_host_server_ip: 172.21.5.184
vault_host_server_user: YourUsername
vault_host_server_password: YourPassword

vault_host_gateway_client_ip: 172.21.5.186
vault_host_gateway_client_user: YourUsername
vault_host_gateway_client_password: YourPassword

vault_host_gateway_server_ip: 172.21.5.193
vault_host_gateway_server_user: YourUsername
vault_host_gateway_server_password: YourPassword
```
Your credentials are stored encrypted in the vault file. The vault file can be edited by using 
`ansible-vault edit /path/to/vault/file`.

### Execute
Finally, execute the Ansible Playbook with the following command:
```ansible
ansible-playbook playbook.yml -i inventory.yml --ask-vault-pass
```

If you are using Windows, Ansible can be executed via Windows Subsystem for Linux (WSL).
