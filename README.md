# ASP.Net Core JWT
The purpose of this repository is to showcase a better, production ready example of JWT implementation in an ASP.Net Web API, rather than the rudmentary samples you encounter in the Microsoft documentation.

## Goals
- Production grade security. This entails both signing (with a user specific salt) and encrypting the JWT token and leasing it with a small expiration time
- Issuing the token in a cookie with the same expiration time
- Issuing role claims in the JWT

## Database
The sample code uses a self-seeding and self-creating SQLite database.
The database contains the following table

## User Data
| Column        | Datatype     | Comments  |
| :------------- |:-------------| :-----|
| Email      | string |  |
| Password      | string      |    |
| Salt      | byte[]      |    |
| Firstname      | string      |    |
| Lastname      | string      |    |
| RolesString      | string      | Comma separate list of roles  |

When the project is run, the following users are automatically created:
| User Email        | Password     | Role(s)  |
| :------------- |:-------------| :-----|
| admin@company      | admin | Administrator |
| nancy.drew@company | office | Office  |
| rakesh.prasad@company | customer  | Customer  |


## API Endpoints

| Url        | Comments | Type     | Body or Querystring  |
| :------------- |:-------------| :-----| :-----|
| https://localhost:5001/api/login/register  | Registers a new user.| POST | { <br/>"email": "shailen@company", <br/>"password" : "1234",<br/>"firstName": "Shailen",<br/>"lastName" : "Sukul"<br/>} |
| https://localhost:5001/api/account/login | Validates user credentials and issues a token in a cookie. <br/>The JWT token is signed with a secret key AND a user specific salt.<br/> The JWT token is also encrypted. | POST | { <br/> "UserId" : "admin@company", <br/> "Password": "admin" <br/> } |
| https://localhost:5001/api/account/AddRoles | Adds a user role. Invalidates all existing tokens. <br/> Requires the Administrator role| POST | { <br/> "UserId" : "nancy.drew@company", <br/> "Roles" : [ "Customer" ] <br/> } |
| https://localhost:5001/api/account/RemoveRoles | Removes a user role.  Invalidates all existing tokens. <br/> Requires the Administrator role | POST | { <br/> "UserId" : "nancy.drew@company", <br/> "Roles" : [ "Customer" ] <br/> } |
| https://localhost:5001/api/account/refreshtoken | Gets a JWT token by sending a request with a valid (maybe about to expire) JWT token. <br/>The user salt is unchanged, meaning that if the user is logged in elsewhere, the older token will still work. | POST |  |
| https://localhost:5001/api/account/test  | Any user with either the Administrator or Customer role should be able to call this endpoint | GET  |  |

