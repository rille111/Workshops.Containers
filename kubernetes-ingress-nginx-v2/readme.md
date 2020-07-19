# Preparation

Make yourself admin for the cluster. So for GKE this is:

* Get your login by: `gcloud config get-value account`
* Make yourself admin: `kubectl create clusterrolebinding cluster-admin-binding  --clusterrole cluster-admin --user`

Optional: Find out your static IP. Note the IP and the Name of this IP Resource (different in GKE, AKS, EKC)

# Delete current ingress

Delete ingress controller, all ingress resources, if the Ingress was installed by Helm, delete it too.
If it´s Nginx ingress, you can first try the below:

* `kubectl delete -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v0.34.1/deploy/static/provider/cloud/deploy.yaml`
* Open your cloud provider and delete remains

# Install/Upgrade via Helm

See https://kubernetes.github.io/ingress-nginx/deploy/#gce-gke

First we´re gonna create a namespace, update the helm repo, install nginx ingress controller, prepare for SSL 
then create some ingresses to a couple of services and give them LetsEncrypt SSL certs while doing it.
Because we dont wanna get on LetsEncrypt shitlist, we´re gonna use their staging environment and our k8s services using that.
When it works, switch to production. But first, you need to install LetsEncrypt staging fake certs so you can actually test the sites.

* `kubectl create namespace ingress-nginx`
* `helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx`
* `helm repo update`

**IMPORTANT**! If we want to use our static IP, Attach the --set options below, otherwise skip it.

* `helm upgrade -n ingress-nginx --install nginx-ingress ingress-nginx/ingress-nginx --set controller.publishService.enabled=true,controller.service.loadBalancerIP=111.111.11.11`
* Observe the ouput for an example how to create some ingresses.
* Also observe your ingress and its external IP with `kubectl get services -o wide -w -n ingress-nginx`

## Preparing for SSL/TLS via HTTPS through LetsEncrypt

First, you need to add two LetsEncrypt staging certificates to your cert store so you can access the websites.
These two, put them in Root and Intermediary respectively, for Computer store: https://letsencrypt.org/docs/staging-environment/

Now, we need to install Jetstacks cert-manager (certificate management controller). Follow:
https://cert-manager.io/docs/installation/kubernetes/#installing-with-helm
Test it with apply,observe and delete  `test-resources.yaml`

Then, tell jetstack how to issue certificates, for LetsEncrypt staging and prod by applying these issuers:

* `kubectl apply -f staging-issuer.yaml`
* `kubectl apply -f production-issuer.yaml` 

## Create some Ingress resources (routes into your webs, services, etc)

* Optional, for Static IPs: **POSSIBLY IMPORTANT**! Edit the yamls for the static ip name that you bought in your cloud provider. Write name, not IP.
* And then run `kubectl apply -f .\staging-ingress-resources.yaml`

Wait a while for Certificates and then go to the URL´s, OR edit your hosts file if you havent set up DNS to access your sites.