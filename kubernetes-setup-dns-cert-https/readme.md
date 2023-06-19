# Get a domain and Setup HTTPS with a certificate (LetsEncrypt)

We're going to set up a TLS certificate provided by LetsEncrypt, issued to a chose sub domain.
A brief walkthrough of the steps required follows below, from registering a domain, editing the DNS settings and making Cert-manager do the LetsEncrypt magic automatically, using ACME (a standard for requesting & issuing).

## Decide upon domain and subdomain

* Using 'kumobits.com' as top tomain because I already own it, and it's registered in the domain registrar 'Gandi.net'.
* Using 'kube', resulting in kube.kumobits.com as subdomain

## Create a DNZ Zone in your Cloud Provider

This will depend on the k8s/cloud provider. We need this setup first so that we have nameservers that can respond to requests.
So, a domain name is required for setting up Ingress endpoints to services running in the cluster. The specified domain name can be a top-level domain (TLD) or a subdomain. In either case you have to manually set up the NS records for the specified TLD or subdomain so as to delegate DNS resolution queries to an the cloud provider DNS zone created.

### For Google Cloud DNS

* Create a DNS zone called 'kube.kumobits.com' (change to your fqdn)
* Add a * wildcard A record, point to the IP of the Ingress, it can be found be executing `kubectl get services`, use at the External IP of the Ingress controller, TTL an hour
    * Like so:  * A 10.10.10.10
    * `*.kube.kumobits.com.	A	300	10.10.10.10`
* Add a @ wildcard A record, point to the same adress, TTL an hour
    * Like so:  @ A 10.10.10.10
    * `@.kube.kumobits.com.	A	300	10.10.10.10`
* Add a regular A record, point to the same adress, TTL an hour
    * Like so:  @ A 10.10.10.10
    * `kube.kumobits.com.	A	300	10.10.10.10`
   
* Note the name servers that is listed in this DNS Zone, there should be a list of 4-5, example: `ns-cloud-e1.googledomains.com.`

### For Azure DNZ Zone

* Create a DNS zone called 'kube.kumobits.com' (change to your fqdn)
* Add a * wildcard A record, point to the IP of the Ingress, it can be found be executing `kubectl get services`, use at the External IP of the Ingress controller, TTL an hour
    * Like so:  * A 10.10.10.10
* Add a @ wildcard A record, point to the same adress, TTL an hour
    * Like so:  @ A 10.10.10.10
* Note the name servers that is listed in this DNS Zone, there should be a list of 4-5, example: `ns1-08.azure-dns.com.`

#### Problems getting a production cert?

I had problems getting a production cert for a while so I also created a DNS zone 'kumobits.com', but I don't think it's necessary, but what I did was this:
* Add a * wildcard A record, point to the IP of the Ingress, TTL an hour
    * Like so:  * A 10.10.10.10
* Add a @ wildcard A record, point to the same adress, TTL an hour
    * Like so:  @ A 10.10.10.10

## Register and set up a domain

Use your preferred domain registrar to register a domain, i'm using `kumobits.com`

#### Configure the topdomain and add a subdomain in your domain registrar

To add a subdomain in your registrar, use the one for the previous step, so I use `kube` (you may have something else). The FQDN becomes `kube.kumobits.com`.

Follow these steps:

* Edit DNS settings for your top domain (`kumobits.com` for me) and add all NS records pointing to the DNS Zone in your cloud provider, for each NS Server noted from above:
    * Subdomain: `kube` (replace with your choice)
    * Type: `NS`
    * Data: `XXX-XX.azure-dns.com` (remember the note from above)
    * TTL: `3600` (an hour)
    * Text record should become: `kube 1800 IN NS XXX-XX.azure-dns.com.`

Then,
* Another, for letsencrypt
    * Subdomain: `kube` (replace with your choice)
    * Type: `CAA`
    * Data: `0 issue "letsencrypt.org"` (tells your registrar to allow letsencrypt to create certificates for this sub domain)
    * TTL: `3600` (an hour)
* And another, also for letsencrypt
    * Subdomain: `@`
    * Type: `CAA`
    * Data: `0 issue "letsencrypt.org"` (tells your registrar to allow letsencrypt to create certificates for this top domain)
    * TTL: `3600` (an hour)

So the data text result for an example would become something like:
```
@ 3600 IN CAA 0 issue "letsencrypt.org"
kube 3600 IN CAA 0 issue "letsencrypt.org"
kube 1800 IN NS ns-cloud-e1.azure-dns.com.com.
kube 1800 IN NS ns-cloud-e2.azure-dns.com.com.
kube 1800 IN NS ns-cloud-e3.azure-dns.com.com.
kube 1800 IN NS ns-cloud-e4.azure-dns.com.com.
```

#### Verify domain

After a while, run these commands:
* `nslookup -q=NS kube.kumobits.com` (Replace with your FQDN)
    * The response should list all the name servers from your cloud provided DNS Zone, only then can you continue!
* `nslookup -q=NS kumobits.com` (Replace with your registered domain)
    * The response should list all the name servers from your domain registrar, only then can you continue!
