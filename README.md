# dotNET-Developer-Assignment-S
Short URL is a URL shortening service where you enter a URL such as https://rebels.io/about-us and 
it returns a short URL such as http://sh.ort/GeAi9K

## Run API
dotnet run --project Rebels.ShortLink.Api

## Encode (Postman)

NOTE:
In this assignment, I have made the POST request idempotent after the second request. Multiple requests with the same original URL will get the same short URL. 
(As we know, POST can be used for operations that are inherently non-idempotent.)

curl --location 'https://localhost:7127/encode' --header 'Content-Type: application/json' \
--data '{
    "Url": "https://rebels.io/about-us"
}'

![image](https://github.com/user-attachments/assets/e990492c-27ae-4e5e-a137-c2739a09dc3e)


## Decode (Postman)

curl --location 'https://localhost:7127/decode/{id}' // replace id with the correct one

![image](https://github.com/user-attachments/assets/213f3e5e-1642-4d7b-8f38-101626f30429)


## RedirectToOriginalUrl (Postman)
NOTE: 
I am not sure that I understand the requirements correctly. The API primarily focuses on resources/data and enables clients to interact with them. 
Redirects are generally not effective in an API context, as most clients interacting with an API do not function like web browsers.

However, the commit (dea10b6) contains the code that opens the browser and navigates to the original URL.

curl --location 'https://localhost:7127/RedirectToOriginalUrl/http%3A%2F%2Fsh.ort%2F{id}' --data ''  // replace id with the correct one

![image](https://github.com/user-attachments/assets/f9f0c465-89af-45c0-ae1c-739eedb8df54)
