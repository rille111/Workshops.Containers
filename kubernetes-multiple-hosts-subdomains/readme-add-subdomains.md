# Setting up a new HTTPS subdomain with DNS, LetsEncrypt and Nginx Ingress (k8s)

## DNS Zone in Google Cloud Platform (GCP)

First, find out the IP of your service you wanna route the subdomain to. For example in GKE its the ingress LB external IP, NOT THE k8s CLUSTER IP!!
For us, its `35.228.57.XX` (Kumobits Cluster)
For us, its `35.228.55.YY` (Afténs Cluster)

Then, prep your cloud provider by telling it theres a new DNS Zone.
The steps are similar in Azure and AWS. Lets assume we wanna add coffee.kumobits.com
You can script this using CLI commands, but this guide wont do that yet.

* Go to Network Services - Cloud CDN, Create Zone
* Zone name: `coffee`, DNS Name: `coffee.kumobits.com`, DNSSec `Off`
* We are adding 3 record sets:
	* `*` Type A 5 minutes 35.228.57.XX
	* `@` Type A 5 minutes 35.228.57.XX
	* `(leave empty)` Type A 5 minutes 35.228.57.XX

## DNS Setup (DNS Registrar)

Now, tell your DNS Registrar to use GCP (or Azure or AWS) Nameservers instead.
You will also let 'letsencrypt' know that it's ok to create certs for this subdomain.

* First, note the nameserver addresses of the DNS Zone in your Cloud provider, from the steps above there should be some NS records.
* Login to your registrar and edit DNS for your domain `kumobits.com`
* Edit the below, for each entry in the above NS records. Do not just copy/paste!

coffee 3600 IN CAA 0 issue "letsencrypt.org"
coffee 1800 IN NS ns-cloud-d1.googledomains.com.
coffee 1800 IN NS ns-cloud-d2.googledomains.com.
coffee 1800 IN NS ns-cloud-d3.googledomains.com.
coffee 1800 IN NS ns-cloud-d4.googledomains.com.

* Save and wait for DNS to propagate, 5 mins to 24 hours. Wtf!??
* go to https://toolbox.googleapps.com/apps/dig/#A/
* Paste the complete URL like `coffee.kumobits.com` and make sure `rcode` says NOERROR and that the answer shows your IP address.
* Then go to https://toolbox.googleapps.com/apps/dig/#NS/
* Do the same and ensure that the nameservers point to the Cloud providers name servers.

## TLS Certificate Issuer 

This is probably already configured, but if not, follow this guide:
https://cert-manager.io/docs/installation/kubernetes/
Or my (old) guide: https://github.com/rille111/Samples.Containers/tree/master/kubernetes-setup-dns-cert-https
And test resources so thing seem to work.

## NGINX Ingress Config

Two things are important here, get it right the first time otherwise certs are gonna be screwed up.
`Ingress.yaml`
```
spec:
  tls: # needed for https
    - hosts:
        - www.kumobits.com 		# the original subdomain
        - coffee.kumobits.com	# !!! add the new subdomain here!!!
```

And a path for the new subdomain to what service further down:

```
  rules:
    - host: www.kumobits.com 	# the original subdomain
      http:
        paths:
          - path: /
            backend:
              serviceName: website-ip-service
              servicePort: 80
    - host: coffee.kumobits.com # !!! add the new subdomain here!!!
      http:
        paths:
          - path: /
            backend:
              serviceName: coffee-ip-service
              servicePort: 2368
```

* Apply the above config kubectl apply -f ./Ingress.yaml
* Check the certificiate and ensure all DNS sub domains appear for TLS and Rules, with: `kubectl describe ing name_of_ingress -n some_namespace` 

## Profit

* Go to https://coffee.kumobits.com and see it work, with a valid Certificate!

## Troubleshooting

* Try changing the secret name! Edit issuer.yaml and ingress.yaml and re-deploy.