* `ping kube.kumobits.com` the IP should be the External IP of your k8s ingress controller
* Go to http://kube.kumobits.com (replace with your registered subdomain) and see that you can access the website, only then can you continue!

## Install and set up Cert-manager for kubernetes

* We're gonna use https://github.com/jetstack/cert-manager
* First, update HELM to v3+
* Go to https://cert-manager.io/docs/installation/kubernetes/#installing-with-helm - follow this guide
* At time of writing, you do: `helm install cert-manager jetstack/cert-manager --namespace cert-mgr --version v0.15.1 --set installCRDs=true`
* Verify by running `kubectl get pods --namespace cert-mgr`

#### Get LetsEncrypt SSL Cert and make HTTPS work

* First Test out certmgr with 
    * `kubectl apply -f .\test-resources.yaml`
    * `kubectl describe certificates -n XXX`
    * Make sure stuff works by seeing the `message: Certificate issued successfully` then cleanup with:
    * `kubectl delete -f test-resources.yaml`
* After that, you can either follow the staging instructions here: 
    * https://cert-manager.io/docs/tutorials/acme/ingress/
    * and follow the instructions there to get a LetsEncrypt certificate, it's better than me writing the same guide here. 
    * Again, first use the staging stuff first (use prepared yamls here)!
    * We're going to use ingress-shim (use 'annotations' in `staging/production -ingress-service.yaml` instead of creating our own with `certificate.yaml` even though the yaml is there as an option.)
* Or, if you're lazy or trust stuff works automagically, ignore that guide, edit and execute these yamls:
    * edit yaml first to use your settings, then `kubectl apply -f staging-issuer.yaml` 
    * edit first, then `kubectl apply -f staging-ingresses.yaml` (overwrites your previously deployed ingress)
    * Run `kubectl describe certificates` and after a couple of minutes you should see "Certificate issued successfully", otherwise wait some more, or go to Troubleshooting below
    * Go to https://kube.kumobits.com (replace with your host) and verify that it works, WITH HTTPS warnings (that's ok)
* Now, use the production variant (validation may take time, with some retries!)
    * edit yaml first to use your settings, then `kubectl apply -f production-issuer.yaml` 
    * then `kubectl delete ingress ingress-service` 
    * edit first, then `kubectl apply -f production-ingresses.yaml` (overwrites your previously deployed ingress)
    * Go to https://kube.kumobits.com (replace with your host) and verify that it works without any HTTPS warning
* Profit!

## Troubleshooting

* Test SSL Cert by either: https://www.ssllabs.com/ssltest/analyze.html or:
  * `choco install openssl.light`
  * `refreshenv`
  * `openssl s_client -showcerts -servername my.domain.com -connect my.domain.com:443`

* Get the IP to your cluster, your ingress-controller by running: `kubectl get services` and look for External IP
    * Run `ping kube.kumobits.com` (replace) to see that you get response with the IP that is exactly the IP shown up in your ingress-controller
* Run `ipconfig /flushdns` 
    * and then  `nslookup -q=NS kube.kumobits.com` to see that correct nameservers show up (your cloud provider)
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
    * deleted the DNZ Zone of 'kumobits.com' that I first had, and created a 'kube.kumobits.com' DNZ zone instead (see above)
    * also edited kumobits.com top domain in my other Registrar and swapped out the default NS to the ones in the Azure DNZ Zone, this hopefully is not needed though!
    * https://sslmate.com/caa/ see here how to edit

* On a challenge: "Accepting challenge authorization failed: acme: authorization for identifier kube.kumobits.com"
    * describe certificates says that it will try again in an hour. Theory is that the ingress-controller temporarily was down (or creating) when a challenge came from letsencrypt
    * It seems by going to letsencrypt challenge url (kubectl describe challenge), the error was because of CAA blabla, same as above
    * I may have fixed this by a combination of:
        * going to my domain registrar and setting NS servers to the cloud provider ones (takes a day or two)
        * and in the same registrar, adding a CAA record like this: (not sure how long it takes)
            * `@` `CAA` with data `0 issuewild "letsencrypt.org"`
        * Verify with either
            * `nslookup -q=CAA kubekumobits.com` and `nslookup -q=CAA kumobits.com` 
            * Or https://www.digwebinterface.com/ for kube.kumobits.com and kumobits.com both should get you some response.
        * See more here: https://www.digitalocean.com/docs/networking/dns/how-to/caa/
        * And here:https://community.letsencrypt.org/t/caa-setup-for-lets-encrypt/9893

# Upgrading/Deleting Cert-Manager

This can be a headache since all resources and CRD's **must be deleted** before namespaces can be deleted, otherwise it will get stuck.

* First, delete Issuers, ClusterIssuers by kubectl delete on the yamls
* Then use HELM to uninstall
* Continue by calling kubectl delete on some files to make sure that CRD and ApiGroups gets deleted
   * Example: kubectl delete -f https://github.com/jetstack/cert-manager/releases/download/v0.8.1/cert-manager.yaml
* Delete namespaces and **make sure they're deleted!**
* Now you can go to the install step and make a fresh install
