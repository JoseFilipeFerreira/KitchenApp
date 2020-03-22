#Auth Service

This service exposes two endpoints:

- /login
- /auth

## /login endpoint

A user can login through a **GET** type request, with a HTML form, with the fields:

- username - User email
- passwd - User password

The server will reply with a **202** (Accepted) status code, and a cookie named **token**, 
containing a JWT token, valid for 1 hour, or with a **401** (Unauthorized) status code if 
any of the given info is invalid.

## /auth endpoint

This endpoint is meant to validate any token that arrives, in a cookie named **token**.
If no token is provided or a invalid token is provided, the server will reply with a **401**
(Unauthorized) status code, and remove the cookie if it exists.
If a valid token is provided, the server will reply with a **202** (Accepted) status code, 
and the token will be valid for another hour.
