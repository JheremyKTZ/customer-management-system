# Stark Customer Management system
## Business Requirements
- Manage business, residential, government and educator types of customers
- Manage our products
- Accept orders from customer online or through our call center

## Implementation details
**Stages**
- Identifying classes
- Separating responsabilities
- Establishing relationships
- Leveraging reuse

**Relationships**
1. Customer Repository uses a Customer (Collaboration)
2. Order has a Customer/Order Item/Address (Composition)
    - Aggregation: Order => Customer
    - Composition: Order => Order Item
3. Customer is a Business/Residential/Educator (Inheritance)

**Generators**
Using Bogus, all essential entities now have a way to generate stub information.
There are 2 approaches:
- Use the Data Builder to Build stubs for all entities.
- Use the Partial Data Builder to build stubs for a specific entity and its dependencies.

## Object-Oriented Programming Fundamentals in C#
More information in this Pluralsight course
https://app.pluralsight.com/library/courses/object-oriented-programming-fundamentals-csharp/table-of-contents
