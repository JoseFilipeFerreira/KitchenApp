# Recipe Service

- /get
- /search
- /star
- /stared
- /unstar

All requests need to have the `auth` header with the JWT, and the response
will also have the same header but with a revalidated token.

## /get endpoint

Through a **GET** request, this endpoint will return a recipe that corresponds
to the given id provided in the route, like ``/get/id``

## /search endpoint

Through this endpoint the user can search existing products, making a **POST**
request, containing form data with a field named `keys`. Optionally, can be sent
in the form a field named `inventory` containing a inventory UID, to filter
recipes containing products presents in it.
The search result will be sent in the response body.

## /star endpoint

Through a **POST** request, and providing a form with a field `id`, a call to
this endpoint adds the recipe with the given id to the user's favourite list.

## /stared endpoint

Through a **GET** request, calling this endpoint will return all the recipes
present in the logged in user favourite list.

## /unstar

Through a **POST** request, and providing a form with a field `id`, a call to
this endpoint removes the recipe with the given id to the user's favourite list.
