# Auth Service

This service exposes three endpoints:

- /login
- /auth
- /creds
- /signup

## /login endpoint

A user can login through a **POST** type request, with a HTML form, with the fields:

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

## /creds endpoint

A user can edit it's login credentials through a ***POST*** type request, with a
HTML form, at least one of the fields listed below:

- username - New user email
- passwd - New user password

The server will reply with a ***200*** (OK) status code, and the JWT
cookie will be updated, and will be valid for another hour.
If the provided email is already in use, the server will reply with a 
***409*** (Conflict) and nothing will be updated. 
If the JWT is invalid, the server will reply with a ***401*** (Unauthorized)
and the token will be destroyed.

## /signup endpoint
A new user can be added through this endpoint, with a ***POST*** request
containing a HTML form with the following fields:

- email - The new user email
- passwd - Desired password
- name - User name
- phone_number - User phone number
- birthdate - User birthday in the format ``yyyy/mm/dd``

The server will reply with a ***409*** (Conflict) status code if a user with the
given email already exists, or sending the user info through the body
