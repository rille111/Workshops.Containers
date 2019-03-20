# Installation

Kubernetes locally (Minikube) and on a Cloud provider (AKS, GKE, EKS)

## Requirements

* Subscription for a cloud provider (optional, if you only wanna do minikube that's ok)
* Windows 10 Professional
* Virtualization=Enabled in your BIOS
* Hyper-V installed
* Choco (go to chocolatey.org and download it)

## Install Kubernetes locally on Windows 10

* First, Create a network switch in hyper-v. (Ref: https://medium.com/@JockDaRock/minikube-on-windows-10-with-hyper-v-6ef0f4dc158c)
    * name it `minikube_network_switch`
* Then follow below steps  (Ref: https://kubernetes.io/docs/tasks/tools/install-minikube/#windows)
    * CD to the C:\ drive
    * run `choco install minikube kubernetes-cli`
    * IMPORTANT!! restart shell/commandprompt - not only refreshenv!!!
    * CD to the C:\ drive
    * run `minikube start --vm-driver hyperv --hyperv-virtual-switch "minikube_network_switch"`
    * it should work, otherwise fix all problems. 
* Then, fix a bug (mentioned in  https://medium.com/@JockDaRock/minikube-on-windows-10-with-hyper-v-6ef0f4dc158c)
    * Stop minikube image in hyper-v
    * Right click - Settings - Memory
    * Turn off dynamic and give it a solid 2gb or something
    * Restart it

Problems?
* If minikube can't find the iso, try:
    * Specify the path for the .minikube directory by setting the MINIKUBE_HOME env variable
        * Like so: $env:MINIKUBE_HOME = "D:/" for powershell (see https://github.com/kubernetes/minikube/issues/1310)
        * Or windows and reboot
    * use cmd not powershell and go to c:, then run it again
* Still doesn't work? Reinstall:
    * Uninstall with `choco uninstall minikube`
    * Open Hyper-V and delete the minikube image
    * Delete everything from:
        * %User%\.kube
        * %User%\.minikube
    * Go through the above installation steps again
    * Or read and do this: https://github.com/kubernetes/minikube/issues/822

More info:
* You can connect to the VM and login with user: **docker**   password: **tcuser**
* Shutdown the image that is running in Hyper-V: `sudo shutdown -r now -f`
* Shut down the cluster from a shell (Not from within the Hyper-V image!!) with `minikube stop`

# Startup

## Startup - Kubernetes (minikube, locally)

* go to /kubernetes folder and run `apply-all.bat`
* run `kubectl get pods` and verify there is one there
* run `kubectl get services` and verify there is one port there
* run `minikube ip` to find out the ip
* go to `http://YOURIP:32001` the port is configured in client-node-port.yaml
* run `minikube dashboard`

## Startup - Kubernetes (on a cloud provider, AKS, GKE, EKS)

* Create cluster using the cloud providers portal
* Configure the CLI tool from the cloud provider (azure cli, google cloud SDK etc)
* Configure the kubectl to use this cluster and not your local Minikube
* Go to /kubernetes folder and run `apply-all.bat`
* Run `kubectl get pods` and verify there is one there
* You won't be able to access your site because **NodePort** is only for local dev!!
* NOTE!: To get going in the real cloud, see the next part of this set of tutorials!

## Links and resources

* Minikube https://github.com/kubernetes/minikube

## FAQ

* Where is Kubectl configuration kept? c:\%USER%\.kube\config
* How can i make Minikube or a cloud provided Kubernetes cluster fetch containers from another Docker repository?
    * Using a local machine registry with minikube: https://blog.hasura.io/sharing-a-local-registry-for-minikube-37c7240d0615/ CTRL-F for "final workflow"
    * Using a cloud container registry: https://container-solutions.com/using-google-container-registry-with-kubernetes/