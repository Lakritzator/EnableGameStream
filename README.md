# EnableGameStream

This small program should patch GeForce Experience to enable GameStream on GT Devices

The idea is based upon the work of others, I used the manual from p1gl3t, as posted here http://forum.xda-developers.com/showpost.php?p=58240849&postcount=123
I really liked the idea of using my laptop (using a NVidia GT 750M) in combination with Moonlight running on an Amazon Fire TV, and I got it working.

One thing I didn't like about the solution, and that is one needs to install Python and also need to do a lot of manual steps.
This is why I wrote this little tool.

The current state is that it is in development and doesn't work yet.
As soon as a functioning tool is available I will place it in the releases of this repository.

Last, an important notice:
**There is a reason that NVidia didn't enable this functionality, as the power of the GT graphics cards is limited you will have issues streaming when a lot of stuff is going on!!!! I tried it with Alan Wake, video sequences were streaming fine to Moonlight but while playing I had about one frame per 3-4 seconds. Local display on the laptop had no lag at all. I had to reduce the details and resolution before it was somewhat playable in Moonlight.**
