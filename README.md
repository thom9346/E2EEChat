# End-to-End Encrypted Chat Application
This project is a secure chat application desgined to provide end-to-end encryption between clients. Please ensure you are on the main branch for testing.

# How to run

**IMPORTANT** You _must_ create a `appsettings.json` file in the `ChatApi.WebApi` folder for this to run. The file is ignored in git.

1. Create a `appsettings.json` file at `./ChatApi/ChatApi.WebApi/appsettings.json`. The file should look like this:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<YOUR_LOCAL_DB_SERVER>;Database=ChatApiDb;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_MUST_BE_ATLEAST_32_CHARACTERS_LONG",
    "Issuer": "yourdomain.com",
    "Audience": "yourdomain.com"
  },
  "Pepper": "A secret pepper for password hashing",
  "Encryption": {
    "Key": "<FOR_AES_ENCRYPTION_MUST_BE_EXACTLY_32_CHARACTERS>"
  },
  "Email": {
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": "587",
      "Username": "<YOUR_GMAIL_ADDRESS@gmail.com>",
      "Password": "<YOUR_APPLICATION_PASSWORD_FOR_GMAIL>",
      "From": "<YOUR_GMAIL_ADDRESS@gmail.com>"
    }
  }
}
```

2. `cd ChatApi/ChatApi.WebApi/`
3. `dotnet run --urls="http://localhost:5000"`
4. `cd ChatFrontend`
5. `npm install`
6. `ng serve --o` (This should open up http://localhost:4200/login)