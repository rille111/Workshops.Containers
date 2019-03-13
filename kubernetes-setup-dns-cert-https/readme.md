## Register a domain
Use your preferred domain registrar to register a domain, i'm using 'fodmaps.nu'

## Create a subdomain that points the subdomain to your kubernetes cluster
* Add a subdomain in your registrar, i'm using 'kube' in this example, so the fqdn becomes 'kube.fodmaps.nu'
* Edit DNS settings for that subdomain, change from the default registrar nameservers to the ones in your cloud provider, for me is 'dns08.azure.com', you should at least have 4 entries in case of one goes down. Set TTL to 1 hour under development
* If there are problems generating certificates, try editing your top tomain and set its NS servers to the ones on your cloud provider (probably not needed!)

## Create a DNZ Zone in your Cloud Provider
This will depend on the k8s/cloud provider.
* For Azure, I created a DNS zone called 'kube.fodmaps.nu'
* Add a * wildcard A record, point to the IP of the Ingress, it can be found be executing `kubectl get services`, use at the External IP of the Ingress controller, TTL an hour
    * Like so:  * A 10.10.10.10
* Add a @ wildcard A record, point to the same adress, TTL an hour
    * Like so:  @ A 10.10.10.10

## Verify domain
After a while, run these commands:
* `nslookup -q=NS kube.fodmaps.nu` (Replace host with your registered host)
    * The response should say that the primary DNS is the one in your cloud provider, only then can you continue
* `ping kube.fodmaps.nu` the IP should be the External IP of your k8s ingress controller
* Go to http://kube.fodmaps.nu (replace with your registered subdomain) and see that you can access the website

## Install Cert-manager
* We're gonna use https://github.com/jetstack/cert-manager
* Go to https://docs.cert-manager.io/en/latest/getting-started/install.html#installing-with-helm and install it
* Verify by running `kubectl get pods --namespace cert-manager`

## Get LetsEncrypt SSL Cert and make HTTPS work
* First Test out certmgr with 
    * `kubectl apply -f .\test-resources.yaml`
    * `kubectl describe certificate -n cert-manager-test`
    * `kubectl delete -f test-resources.yaml`
* After that, go to https://github.com/jetstack/cert-manager/blob/master/docs/tutorials/acme/quick-start/index.rst and follow the instructions there to get a LetsEncrypt certificate, it's better than me writing the same guide here
* We're going to use ingress-shim (use annotations in `ingress-service.yaml` instead of creating our own with `certificate.yaml`)
* If you're lazy or trust stuff works automagically, ignore that guide, edit and execute these yamls:
    * edit first, then `kubectl apply -f staging-issuer.yaml` 
    * edit first, then `kubectl apply -f issuer.yaml` 
    * edit first, then `kubectl apply -f ingress-service.yaml` (overwrites your previously deployed ingress)
* Run `kubectl describe certificates` and it should say that certificate is valid, or that it's up to date, otherwise wait, or go to Troubleshooting below
* Go to https://kube.fodmaps.nu (replace with your host) and verify that it works
* Profit!

## Troubleshooting
* Run `ipconfig /flushdns` and then  `nslookup` to see that correct nameservers show up (your cloud provider)
* Run `ip`
* Run `kubectl describe certificates` note the order id
* Run `kubectl describe order kube-fodmaps-nu-tls-staging-2780172425` use the order id, note the challenge id (https://docs.cert-manager.io/en/latest/reference/orders.html
* Run `kubectl describe challenge kube-fodmaps-nu-tls-staging-2780172425-0`
Errors I've got and how I fixed them:
* "CAA Servfail" on a challenge: SERVFAIL seems to be a LetsEncrypt error when it is unable to resolve to your domain correctly.
  You may need to do some additional checking to ensure that your DNS records are resolving to your domain correctly. 
  See: https://caddy.community/t/failed-to-get-certificate-error-presenting-token-unexpected-response-code-servfail-with-cloudflare/3305
  * To fix this
    * i deleted the DNZ Zone of 'fodmaps.nu' that I first had, and created a 'kube.fodmaps.nu' DNZ zone instead (see above)
    * I also edited fodmaps.nu top domain in my other Registrar and swapped out the default NS to the ones in the Azure DNZ Zone, this hopefully is not needed though!