# Product Service

- /get
- /add
- /search
- /getall
- /category/all

All requests need to have the `auth` header with the JWT, and the response
will also have the same header but with a revalidated token.

## /get endpoint

This endpoint retrives the information about a given product, which UID is
sent in the request URL, `/get/uid`, returning the information in the request
body. If the given product doesn't exist the response will be a **404** (Not
Found) status code.

## /add endpoint

This endpoint is meant to add a new product to the system. This can be achived
by a **POST** request, with form data containing the following fields:

- name: Containing the name of the product
- category: Containing the product category
- quantity: Containing the product quantity by package, like 10 cookies.
- units; Specifying the units of the field above, like 150 grams.
- price: Containing the product price

## /search endpoint

Through this endpoint the user can search existing products, making a **POST**
request, containing form data with a field named `regex`. This search will match
with a product if its name starts with the given string, or if it matches the
given regex pattern. If a field called `category`, containing a category name,
is provided, the search will only contain products that belong to that category.

The search result will be sent in the response body.

## /getall endpoint

This endpoint returns all the products present in the database in the response
body, and can be called with a **GET** request.

## /category/all

Calling this endpoint through a **GET** request, will return a list containing
all the avaliable categories present in the database.

## /category/getprods

Calling this endpoint through a **POST** request, will return a list containing
all the products from a category, given in a form in the field **category**,
present in the database.
