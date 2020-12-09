Custom Authorization Filter
https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-5.0

Url:

https://jasonwatmore.com/post/2020/05/25/aspnet-core-3-api-jwt-authentication-with-refresh-tokens

[Encrypting Tokens]
https://stackoverflow.com/questions/53505198/force-the-authentication-middleware-to-only-accept-encrypted-tokens/53506779#53506779

[Using Dynamic Signing Keys]
https://www.gwllosa.com/post/dynamic-key-validation-with-jwt-in-asp-net-core

https://www.c-sharpcorner.com/article/implement-jwt-in-asp-net-core-3-1/

https://medium.com/dev-genius/jwt-bearer-authentication-for-machine-to-machine-and-single-page-applications-1c8ba1211a90

https://github.com/dotnet/aspnetcore/tree/master/src/Middleware

Get token
https://localhost:5001/api/token

Protected endpoint
https://localhost:5001/api/weatherforecast

AspNetAuthorizationWorkshop
https://github.com/blowdart/AspNetAuthorizationWorkshop

Policy-based authorization in ASP.NET Core
https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-5.0

Database
dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet ef migrations add InitialCreate --project api.data --startup-project api

dotnet ef database update --project api.data --startup-project api
cd ..

Postman

REGISTER
https://localhost:5001/api/account/register
POST
{
    "email": "shailensukul@gmail.com",
    "password" : "1234",
    "firstName": "Shailen",
    "lastName" : "Sukul"
}


LOGIN
https://localhost:5001/api/account/login?email=shailensukul@gmail.com&password=1234
GET