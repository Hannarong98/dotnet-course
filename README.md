# Movie REST API

* .NET 9
* Postgres npgsql
* Dapper
* Keycloak OAuth2
* Scalar API doc


## Running locally

- setup required secrets
  - client secrets for various consumer (Scalar and SDK consumer)
  - Keycloak auth/token urls
  - Connection string to database
- docker compose up
- starts the api via IDE
- some ClickOps in KeyCloak required

## OpenAPI Doc

- accessible at https://localhost:7175/docs