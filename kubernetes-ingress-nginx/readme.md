# Installing an Ingress Controller

The Ingress Controller is the the actual entrypoint to the services inside your cluster.
The Controller is like a service that set up and configure a load balancer, along with routing etc.
In this example we're using *kubernetes* `ingress-nginx` solution, **NOT** the company NginX's own Ingress Controller. Big difference!

* Get the latest kubectl! `choco install kube-cli` or `choco upgrade kubernetes-cli`
* Execute the steps in: https://kubernetes.github.io/ingress-nginx/deploy/#prerequisite-generic-deployment-command
* Follow the steps for YOUR cloud provider
* For example, I use Win10 so i needed to get the current account I was using, and the prereq command didnt work on Powershell.
  * So I executed `gcloud config get-value account` to get the account
  * And then `kubectl create clusterrolebinding cluster-admin-binding --clusterrole=cluster-admin --user rickard@kumobits.com`
  * Then the mandatory command: `kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/nginx-0.30.0/deploy/static/mandatory.yaml`
  * Then for GKE: `kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/nginx-0.30.0/deploy/static/provider/cloud-generic.yaml`
* Verify with `kubectl get pods --all-namespaces -l app.kubernetes.io/name=ingress-nginx --watch`
* Here for more docs: https://kubernetes.github.io/ingress-nginx/
* Profit!

## On minikube

* Run `minikube addons enable ingress` (from https://kubernetes.io/docs/tasks/access-application-cluster/ingress-minikube/#enable-the-ingress-controller)
* Browse to http://192.168.1.180  and verify you can see "default backend"

## On your cloud - using Helm

On your cloud provider!
* Go here: https://kubernetes.github.io/ingress-nginx/deploy/#using-helm and follow that guide

## Verify access to the cluster using the ingress controller

* Open your k8s Dashboard with whatever command is needed for your provider (kubectl browse, az aks browse)
* Go to Services, observe the External endpoints for the ingress controller
* Click it and make sure you get a response in your browser like so: 'default backend - 404'

## Configure http routing to the webclient front

* Run `kubectl apply -f ..\kubernetes\client-cluster-ip-service.yaml` to instead a clusterip to the running pods if you haven't done that (previous part of the guide)
* Run `kubectl apply -f .\ingress-service.yaml` to apply the ingress service that routes traffic
* Verify with opening the dashboard, go to services, access the external endpoint for HTTP, OR use `minikube ip` to get the IP
* Browse to http://your ip and you should see your site!
* Profit!

## Making Ingress work cross-namespaces in kubernetes

Seems unnecessary and expensive to use separate ingress-services for each namespace, since that creates a LB for each as well!
Read: https://github.com/ProProgrammer/cross-namespace-nginx-ingress-kubernetes
Haven't tested this myself though.
