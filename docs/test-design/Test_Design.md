# Test design for FoodOrderQuality

## 1. Requirements under test

| ID | Requirement | Main test technique |
|---|---|---|
| R1 | Basket must contain at least one item | Equivalence class testing |
| R2 | Quantity must be between 1 and 20 | Boundary value testing |
| R3 | Subtotal must be at least 75 DKK | Boundary value testing |
| R4 | Delivery distance must be between 0 and 10 km | Equivalence class + boundary value testing |
| R5 | Delivery fee depends on distance interval | Boundary value testing |
| R6 | Delivery is only accepted during opening hours | Boundary value testing |
| R7 | Discount code must exist | Decision table testing |
| R8 | Discount minimum subtotal must be reached | Boundary value testing |
| R9 | Correct final price must be calculated | Unit + API testing |

## 2. Equivalence classes

### Delivery distance

| Class | Range | Representative value | Expected result |
|---|---:|---:|---|
| Invalid low | < 0 km | -1 | Reject |
| Valid short | 0-2 km | 1 | Accept, fee 29 |
| Valid medium | >2-5 km | 4 | Accept, fee 39 |
| Valid long | >5-10 km | 8 | Accept, fee 59 |
| Invalid high | >10 km | 11 | Reject |

### Quantity

| Class | Range | Representative value | Expected result |
|---|---:|---:|---|
| Invalid low | < 1 | 0 | Reject |
| Valid | 1-20 | 5 | Accept |
| Invalid high | > 20 | 21 | Reject |

### Subtotal

| Class | Range | Representative value | Expected result |
|---|---:|---:|---|
| Invalid low | < 75 DKK | 50 | Reject |
| Valid | >= 75 DKK | 100 | Accept if other conditions are valid |

## 3. Boundary value tests

| Rule | Boundary values |
|---|---|
| Quantity 1-20 | 0, 1, 20, 21 |
| Minimum subtotal 75 | 74.99, 75, 75.01 |
| Delivery distance 0-10 | -0.01, 0, 2, 2.01, 5, 5.01, 10, 10.01 |
| Weekday opening hours 16:00-22:00 | 15:59, 16:00, 22:00, 22:01 |
| Friday/Saturday closing time 23:30 | 23:30, 23:31 |

## 4. Decision table for order acceptance

This table is used to decide whether a basic order should be accepted before discount calculation.

| Rule | Basket valid | Subtotal >= 75 | Distance valid | Open now | Expected result |
|---|---|---|---|---|---|
| D1 | No | Yes | Yes | Yes | Reject: `BASKET_EMPTY` or item validation error |
| D2 | Yes | No | Yes | Yes | Reject: `MINIMUM_ORDER_NOT_REACHED` |
| D3 | Yes | Yes | No | Yes | Reject: `DISTANCE_NEGATIVE` or `DISTANCE_TOO_FAR` |
| D4 | Yes | Yes | Yes | No | Reject: `OUTSIDE_OPENING_HOURS` |
| D5 | Yes | Yes | Yes | Yes | Accept |

The unit test `OrderPricingDecisionTableTests` contains one test case for each row.

## 5. Decision table for discount handling

| Rule | Code provided | Code exists | Not expired | Minimum reached | Discount type | Expected result |
|---|---|---|---|---|---|---|
| C1 | No | - | - | - | - | Accept without discount |
| C2 | Yes | No | - | - | - | Reject: `DISCOUNT_CODE_UNKNOWN` |
| C3 | Yes | Yes | No | - | - | Reject: `DISCOUNT_CODE_EXPIRED` |
| C4 | Yes | Yes | Yes | No | - | Reject: `DISCOUNT_MINIMUM_NOT_REACHED` |
| C5 | Yes | Yes | Yes | Yes | Percentage | Apply percentage discount |
| C6 | Yes | Yes | Yes | Yes | Free delivery | Set delivery fee to 0 |
| C7 | Yes | Yes | Yes | Yes | Fixed amount | Subtract fixed amount |

## 6. White-box testing and cyclomatic complexity

The main white-box target is `OrderPricingService.Calculate`.

Important decision points:

1. Request is null.
2. Basket is empty.
3. Item id is missing.
4. Item name is missing.
5. Item price is invalid.
6. Item quantity is outside the valid range.
7. Validation errors exist after item validation.
8. Subtotal is below minimum.
9. Distance is negative.
10. Distance is above maximum.
11. Requested time is outside opening hours.
12. Validation errors exist after business-rule validation.
13. Discount code is provided.
14. Discount code is unknown.
15. Discount code is expired.
16. Discount minimum is not reached.
17. Discount type is percentage/free delivery/fixed amount/unsupported.

A simplified estimate using `decision points + 1` gives a cyclomatic complexity around 18. The exact value may differ depending on how we count `switch` branches and loop decisions, but the method clearly has non-trivial control flow.

### Practical interpretation

The point of the white-box analysis is not to test every possible combination. The point is to identify independent paths and then choose meaningful tests:

- Null request path.
- Empty basket path.
- Invalid item path.
- Subtotal too low path.
- Distance too far path.
- Closed restaurant path.
- Accepted order without discount path.
- Unknown discount path.
- Expired discount path.
- Discount minimum not reached path.
- Percentage discount path.
- Free delivery path.
- Fixed discount path.


## 7. Traceability matrix

| Requirement | Unit test | BDD scenario | Postman test |
|---|---|---|---|
| R1 Basket not empty | `OrderPricingDecisionTableTests` | - | - |
| R2 Quantity 1-20 | `OrderPricingBoundaryTests` | - | - |
| R3 Minimum subtotal | `OrderPricingBoundaryTests` | - | - |
| R4 Distance max 10 | `OrderPricingBoundaryTests` | Delivery distance is too far | Distance above 10 km returns 400 |
| R5 Delivery fee intervals | `DeliveryFeeCalculatorTests` | Valid order without discount | Valid order returns calculated total |
| R6 Opening hours | `OpeningHoursPolicyTests` | - | - |
| R7 Discount exists | `OrderPricingDiscountTests` | - | - |
| R8 Discount minimum | `OrderPricingDiscountTests` | - | - |
| R9 Final price | `OrderPricingDiscountTests` | Customer gets free delivery | Free shipping discount returns delivery fee 0 |
