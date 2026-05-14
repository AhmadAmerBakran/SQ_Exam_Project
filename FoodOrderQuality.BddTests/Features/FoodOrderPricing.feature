Feature: Food order pricing
  As a restaurant owner
  I want the system to validate orders and calculate prices consistently
  So that customers see correct delivery fees, discounts and totals

Scenario: Valid order without discount
  Given the customer has an order subtotal of 150 DKK
  And the delivery distance is 4 km
  And the delivery time is "2026-05-18T18:00:00+00:00"
  When the order is calculated
  Then the order should be accepted
  And the delivery fee should be 39 DKK
  And the total should be 189 DKK

Scenario: Customer gets free delivery
  Given the customer has an order subtotal of 220 DKK
  And the delivery distance is 4 km
  And the delivery time is "2026-05-18T18:00:00+00:00"
  And the discount code is "FREESHIP"
  When the order is calculated
  Then the order should be accepted
  And the delivery fee should be 0 DKK
  And the total should be 220 DKK

Scenario: Delivery distance is too far
  Given the customer has an order subtotal of 150 DKK
  And the delivery distance is 10.01 km
  And the delivery time is "2026-05-18T18:00:00+00:00"
  When the order is calculated
  Then the order should be rejected with error "DISTANCE_TOO_FAR"
