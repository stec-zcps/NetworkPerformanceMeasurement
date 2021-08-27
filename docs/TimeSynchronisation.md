# Time Synchronisation

A good way to synchronize system clocks is via GPS. Some GPS modules output a Pulse Per Second (PPS) which can be used
to achieve highly synchronized system clocks. In the following the time synchronisation via GPS is exemplary shown
for a [ODYSSEY - X86J4125800](https://www.seeedstudio.com/ODYSSEY-X86J4125800-p-4915.html) by using a GPIO and kernel
module. Beside GPS there are other options like Precision Time Protocol (PTP) to synchronize system clocks. However, 
this is not explained here.

## PPS Kernel Module
The kernel module we need to install will attach an interrupt to the gpio pin connected to our pps source (the GPS 
modules PPS output), and relay the falling and rising events to the chrony service via a /dev/pps device.

Clone from:
```
git clone https://github.com/Jobbel/pps-gpio-dtless
```
```
cd ./pps-gpio-dtless
```
Make against currently running realtime kernel.
```
make -C /lib/modules/`uname -r`/build M=$PWD
```
Linux Pin 364 refers to Hardware Pin 11 on the Odyssey x86 as can be checked at this page:
https://wiki.seeedstudio.com/ODYSSEY-X86J4105-GPIO/
Only Pins 364, 390 and 391 can be used as interrupts and therefor as PPS Input Pins.

We can now insert the compiled module into the Kernel using:
```
sudo insmod ./pps-gpio-dtless.ko gpio=364
```
And check with ```dmesg``` if it worked.

Next we can install pps-tools and check if we receive assertion events.
A working GPS receiver with a fix has to be connected to the correct GPIO of the Odyssey for this next step to work. Use the ppsx number that just attached from dmesg in this case it is 2.
```
sudo apt install pps-tools
sudo ppstest /dev/pps2
```
It should look something like
```
trying PPS source "/dev/pps2"
found PPS source "/dev/pps2"
ok, found 1 source(s), now start fetching data...
source 0 - assert 1628343409.999914126, sequence: 158082 - clear  0.000000000, sequence: 0
source 0 - assert 1628343410.999902957, sequence: 158083 - clear  0.000000000, sequence: 0
source 0 - assert 1628343411.999898906, sequence: 158084 - clear  0.000000000, sequence: 0
```
If you get timeouts something is not working yet.

If all of this works you can make the pps-gpio-dtless module automatically load into the kernel while booting. First we need to edit
```
sudo nano /etc/modules
```
and add an entry
```
pps-gpio-dtless
```
next we have to copy our kernel module to
```
sudo cp ./pps-gpio-dtless.ko /lib/modules/`uname -r`/kernel/drivers/pps/clients
```
and run
```
sudo depmod
```
we can now reboot and check with
```
sudo reboot
dmesg
sudo ppstest /dev/pps2
```
whether the module was loaded correctly, and the pps device works.

## Chrony & GPSD
### GPSD
First we need to install gpsd, the service that relays nmea gps data from the serial port to chrony.
```
sudo apt-get install gpsd gpsd-clients
```
Next we have to edit the gpsd config at
```
sudo nano  /etc/default/gpsd
```
and add
```
START_DAEMON="true"
USBAUTO="false"
DEVICES="/dev/ttyS4"
GPSD_OPTIONS="-n"
```
with /dev/ttyS4 being the UART on the 40 Pin Header of the Odyssey x86

With tools such as
```
cgps -s 
```
or
```
gpsmon 
```
you can check if your gps device is connected properly and even if it your reception is good enough for it to have a fix.

### Chrony
Now we can actually install chrony the time keeping service
```
sudo apt-get install chrony
```
Edit the chrony configuration
```
sudo nano /etc/chrony/chrony.conf
```
like this
```
pool ntp.ubuntu.com        iburst maxsources 4
pool 0.ubuntu.pool.ntp.org iburst maxsources 1
pool 1.ubuntu.pool.ntp.org iburst maxsources 1
pool 2.ubuntu.pool.ntp.org iburst maxsources 2

refclock PPS /dev/pps2 lock NMEA noselect
refclock SHM 0 offset 0 delay 0.2 refid NMEA noselect

# This directive specify the location of the file containing ID/key pairs for
# NTP authentication.
keyfile /etc/chrony/chrony.keys

# This directive specify the file into which chronyd will store the rate
# information.
driftfile /var/lib/chrony/chrony.drift

# Uncomment the following line to turn logging on.
#log rawmeasurements tracking measurements statistics refclocks

# Log files location.
logdir /var/log/chrony

# Stop bad estimates upsetting machine clock.
maxupdateskew 100.0

# This directive enables kernel synchronisation (every 11 minutes) of the
# real-time clock. Note that it can’t be used along with the 'rtcfile' directive.
rtcsync

# Step the system clock instead of slewing it if the adjustment is larger than
# one second, but only in the first three clock updates.
makestep 1 3
```
You will now have to adjust the time offset between the PPS from the GPIO and the NMEA message from the serial port for you specific setup.
This is directly quoted from https://chrony.tuxfamily.org/faq.html:

"A pulse-per-second (PPS) reference clock requires a non-PPS time source to determine which second of UTC corresponds to each pulse. If it is another reference clock specified with the  `lock`  option in the  `refclock`  directive, the offset between the two reference clocks must be smaller than 0.2 seconds in order for the PPS reference clock to work. With NMEA reference clocks it is common to have a larger offset. It needs to be corrected with the  `offset`  option.

One approach to find out a good value of the  `offset`  option is to configure the reference clocks with the  `noselect`  option and compare them to an NTP server. For example, if the  `sourcestats`  command showed
```
Name/IP Address            NP  NR  Span  Frequency  Freq Skew  Offset  Std Dev
==============================================================================
PPS0                        0   0     0     +0.000   2000.000     +0ns  4000ms
NMEA                       58  30   231    -96.494     38.406   +504ms  6080us
foo.example.net             7   3   200     -2.991     16.141   -107us   492us
```
the offset of the NMEA source would need to be increased by about 0.504 seconds. It does not have to be very accurate. As long as the offset of the NMEA reference clock stays below 0.2 seconds, the PPS reference clock should be able to determine the seconds corresponding to the pulses and allow the samples to be used for synchronisation."

Our current chrony configuration already has the noselect option and some internet time sources enabled. You can just run
```
chronyc sourcestats
```
and note your offset.
Change your chrony config to the final configuration
```
#pool ntp.ubuntu.com        iburst maxsources 4
#pool 0.ubuntu.pool.ntp.org iburst maxsources 1
#pool 1.ubuntu.pool.ntp.org iburst maxsources 1
#pool 2.ubuntu.pool.ntp.org iburst maxsources 2

refclock PPS /dev/pps2 lock NMEA
refclock SHM 0 offset 0.336 delay 0.2 refid NMEA noselect

# This directive specify the location of the file containing ID/key pairs for
# NTP authentication.
keyfile /etc/chrony/chrony.keys

# This directive specify the file into which chronyd will store the rate
# information.
driftfile /var/lib/chrony/chrony.drift

# Uncomment the following line to turn logging on.
#log rawmeasurements tracking measurements statistics refclocks

# Log files location.
logdir /var/log/chrony

# Stop bad estimates upsetting machine clock.
maxupdateskew 100.0

# This directive enables kernel synchronisation (every 11 minutes) of the
# real-time clock. Note that it can’t be used along with the 'rtcfile' directive.
rtcsync

# Step the system clock instead of slewing it if the adjustment is larger than
# one second, but only in the first three clock updates.
makestep 1 3
```
and fill in your offset for 0.336

If you adjusted your offset correctly and everything is working
```
chronyc sources
```
should look something like
```
210 Number of sources = 2
MS Name/IP address         Stratum Poll Reach LastRx Last sample               
===============================================================================
#* PPS0                          0   4   377    16    +11us[  +28us] +/- 5524ns
#? NMEA                          0   4   377    15    -89ms[  -89ms] +/-  101ms

```
Congratulations, your Odyssey x86 is now time synced over GPS!
