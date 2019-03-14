# Get a domain and Setup HTTPS with a certificate (LetsEncrypt)

We're going to set up a TLS certificate provided by LetsEncrypt, issued to a chose sub domain.
A brief walkthrough of the steps required follows below, from registering a domain, editing the DNS settings and making Cert-manager do the LetsEncrypt magic automatically, using ACME.

## Decide upon domain and subdomain
* I choose fodmaps.nu as top tomain because I already own it, and it's registered in the domain registrar 'Binero' a swedish one for buying domains
* I choose kube, resulting in kube.fodmaps.nu as subdomain

## Create a DNZ Zone in your Cloud Provider
This will depend on the k8s/cloud provider. We need this setup first so that we have nameservers that can respond to requests.

* I created a DNS zone called 'kube.fodmaps.nu' (
* Add a * wildcard A record, point to the IP of the Ingress, it can be found be executing `kubectl get services`, use at the External IP of the Ingress controller, TTL an hour
    * Like so:  * A 10.10.10.10
* Add a @ wildcard A record, point to the same adress, TTL an hour
    * Like so:  @ A 10.10.10.10
* Note the name servers that is listed in this DNS Zone, there should be a list of 4-5, example: `ns1-08.azure-dns.com.`

#### Problems getting a production cert?
I had problems getting a production cert for a while so I also created a DNS zone 'fodmaps.nu', but I don't think it's necessary, but what I did was this:
* Add a * wildcard A record, point to the IP of the Ingress, TTL an hour
    * Like so:  * A 10.10.10.10
* Add a @ wildcard A record, point to the same adress, TTL an hour
    * Like so:  @ A 10.10.10.10

## Register and set up a domain
Use your preferred domain registrar to register a domain, i'm using `fodmaps.nu`

#### Register and set up a subdomain

To add a subdomain in your registrar, use the one for the previous step, so I use `kube` (you may have something else). The FQDN becomes `kube.fodmaps.nu`.

Follow these steps:

* Edit DNS settings for your domain (`fodmaps.nu` for me) and add an NS record pointing the DNS Zone in your cloud provider
    * Subdomain: `kube` (replace with your choice)
    * Type: `NS`
    * Data: `ns1-08.azure-dns.com` (remember the note from above)
    * TTL: `3600` (an hour)
* And another
    * Subdomain: `kube` (replace with your choice)
    * Type: `CAA`
    * Data: `0 issue "letsencrypt.org"` (tells your registrar to allow letsencrypt to create certificates for this sub domain)
    * TTL: `3600` (an hour)
* And another
    * Subdomain: `@`
    * Type: `CAA`
    * Data: `0 issue "letsencrypt.org"` (tells your registrar to allow letsencrypt to create certificates for this top domain)
    * TTL: `3600` (an hour)

#### Verify domain

After a while, run these commands:
* `nslookup -q=NS kube.fodmaps.nu` (Replace with your FQDN)
    * The response should list all the name servers from your cloud provided DNS Zone, only then can you continue!
* `nslookup -q=NS fodmaps.nu` (Replace with your registered domain)
    * The response should list all the name servers from your domain registrar, only then can you continue!
* `ping kube.fodmaps.nu` the IP should be the External IP of your k8s ingress controller
* Go to http://kube.fodmaps.nu (replace with your registered subdomain) and see that you can access the website, only then can you continue!

## Install and set up Cert-manager for kubernetes

* We're gonna use https://github.com/jetstack/cert-manager
* Go to https://docs.cert-manager.io/en/latest/getting-started/install.html#installing-with-helm and install it
* Verify by running `kubectl get pods --namespace cert-manager`

#### Get LetsEncrypt SSL Cert and make HTTPS work

* First Test out certmgr with 
    * `kubectl apply -f .\test-resources.yaml`
    * `kubectl describe certificate -n cert-manager-test`
    * Make sure stuff works, then cleanup with:
    * `kubectl delete -f test-resources.yaml`

After that, you can either follow the staging instructions here: 
    * https://github.com/jetstack/cert-manager/blob/master/docs/tutorials/acme/quick-start/index.rst 
    * and follow the instructions there to get a LetsEncrypt certificate, it's better than me writing the same guide here. 
    * Again, first use the staging stuff first (use prepared yamls here)!
    * We're going to use ingress-shim (use 'annotations' in `staging/production-ingress-service.yaml` instead of creating our own with `certificate.yaml` even though the yaml is there as an option.)
Or, if you're lazy or trust stuff works automagically, ignore that guide, edit and execute these yamls:
    * edit yaml first to use your settings, then `kubectl apply -f staging-issuer.yaml` 
    * edit first, then `kubectl apply -f staging-ingress-service.yaml` (overwrites your previously deployed ingress)
    * Run `kubectl describe certificates` and it should say that certificate is valid, or that it's up to date, otherwise wait, or go to Troubleshooting below
    * Go to https://kube.fodmaps.nu (replace with your host) and verify that it works, WITH HTTPS warnings (that's ok)
Now, use the production variant (validation may take time, with some retries!)
    * edit yaml first to use your settings, then `kubectl apply -f production-issuer.yaml` 
    * then `kubectl delete ingress ingress-service` 
    * edit first, then `kubectl apply -f production-ingress-service.yaml` (overwrites your previously deployed ingress)
    * Go to https://kube.fodmaps.nu (replace with your host) and verify that it works without any HTTPS warning
* Profit!

## Troubleshooting

* Get the IP to your cluster, your ingress-controller by running: `kubectl get services` and look for External IP
    * Run `ping kube.fodmaps.nu` (replace) to see that you get response with the IP that is exactly the IP shown up in your ingress-controller
* Run `ipconfig /flushdns` 
    * and then  `nslookup -q=NS kube.fodmaps.nu` to see that correct nameservers show up (your cloud provider)
* Run `kubectl describe certificates` note the order id, and if the order is in state 'Issuing' or failed, see:
    * Run `kubectl describe order kube-fodmaps-nu-tls-staging-2780172425` use the order id, note the challenge id (https://docs.cert-manager.io/en/latest/reference/orders.html
    * Run `kubectl describe challenge kube-fodmaps-nu-tls-staging-2780172425-0`
        * Look at: Status.Challenges.URL for the order url and it's status
        * Then look at: Status.Challenges.Authz URL and go there 
            * example: https://acme-v02.api.letsencrypt.org/acme/challenge/6QCHQCY8YAUPXH7hcFuAJETk_WhsuT_Re1UfqQnSD6M/13597139880
                * in this page you will see the validationRecord.url, this is the URL letsencrypt will try to access
                * see error.detail for more information
                * make sure the IP corresponds to the ip of the ingress-controller (see it thru `kubectl get services`)
                * go to that URL and make sure you can reach it, if not, then it wont work!
* See log output from a pod with:
    * `kubectl get pods --namespace cert-manager`
    * `kubectl logs cert-manager-6889798676-xrmvr --namespace cert-manager --tail=20`
        
Errors I've got and how I fixed them:

* On a challenge: "CAA Servfail". SERVFAIL seems to be a LetsEncrypt error when it is unable to resolve to your domain correctly.
  You may need to do some additional checking to ensure that your DNS records are resolving to your domain correctly. 
  * Test this by shooting a DIG CA here, paste your host, choose Type: CAA, click Dig and you should see your cloud provider DNS there
  * Also see: https://caddy.community/t/failed-to-get-certificate-error-presenting-token-unexpected-response-code-servfail-with-cloudflare/3305
  * To fix this i did this:
    * deleted the DNZ Zone of 'fodmaps.nu' that I first had, and created a 'kube.fodmaps.nu' DNZ zone instead (see above)
    * also edited fodmaps.nu top domain in my other Registrar and swapped out the default NS to the ones in the Azure DNZ Zone, this hopefully is not needed though!
    * https://sslmate.com/caa/ see here how to edit

* On a challenge: "Accepting challenge authorization failed: acme: authorization for identifier kube.fodmaps.nu"
    * describe certificates says that it will try again in an hour. Theory is that the ingress-controller temporarily was down (or creating) when a challenge came from letsencrypt
    * It seems by going to letsencrypt challenge url (kubectl describe challenge), the error was because of CAA blabla, same as above
    * I may have fixed this by a combination of:
        * going to my domain registrar and setting NS servers to the cloud provider ones (takes a day or two)
        * and in the same registrar, adding a CAA record like this: (not sure how long it takes)
            * `@` `CAA` with data `0 issuewild "letsencrypt.org"`
        * Verify with either
            * `nslookup -q=CAA kubefodmaps.nu` and `nslookup -q=CAA fodmaps.nu` 
            * Or https://www.digwebinterface.com/ for kube.fodmaps.nu and fodmaps.nu both should get you some response.
        * See more here: https://www.digitalocean.com/docs/networking/dns/how-to/caa/
        * And here:https://community.letsencrypt.org/t/caa-setup-for-lets-encrypt/9893