# ASP.Net Core JWT
The purpose of this repository is to showcase a better, production ready example of JWT implementation in an ASP.Net Web API, rather than the rudmentary samples you enconter in Microsoft documentation.

Goals
- Production grade security. This entails both signing and encrypting the JWT token and leasing it with a small expiration time
- Issuing the token in a cookie with the same expiration time
- Issuing role claims in the JWT

Database: 
The sample code uses a self-seeding and self-creating SQLite database.
The database contains the following table

##User
| Column        | Datatype     | Comments  |
| :------------- |:-------------| :-----|
| Email      | string |  |
| Password      | string      |    |
| Salt      | byte[]      |    |
| Firstname      | string      |    |
| Lastname      | string      |    |
| RolesString      | string      | Comma separate list of roles  |
