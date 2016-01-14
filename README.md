# EnableGameStream

This small program should patch GeForce Experience to enable GameStream on GT Devices

The idea is based upon the work of others, I used the manual from p1gl3t, as posted on [xda-developers here](http://forum.xda-developers.com/showpost.php?p=58240849&postcount=123)
I really liked the idea of using my laptop (using a NVidia GT 750M) in combination with Moonlight running on an Amazon Fire TV, and I got it working.

One thing I didn't like about the solution, and that is one needs to install Python and also need to do a lot of manual steps.
This is why I wrote this little tool.

What it currently does:
* Find your graphics card ID
* Find the service
* Find the files to patch
* Stop the service
* Patch the files
* Start the service

What this doesn't:
* Enable frame buffer capture. I could by including the NvFBCEnable.exe, but I don't want issues with NVidia. Unless someone has information on how this works... or if this is still needed? you can find the download in the link on [xda-developers here](http://forum.xda-developers.com/showpost.php?p=58240849&postcount=123)
* Patch the HTTP(S) communication between GeForce-Experience and the site, this could be done by installing my application temporarily as a Proxy (using Fiddler). [Here on xda-developers](http://forum.xda-developers.com/showpost.php?p=62867011&postcount=158) is an example on what the proxy needs to do.

Downloads can be found under the [releases](https://github.com/Lakritzator/EnableGameStream/releases) tab of this repository.

Last, an important notice:
**There is a reason that NVidia didn't enable this functionality, as the power of the GT graphics cards is limited you will have issues streaming when a lot of stuff is going on!!!! I tried it with Alan Wake, video sequences were streaming fine to Moonlight but while playing I had about one frame per 3-4 seconds. Local display on the laptop had no lag at all. I had to reduce the details and resolution before it was somewhat playable in Moonlight.**


**DISCLAIMER**

EnableGameStream is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
