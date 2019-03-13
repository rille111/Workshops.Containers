## Register a domain
Use your preferred domain registrar to register a domain, i'm using 'fodmaps.nu'

## Create a subdomain that points the subdomain to your kubernetes cluster
* Add a subdomain in your registrar, i'm using 'kube' in this example, so the fqdn becomes 'kube.fodmaps.nu'
* Edit DNS settings for that subdomain, change the default registrar nameservers to the ones in your cloud, for me is 'dns04.azure.com', you should at least have 4 entries in case of one goes down. Set TTL to 1 hour under development

## Create a DNZ Zone in your Cloud Provider
This will depend on the k8s/cloud provider.
* For Azure, I created a DNS zone called 'fodmaps.nu'
* Add an A record, point to the IP of the Ingress, it can be found be executing `kubectl get services`, use at the External IP of the Ingress controller

## Verify
After a while, an hour or so, run these commands:
* `nslookup -q=NS kube.fodmaps.nu` (Replace host with your registered host)
    * The response should say that the primary DNS is the one in your cloud provider
* `ping kube.fodmaps.nu` the IP should be the External IP of your k8s ingress controller
* Go to http://kube.fodmaps.nu (replace with your registered subdomain) and see that you can access the website

## Install Cert-manager and make https work
* Go to https://docs.cert-manager.io/en/latest/getting-started/index.html and install Cert-manager with Helm
* Edit certificate.yaml, issuer.yaml and ingress-service.yaml in this folder to match your host
* Apply the same yamls in that order
* Go to http://kube.fodmaps.nu and verify that it works
* Profit!