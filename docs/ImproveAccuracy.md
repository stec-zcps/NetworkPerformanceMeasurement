# Improve Measurement Accuracy
To improve accuracy of the software-based network performance measurement CPU affinity in combination with CPU core
isolation as well as Linux Kernel Realtime Patch can be used.

### CPU affinity / CPU core isolation
To get higher test result accuracy the external test tool should be executed on dedicated CPU cores. This can be
achieved by setting the ```--cpu-affinity``` option. The option expects a comma-separated list of CPU core id
(e.g. ```--cpu-affinity 2,3``` to use third and fourth CPU core). Ideally  these CPU cores should be isolated from
operating system. Follow steps below to enable CPU core isolation on your system.

Edit the ```GRUB_CMDLINE_LINUX``` option in the ```/etc/default/grub``` file. There you can specify the ids of
CPU cores which should be isolated (**first core has index 0!**). E.g. if you have a quad-core CPU and you want to
isolate the last two CPU cores, set the option like this:
```bash
GRUB_CMDLINE_LINUX="isolcpus=2,3"
```
Then run ```sudo update-grub``` , reboot the system and check if everything worked. Therefore use the command
```ps -eo psr,command | tr -s " " | grep "^ [2|3]"```. The output should look like this:
```bash
2 [cpuhp/2]
2 [watchdog/2]
2 [migration/2]
2 [ksoftirqd/2]
2 [kworker/2:0]
2 [kworker/2:0H]
3 [cpuhp/3]
3 [watchdog/3]
3 [migration/3]
3 [ksoftirqd/3]
3 [kworker/3:0]
3 [kworker/3:0H]
2 [kworker/2:1]
3 [kworker/3:1]
3 [kworker/3:1H]
3 /usr/sbin/pound
```
You should also see no utilization in tools like htop.

## Linux Kernel Realtime Patch (Ubuntu)
To build the realtime patched kernel you need at least 30 GB free disk space. Check 
[https://wiki.linuxfoundation.org/realtime/start](https://wiki.linuxfoundation.org/realtime/start) for the latest stable
 version of the realtime patch, at the time of writing this is “Latest Stable Version 5.10-rt”. If we click on the link, 
we get the exact version number. Currently, it is patch-5.10.56-rt48.patch.gz.

We can now start by creating a directory in our home dir and entering it
```
mkdir ~/kernel
cd ~/kernel
```
Download and unpack the latest stable realtime kernel patch
```
wget http://cdn.kernel.org/pub/linux/kernel/projects/rt/5.10/patch-5.10.56-rt48.patch.gz
gunzip patch-5.10.56-rt48.patch.gz
```
Find the matching linux Kernel at https://mirrors.edge.kernel.org/pub/linux/kernel/v5.x/, download and unpack it aswell.
```
wget https://mirrors.edge.kernel.org/pub/linux/kernel/v5.x/linux-5.10.56.tar.gz
tar -xzf linux-5.10.56.tar.gz
```
Enter the unpacked linux kernel dir
```
cd linux-5.10.56
```
and patch it with the rt patch
```
patch -p1 < ../patch-5.10.56-rt48.patch
```
Copy the .config of the currently running Ubuntu
```
cp /boot/config-5.8.0-63-generic .config
```
Open Software & Updates. in the Ubuntu Software menu tick the ‘Source code’ box
We need some tools to build kernel, install them with
```
sudo apt-get build-dep linux
sudo apt-get install libncurses-dev flex bison openssl libssl-dev dkms libelf-dev libudev-dev libpci-dev libiberty-dev autoconf fakeroot
```
To enable all Ubuntu configurations, we simply use
```
yes '' | make oldconfig
```
Then we need to enable rt_preempt in the kernel building config. We call
```
make menuconfig
```
and set the following
```
# Enable CONFIG_PREEMPT_RT
 -> General Setup
  -> Preemption Model (Fully Preemptible Kernel (Real-Time))
   (X) Fully Preemptible Kernel (Real-Time)

# Enable CONFIG_HIGH_RES_TIMERS
 -> General setup
  -> Timers subsystem
   [*] High Resolution Timer Support

# Enable CONFIG_NO_HZ_FULL
 -> General setup
  -> Timers subsystem
   -> Timer tick handling (Full dynticks system (tickless))
    (X) Full dynticks system (tickless)

# Set CONFIG_HZ_1000 (note: this is no longer in the General Setup menu, go back twice)
 -> Processor type and features
  -> Timer frequency (1000 HZ)
   (X) 1000 HZ

# Set CPU_FREQ_DEFAULT_GOV_PERFORMANCE [=y]
 ->  Power management and ACPI options
  -> CPU Frequency scaling
   -> CPU Frequency scaling (CPU_FREQ [=y])
    -> Default CPUFreq governor (<choice> [=y])
     (X) performance
```
Save and exit menuconfig.

Finally, before building, we have to change the line
```
CONFIG_SYSTEM_TRUSTED_KEYS="debian/canonical-certs.pem" 
```
in the .config file to
```
CONFIG_SYSTEM_TRUSTED_KEYS="" 
```
Now we’re going to build the kernel which will take quite some time.
```
make -j `nproc` deb-pkg
```
If the build finished without errors, we can install the new rt kernel using
```
sudo dpkg -i ../*.deb
```
Finally, you can reboot the system and check the new kernel version.
```
sudo reboot
uname -a
```
The output should look like this:
```
Linux ipa-test-client 5.10.52-rt47 #1 SMP PREEMPT_RT Wed Aug 4 23:21:00 CEST 2021 x86_64 x86_64 x86_64 GNU/Linux
```
