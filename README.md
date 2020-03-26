
# Xbox Live Machine Account Creation Service

**Experimenting** with the revival of Xbox Live 1.0

**Warning**: This is by no means complete and there is zero guarnatee that I my self will get the XBL service running.

For those interested in contributing to the project please join the [XBL Homebrew Discord](https://discord.gg/HsHnHZ5)!

**Current Project Status**: Incomplete

- AS-REQ for the most part is decoded
- AS-REP is still being built (Response to AS-REQ)
- Encrypted Timestamp can be decrypted and decoded
- Signature verification works (verifying a real console)

**Project Background/Idea**: 
Here is what is going on right now so that people don't get triggered/confused. What I am doing is taking a different approach to what others have taken and it may seem a little out of sort right now but as time goes on it will be structured a lot better than it is right now since I am only experimenting with the idea.

The average perason may think well if this is all Kerberos in the authentication layer than why not re-implement all the stuff that Microsoft implemented in to Kerberos to add a custom layer. I say that's good and all but I am not really in the mood to sit here writting C all day to reimplement it all over again. It would take numerous amounts of hours to do all that and I am not really interested in doing that (In other words I am insanely lazy). What I am doing is basically using C# (a simple modern language) and parsing all incoming and packaging all out going to sort of make a higher than higher level Kerberos server (if that makes any sense). So then I will send these bytes as if it were an actual Kerberos server.

It seems like an insane idea and may not seem like the most "appropriate" approach but honestly I am willing to give it a shot.


This project is by **no means** associated nor endorsed by Microsoft.

**Credits**:
- Kerberos decoding/encoding parts are from GhostPack/Rubeus
