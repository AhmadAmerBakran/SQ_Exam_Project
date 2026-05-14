# FoodOrderQuality

Software Quality exam project by Ahmad Amer Bakran and Mahmoud Eybo.

## Project idea

This project is a small C# Web API for calculating food order prices and delivery validity. The practical focus is not the food domain itself, but how systematic testing techniques can improve software quality in a system with business rules and non-trivial control flow.

Main business rules:

- The basket must contain at least one item.
- Each item must have a valid id, name, price and quantity.
- Quantity must be between 1 and 20.
- The subtotal must be at least 75 DKK for delivery.
- Delivery distance must be between 0 and 10 km.
- Opening hours must allow the requested delivery time.
- Delivery fee depends on distance: 0-2 km = 29 DKK, >2-5 km = 39 DKK, >5-10 km = 59 DKK.
- Discount codes are checked through a repository dependency.
- WELCOME10 gives 10% discount when subtotal is at least 100 DKK.
- FREESHIP removes delivery fee when subtotal is at least 200 DKK.
- TAKE25 removes 25 DKK when subtotal is at least 150 DKK.



The project demonstrates theory through practical work:

| Exam/course area | Where it is shown |
|---|---|
| Design for testability | Core logic is separated from API, dependencies are injected through interfaces |
| Unit testing | `FoodOrderQuality.UnitTests` |
| Data-driven testing | xUnit `[Theory]`, `[InlineData]`, `[MemberData]` |
| Mocking | Moq tests for `IClock` and `IDiscountRepository` |
| BDD/Cucumber style testing | Reqnroll feature file in `FoodOrderQuality.BddTests` |
| API testing | Postman collection in `postman/` |
| Equivalence classes | Documented in `docs/test-design/Test_Design.md` |
| Boundary value testing | Distance, quantity, subtotal and opening hours tests |
| Decision table testing | Order acceptance/rejection table in the test design and unit tests |
| White-box testing | Path testing and cyclomatic complexity in the test design |

## Run the project

```bash
dotnet restore
dotnet build
dotnet test
```

Run the API:

```bash
dotnet run --project FoodOrderQuality.Api
```

Open Swagger in the browser http://localhost:5000/swagger/index.html as we're running the API on port 5000

```text
POST /api/orders/quote
GET /api/health
```

## Postman

Import this collection into Postman:

```text
postman/FoodOrderQuality.postman_collection.json
```