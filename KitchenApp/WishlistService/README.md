# Wishlist Service

## Endpoints

- /info
- /all
- /addproduct
- /removeproduct
- /add
- /edit
- /remove
- /share
- /shared

### /info endpoint

This endpoint provides info about a wishlist, using the JWT auth header to
determine if a user can see that info. This info can be retrived through a 
**GET** type request, passing the wishlist uid through the URL (see example). 
If the wishlist provided is ivalid or non existent, the server will reply 
with a **404** (Not Found) status code. Otherwise, the user info
will be sent in the Response body. If the user present in the token isn't valid
it will be returned a **401** (Unauthorized) status code.

``http://example.com/wishlist/info/{uid}``

### /all endpoint

Making a **GET** request to this endpoint, alongside with the JWT auth header,
will return, in the request body, a dictionary containing the name and UID
of all the logged in user wishlists. If the user present in the token isn't valid
it will be returned a **401** (Unauthorized) status code.

### /addproduct

This endpoint adds a product to a specified wishlist through a **POST** request
with a JWT token indicating the logged in user in the `auth` header. All the
relevant info from the product is recived by a `form` with the following fields:

- product: The uid from the product to be added
- quantity: The quantity present in the wishlist
- expire: A datetime corresponding to the product's expiration date
- uid: The wishlist uid

If the product uid or the wishlist uid sent don't correspond to a valid
wishlist the server will respond with a **404** (Not found) status code.

### /removeproduct

This endpoint removes a product from the a given wishlist, through a **POST**
request, the relevant info present in a form and the JWT token in the auth
header. The fields required to this endpoint are:

- uid: The wishlist uid
- product: The uid from the product to be added

If the product uid or the wishlist uid sent don't correspond to a valid
wishlist the server will respond with a **404** (Not found) status code.

### /add

This endpoint is meant the logged in user create a new wishlist, through a
**POST** request and the info present in a form with a field called `name`, with
the desired wishlist name. A user cannot have two wishlists with the same
name, so if a wishlist already exists with the given name, a response with a
**409** (Conflict) status code will be sent.

### /edit

This endpoint is meant the logged in user edit a given wishlist, through a
**POST** request and the info present in a form with a field called `name`, with
the desired wishlist name, and a field `uid` with the desired . A user cannot 
have two wishlists with the same name, so if a wishlist already exists with 
the given name, a response with a **409** (Conflict) status code will be sent.

### /remove endpoint

This endpoint is meant to destroy the wishlist passed in the URL, and its all its 
info from the database. It's irreversible. You can use this endpoint through a
**DELETE** type request. If the JWT is invalid or the logged in user isn't the wishlist
owner, the server will respond with a **401** (Unauthorized) status code. 

``http://example.com/wishlist/remove/{uid}``

### /share endpoint

This endpoint allows a user to share one of its wishlists with another user
registered in the system. This can be achived by a **POST** request containing a
form with two fields, ``uid`` correponding to the wishlist to share and
`friend` with the account email who the wishlist will be shared. If any of the
form info is invalid, the server will reply with a **404** (Not Found) status
code.

### /all endpoint

Making a **GET** request to this endpoint, alongside with the JWT auth header,
will return, in the request body, a dictionary containing the name and UID
of all the logged in user shared wishlists. If the user present in the token isn't valid
it will be returned a **401** (Unauthorized) status code.


