# 💰 **MiniBank**

A **console-based banking simulation** that demonstrates **OOP principles** — abstraction, inheritance, interfaces, and polymorphism — through a simple, interactive banking experience.

---

## 🚀 Overview
**MiniBank** lets users:
- Create **Checking**, **Savings**, or **Loan** accounts  
- Perform **deposits**, **withdrawals**, and **view balances**
- Run **month-end processing** that behaves differently per account type

Built to illustrate clean architecture, interface segregation, and polymorphic behavior in .NET.

---

## 🎯 Learning Goals
- Model a domain using **abstract base classes** and **derived types**
- Implement **interfaces** for optional capabilities:
  - Interest accrual  
  - Overdraft policies  
  - Statement printing
- Invoke polymorphic behavior across mixed account types
- Apply **input validation** and clear console UX

---

## ⚙️ Requirements

### Functional
- Maintain an in-memory list of accounts  
- Supported: `CheckingAccount`, `SavingsAccount`, `LoanAccount`

**Menu Options**
```

1. List accounts
2. Create account
3. Deposit
4. Withdraw
5. View statement
6. Run month-end
7. Exit

```

**Account Rules**
| Type | Behavior |
|------|-----------|
| **Checking** | Allows overdraft (e.g. up to -200) |
| **Savings** | Earns 1% monthly interest if balance > 0 |
| **Loan** | Negative balance = debt; interest on owed amount |

> 💡 All inputs validated (amount > 0, valid account ID, etc.)

---

### Non-Functional
- Base class: `BankAccount` (shared state + behavior)
- Interfaces:
  - `ITransactable` – deposit/withdraw
  - `IInterestBearing` – monthly interest/fees
  - `IOverdraftPolicy` – overdraft limit
  - `IStatement` – print recent operations
- Polymorphism for month-end and statements
- Beginner-friendly, standard .NET code

---

## 🧩 Interfaces Overview

| Interface | Purpose | Methods | Implemented By |
|------------|----------|----------|----------------|
| **ITransactable** | Common money ops | `Deposit`, `Withdraw` | All accounts |
| **IInterestBearing** | Monthly interest | `ApplyMonthlyInterest()` | Savings, Loan |
| **IOverdraftPolicy** | Overdraft limit | `OverdraftLimit` | Checking |
| **IStatement** | Print logs | `PrintStatement()` | All accounts |

---

## 🏗️ Architecture & Patterns

- **Abstract Base:** `BankAccount` → common fields (Id, Owner, Balance, Log)
- **Derived:** Specialized rules for each account type
- **Patterns:**
  - Template-like withdraw flow with overrides
  - Strategy via interfaces
  - Polymorphism for unified month-end processing

**Folder Structure**
```

MiniBank/
├── Program.cs
├── Models/
│    ├── BankAccount.cs
│    ├── CheckingAccount.cs
│    ├── SavingsAccount.cs
│    ├── LoanAccount.cs
│    └── Interfaces/
│         ├── ITransactable.cs
│         ├── IInterestBearing.cs
│         ├── IOverdraftPolicy.cs
│         └── IStatement.cs
└── Services/
└── AccountRegistry.cs

```

---

## 💻 UI Example

```

=== MINIBANK ===

1. List accounts
2. Create account
3. Deposit
4. Withdraw
5. View statement
6. Run month-end
7. Exit
   Select: 2
   Type (Checking/Savings/Loan): Savings
   Owner: Alice
   Opening deposit: 500
   Created #1 Savings for Alice with BAL ¤500.00
   Select: 6
   Month-end applied (interest/fees)

```

✅ Validations  
- Amounts > 0  
- Account must exist  
- Withdraw rules enforced per account type  

---

## 🧪 Testing Plan
| Test | Expected Result |
|------|-----------------|
| Checking overdraft exceeded | Error |
| Savings month-end | +1% balance |
| Loan deposit | Reduces debt |
| Loan month-end | Adds interest |
| Statement | Shows operations in order |

---

## ✅ Acceptance Criteria
- Month-end applies interest via a single polymorphic call  
- Checking overdraft respected  
- Savings cannot overdraft  
- Loan behaves correctly for debt/repayment  
- Invalid inputs handled gracefully  

---

## 📊 Evaluation Rubric (10 pts)
| Category | Points |
|-----------|---------|
| Correctness | 4 |
| OOP Concepts | 3 |
| Code Quality | 2 |
| UX Clarity | 1 |

---

## 🧠 Extensions (Optional)
- Account transfers  
- FixedDepositAccount with penalties  
- JSON persistence  
- Simple authentication per owner  
- Multi-currency or locale formatting  

---

## 🧱 Deliverables
- Runtime: **.NET 8 or 9 Console App**
- Timebox: **1–2 days**
- Conventions: PascalCase, `decimal` for money, guard clauses

---

## 🔍 Reviewer Checklist
- Interfaces model distinct capabilities  
- Polymorphic month-end via `List<BankAccount>`  
- Correct overdraft and loan rule enforcement  
- Clear, readable console output  

---

> 🧾 *"Good design adds discipline without adding complexity."*  
> — Unknown
