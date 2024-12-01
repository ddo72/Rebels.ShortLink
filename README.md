# dotNET-Developer-Assignment-S
Short URL is a URL shortening service where you enter a URL such as https://rebels.io/about-us and 
it returns a short URL such as http://sh.ort/GeAi9K

## Test with postman

### Encode
curl --location 'https://localhost:7127/encode' --header 'Content-Type: application/json' \
--data '{
    "Url": "https://rebels.io/about-us"
}'

### Decode
curl --location 'https://localhost:7127/decode/{id}' // replace id with the correct one

### RedirectToOriginalUrl
curl --location 'https://localhost:7127/RedirectToOriginalUrl/http%3A%2F%2Fsh.ort%2F{id}' --data ''  // replace id with the correct one
