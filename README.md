<div align="center">

![Momo Logo](http://cdn.mutedevs.nl/git/Momo/MomoLogo.png)

# Momo

### Send copypastas, code, data dumps, whatever you want for free

</div>

<hr />

## What is Momo
Momo is a service with which you can easily share code and other things in the form of text. Kind of like PasteBin. 

## Why did I make this
I was bored during my vacation 🌴

## API Documentation
No API key is required for creating new pastes with the Momo API.

### [GET] /paste/{id}
Retrieve a paste by id

### [POST] /paste
Create a new paste

#### Required Parameters

| Name                   | Description                                                        | Provided in |
| ---------------------- | ------------------------------------------------------------------ | ----------- |
| Title                  | The title of the paste                                             | Body        |
| Content                | The contents of the paste (Base64 Encoded)                                         | Body        |

#### Optional Parameters

| Name                   | Description                                                        | Provided in |
| ---------------------- | ------------------------------------------------------------------ | ----------- |
| InvalidateAfterViewing | Discard the post after it has been viewed once                     | Query       |
| Expires                | Time in minutes until the paste should expire, 0 (Default) = never | Query       |

This route will create a new paste and return the url of the newly generated paste.



*Each paste is currently limited to 100.000 characters*
