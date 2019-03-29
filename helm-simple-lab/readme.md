# What's Helm & Tiller

This guide is a shortened quick guide to create our own Helm Chart.
Read more here: https://helm.sh/docs/developing_charts/#charts

# First install Helm & Tiller

# Create a chart out of the EFK Stack

Go to a folder that will be the root of your chart (this folder 'efk-stack' is already prepared so make a new folder/projectname)
* `mkdir efk-stack` and `cd efk-stack` 
* `helm version` to make sure Tiller runs ok your k8s cluster - if you get an error that a tiller-ready pod couln't be found, try killing the pod or google
* `helm create efk-stack` to scaffold the folders and files
* Inspect the files, then copy over everything from folder 'efk-stack' to your folder
* Make changes to Chart.yaml, change the **name**
* `helm lint` to verify chart & templates
* `helm install --debug --dry-run` 
* .....