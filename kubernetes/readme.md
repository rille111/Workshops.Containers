# Installation

This part describes installation for Kubernetes locally (Minikube) and on a Cloud provider (AKS, GKE, EKS)

## Requirements

* Subscription for a cloud provider (optional, if you only wanna do minikube that's ok)
* Windows 10 Professional
* Virtualization=Enabled in your BIOS
* Hyper-V installed
* Choco (go to chocolatey.org and download it)

## Install Kubernetes (Minikube) locally on Windows 10

* First, Create a network switch in hyper-v. (Ref: https://medium.com/@JockDaRock/minikube-on-windows-10-with-hyper-v-6ef0f4dc158c)
    * name it `minikube_network_switch`
* Then we're gonna follow these steps: (Ref: https://kubernetes.io/docs/tasks/tools/install-minikube/#windows)
* CD to the C:\ drive **IMPORTANT** It might have to to with where your Users-folder is located
* run `choco install minikube kubernetes-cli` (upgrade if you already have it)
* IMPORTANT!! restart shell/commandprompt - not only refreshenv!!!
* CD to the C:\ drive
* run `minikube start --vm-driver hyperv --hyperv-virtual-switch "minikube_network_switch"` Going to take some minutes, dont cancel the process!
* It should work, otherwise fix all problems.
* Then, fix a bug (mentioned in  https://medium.com/@JockDaRock/minikube-on-windows-10-with-hyper-v-6ef0f4dc158c)
    * **Turn off** minikube image in hyper-v
    * Right click - Settings - Memory
    * Turn off dynamic and give it a solid 2gb or something
    * Restart it
* Run `minikube status` to see that it works
* Other fun commands:
    * `minikube ip`
    * `minikube dashboard`

## Problems?
* If minikube can't find the iso, try:
    * You probably didnt do this from C: where your User-folder exists by default
    * Specify the path for the .minikube directory by setting the MINIKUBE_HOME env variable
        * Example powershell `$env:MINIKUBE_HOME = "D:/"` (see https://github.com/kubernetes/minikube/issues/1310)
        * Or do it in windows and reboot
    * Use cmd not powershell and go to c:, then run it again
* Still doesn't work? Delete all:
    * Try `minikube delete`
    * Delete everything from:
        * %User%\.kube
        * %User%\.minikube
    * Open Hyper-V and delete the minikube image
    * Go through the above installation steps again
    * Or read and do this: https://github.com/kubernetes/minikube/issues/822

More info:
* You can connect to the VM and login with user: **docker**   password: **tcuser**
* Shutdown the image that is running in Hyper-V: `sudo shutdown -r now -f`
* Shut down the cluster from a shell (Not from within the Hyper-V image!!) with `minikube stop`

# Startup

## a) Kubernetes (minikube, locally)
Apply the headless service (cluster-ip), and then the deployment with 1 pod containing the web client
* run `kubectl apply -f client-cluster-ip-service.yaml`
* run `kubectl apply -f client-deployment.yaml`
* run `kubectl get pods` and verify there is one there
* run `kubectl get services` and verify there is one clusterip there
* run `.\expose-client-pod.ps1`
* go to `http://127.0.0.1:8089` and verify that you can access the web site

## b) Kubernetes (on a cloud provider, AKS, GKE, EKS)

* Create cluster using the cloud providers portal
* Configure the CLI tool from the cloud provider (azure cli, google cloud SDK etc)
* Configure the kubectl to use this cluster and not your local Minikube
* Then, run the steps found in a)
* NOTE!: To get going in the real cloud, see the next part of this set of tutorials!

## Using private container repository
See: https://kubernetes.io/docs/concepts/containers/images/#using-a-private-registry

## Links and resources
* Minikube https://github.com/kubernetes/minikube

## FAQ

* Where is Kubectl configuration kept? c:\%USER%\.kube\config
* How can i make Minikube or a cloud provided Kubernetes cluster fetch containers from another Docker repository?
    * Using a local machine registry with minikube: https://blog.hasura.io/sharing-a-local-registry-for-minikube-37c7240d0615/ CTRL-F for "final workflow"
    * Using a cloud container registry: https://container-solutions.com/using-google-container-registry-with-kubernetes/