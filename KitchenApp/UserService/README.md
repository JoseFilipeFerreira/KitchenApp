# User Service

This service exposes three endpoints:

- /user/info
- /user/edit
- /user/delete

## /info endpoint

This endpoint provides info about the logged in user, using the JWT auth header to
determine who's logged in. This info can be retrived through a **GET** type
request. If the user provided in the token is ivalid or non existent, the server
will reply with a **404** (Not Found) status code. Otherwise, the user info
will be sent in the Response body.

## /edit endpoint

A user can edit it's info through a **POST** type request, with a
HTML form, at least one of the fields listed below:

- birthday - New user birthday
- name - New user name

The updated user info will be returned in the response body.
If the JWT is invalid, the server will reply with a **404** (Not Found)
and the token will be destroyed.

## /delete endpoint

This endpoint is meant to destroy the logged in user, and its all its info from
the database. It's irreversible. You can use this endpoint through a
**DELETE** type request. After removing the user, its JWT will be destroyed
also. If the JWT is invalid, the server will respond with a **404** (Not
found) status code, and the existent JWT deleted.
