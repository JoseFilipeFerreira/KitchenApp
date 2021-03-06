# ShoppingList Service

## Endpoints

- /info
- /all
- /addproduct
- /editproduct
- /removeproduct
- /add
- /edit
- /remove
- /share
- /shared

### /info endpoint

This endpoint provides info about a ShoppingList, using the JWT auth header to
determine if a user can see that info. This info can be retrived through a 
**GET** type request, passing the Shoppinglist uid through the URL (see example). 
If the ShoppingList provided is ivalid or non existent, the server will reply 
with a **404** (Not Found) status code. Otherwise, the ShoppingList info
will be sent in the Response body. If the user present in the token isn't valid
it will be returned a **401** (Unauthorized) status code.

``http://example.com/shopping/info/{uid}``

### /all endpoint

Making a **GET** request to this endpoint, alongside with the JWT auth header,
will return, in the request body, a dictionary containing the name and UID
of all the logged in user ShoppingLists. If the user present in the token isn't valid
it will be returned a **401** (Unauthorized) status code.

### /addproduct

This endpoint adds a product to a specified ShoppingList through a **POST** request
with a JWT token indicating the logged in user in the `auth` header. All the
relevant info from the product is recived by a `form` with the following fields:

- product: The uid from the product to be added
- quantity: The quantity to be bought
- uid: The Shoppinglist uid

If the product uid or the Shoppinglist uid sent don't correspond to a valid
Shoppinglist the server will respond with a **404** (Not found) status code.

### /editproduct

This endpoint edits the info about a product present in the specified Shoppinglist,
with a **POST** request and the info present in a form, similar to the
``/addproduct``.

### /removeproduct

This endpoint removes a product from the a given Shoppinglist, through a **POST**
request, the relevant info present in a form and the JWT token in the auth
header. The fields required to this endpoint are:

- uid: The Shoppinglist uid
- product: The uid from the product to be added

If the product uid or the Shoppinglist uid sent don't correspond to a valid
Shoppinglist the server will respond with a **404** (Not found) status code.

### /add

This endpoint is meant the logged in user create a new Shoppinglist, through a
**POST** request and the info present in a form with a field called `name`, with
the desired Shoppinglist name. A user cannot have two Shoppinglists with the same
name, so if a Shoppinglist already exists with the given name, a response with a
**409** (Conflict) status code will be sent.

### /edit

This endpoint is meant the logged in user edit a given Shoppinglist, through a
**POST** request and the info present in a form with a field called `name`, with
the desired Shoppinglist name, and a field `uid` with the desired . A user cannot 
have two Shoppinglists with the same name, so if a Shoppinglist already exists with 
the given name, a response with a **409** (Conflict) status code will be sent.

### /remove endpoint

This endpoint is meant to destroy the Shoppinglist passed in the URL, and its all its 
info from the database. It's irreversible. You can use this endpoint through a
**DELETE** type request. If the JWT is invalid or the logged in user isn't the Shoppinglist
owner, the server will respond with a **401** (Unauthorized) status code. 

``http://example.com/shopping/remove/{uid}``

### /share endpoint

This endpoint allows a user to share one of its Shoppinglists with another user
registered in the system. This can be achived by a **POST** request containing a
form with two fields, ``uid`` correponding to the Shoppinglist to share and
`friend` with the account email who the Shoppinglist will be shared. If any of the
form info is invalid, the server will reply with a **404** (Not Found) status
code.

### /shared endpoint

Making a **GET** request to this endpoint, alongside with the JWT auth header,
will return, in the request body, a dictionary containing the name and UID
of all the logged in user shared Shoppinglists. If the user present in the token isn't valid
it will be returned a **401** (Unauthorized) status code.


