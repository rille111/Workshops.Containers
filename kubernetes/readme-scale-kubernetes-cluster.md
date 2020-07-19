## Maintenance

### Svale the nodepool resources (CPU, Ram)

* To kill a nodepool, or change the resources, you can first add a new nodepool and assign it, using https://console.cloud.google.com
* Then, you should cordon and drain the other nodepools - to get ZERO downtime.
* Follow this link: https://cloud.google.com/blog/products/gcp/kubernetes-best-practices-upgrading-your-clusters-with-zero-downtime

### Upgrades

* Use the console.google.com
* Upgrade the master plane (doesnt bring any downtime at all)
* Either use the above method (nodepool, cordon & drain) for zero downtime, 
* OR, just use https://console.cloud.google.com and upgrade the nodes, however this will bring a couple of minutes downtime but it's easier and faster.