# User Service

## User Info related Endpoints

This service exposes three endpoints:

- /info
- /edit
- /delete

### /info endpoint

This endpoint provides info about the logged in user, using the JWT auth header to
determine who's logged in. This info can be retrived through a **GET** type
request. If the user provided in the token is ivalid or non existent, the server
will reply with a **404** (Not Found) status code. Otherwise, the user info
will be sent in the Response body.

### /edit endpoint

A user can edit it's info through a **POST** type request, with a
HTML form, at least one of the fields listed below:

- birthday - New user birthday
- name - New user name
- phone_number - New user phone number

The updated user info will be returned in the response body.
If the JWT is invalid, the server will reply with a **404** (Not Found)
and the token will be destroyed.

### /delete endpoint

This endpoint is meant to destroy the logged in user, and its all its info from
the database. It's irreversible. You can use this endpoint through a
**DELETE** type request. After removing the user, its JWT will be destroyed
also. If the JWT is invalid, the server will respond with a **404** (Not
found) status code, and the existent JWT deleted.

## Friends Info Endpoints

Friends info and managment can be found in the `/friends`, and here are exposed 
five endpoints, being

- /get
- /pending
- /sent
- /add
- /remove
- /accept

All endpoints recive the JWT in an header named auth, to identify the logged in
user, and return in the same header a revalidated token.

### /get

This endpoint, through a **GET** request, returns a dictionary of all the friends a user has, 
the key of the dictionary being the friend's email, and the value the corresponding name.

### /pending

This endpoint is similar to the `/get` endpoint, the difference being that it
returns the info about pending friend requests.

### /sent

This endpoint is similar to the `/get` endpoint, the difference being that it
returns the info about sent friend requests.

### /add

This endpoint allows the user to send a friend request to another existent user,
via a **POST** request, providing the other user's email through a form,
containing the `friend` field.

### /remove

This endpoint removes a friend, or a pending request, through a **DELETE**
request, and providing the friend's email to whom the friendship shall be
terminated, in a **HTML** form, with the email in the field `friend`. 

### /accept

This endpoint is meant to accept a pending request, through a **POST**
request, and providing the friend's email to whom the friendship shall be
accepted, in a **HTML** form, with the email in the field `friend`. 
