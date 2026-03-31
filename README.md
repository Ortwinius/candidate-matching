# Technical Documentation - Candidate Matching System

## Description

The project provides a ranking prototype for candidate matching and analyzes the results 
of two specific ranking algorithms:
- **Technique of Order Preference by Similarity to Ideal Solution (TOPSIS)**, and
- **Weighted Sum Model (WSM)**.

It focuses on several analytical features, including:
- **Hitman Ratio** and **Spearman Coefficient**
- **Weight Sensitivity**
- **Rank Reversal**
- **Tie Rate**

The results were gathered by performing a **Monte Carlo Simulation** 
and will be published in my thesis paper. 
Additionally, the prototype also serves as a baseline for a potential production system in the future.

---

## Project Structure

The project follows a **layered architecture** including the following key components:

### **1. API Layer**
- **Controllers**: The ranking and testing endpoints are implemented here to expose use cases over HTTP.

### **2. Application Layer**
- Contains the main **use case** logic.
- Two use cases:
    - `Ranking`: Implements TOPSIS and WSM ranking strategies.
    - `Testing`: Simulates iterative performance and comparison metrics using Monte Carlo techniques.

### **3. Domain Layer**
- Defines the core contracts, including DTOs, interfaces, and enums. It provides:
    - Interfaces for ranking (`IRankingService`) and testing (`ITestService`).
    - Data models (`CandidateDto`, `CriterionDto`, etc.).

### **4. Lib**
- Includes helper logic for math operations and debugging.

### Architecture Overview

```mermaid
flowchart TB
    Client[Client]

    subgraph API["API Layer"]
        RankingCtrl[RankingController]
        TestingCtrl[TestingController]
    end

    subgraph App["Application Layer"]
        RankCtx[RankingContext]
        Topsis[TOPSIS Ranking Service]
        Wsm[WSM Ranking Service]
        TestService[Test Service]
        CandFactory[Candidate Factory]
%%        WeightFactory[Weight Factory]
        Metrics[Metrics]
    end

    subgraph Domain["Domain Layer"]
        Models[DTOs / Interfaces / Enums]
    end

    Client --> RankingCtrl
    Client --> TestingCtrl

    RankingCtrl --> RankCtx
    RankCtx --> Topsis
    RankCtx --> Wsm

    TestingCtrl --> TestService
    TestService --> CandFactory
%%    TestService --> WeightFactory
    TestService --> RankCtx
    TestService --> Metrics

    RankingCtrl --> Models
    TestingCtrl --> Models
    RankCtx --> Models
    Topsis --> Models
    Wsm --> Models
    TestService --> Models

```
###

### Ranking Use Case - Sequence Diagram

```mermaid
sequenceDiagram
    autonumber
    actor User as Client / User
    participant RC as RankingController
    participant CX as RankingContext
    participant RS as IRankingService
    participant MB as MatrixBuilder
    participant N as Normalizer

    User->>RC: POST /api/rank<br/>(candidates, criteria, strategy)
    activate RC

    RC->>RC: Validate request
    alt invalid input
        RC-->>User: 400 Bad Request
        deactivate RC
    else valid input
        activate RC
        RC->>CX: Resolve(strategy)
        CX-->>RC: Ranking service instance

        RC->>RS: PerformRanking(candidates, weights)
        activate RS
        
        RS->>RS: Assert valid input
        RS->>MB: AddRows(candidates)
        RS->>MB: Build()
        MB-->>RS: decision matrix

        RS->>N: Apply normalization to matrix
        N-->>RS: normalized matrix

        RS->>N: ApplyWeights(normalizedMatrix, weights)
        N-->>RS: weighted normalized matrix

        alt TOPSIS strategy
            RS->>RS: Compute ideal / anti-ideal solutions
            RS->>RS: Compute distances & closeness factors
        else WSM strategy
            RS->>RS: Sum weighted criteria scores
        end

        RS->>RS: Map performances to ranking results
        RS->>RS: Sort results
        RS-->>RC: RankingResultDto
        deactivate RS

        RC-->>User: 200 OK<br/>ranking result
        deactivate RC
    end

```

### Testing Use Case - Sequence Diagram

```mermaid
sequenceDiagram
    autonumber
    actor User as Client / User
    participant TC as TestingController
    participant TS as ITestService
    participant WF as WeightFactory
    participant CF as CandidateFactory
    participant MC as MetricRegistry
    participant CX as RankingContext

    User->>+TC: POST /api/test<br/>(iterations, candidateAmount, criteriaAmount, weights)
    TC->>TC: Validate request
    alt Invalid input
        TC-->>User: 400 Bad Request
        deactivate TC
    else Valid input
        activate TC
        TC->>+TS: RunTests(iterations, candidateAmount, criteriaAmount, weights)

        %% Step: Candidate and Weight Generation
        TS->>+WF: Generate weights (unless specified)
        WF-->>TS: Weights
        TS->>+CF: Create candidates (unless specified)
        CF-->>TS: Candidate list 

        %% Step: Ranking and Metrics
        loop For each iteration
            %% Get algo instances
            TS->>TS: Prepare testing context (candidates, weights)
            TS->>CX: Get instances of algorithms to compare
            CX-->>TS: Ranking Instances (TOPSIS & WSM)

            %% Execute Rankings
            TS->>TopsisRankingService: PerformRanking(candidates, weights)
            TS->>WsmRankingService: PerformRanking(candidates, weights)

            %% Calculate Metrics
            TS->>+MC: Calculate all pair and single metrics
            MC-->>TS: Metric results
        end

        TS-->>TC: TestResultDto
        deactivate TS

        TC-->>User: 200 OK<br/>Test Result
        deactivate TC
    end

```

## **Installation and Usage**

### Prerequisites:
- [.NET 10.0](https://dotnet.microsoft.com/download) or higher

### How to Run:
1. Clone the repository:
   ```bash
   git clone https://github.com/Ortwinius/candidate-matching.git
   cd CandidateMatchingProject
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the project:
   ```bash
   dotnet run --project CandidateMatching.Project
   ```

4. Access the Swagger UI :
   ```
   http://localhost:5094/swagger
   ```
   Alternatively, you could use ny API CLI or framework (e.g. Postman)
---

## **License**

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

Feel free to contribute or use this code for educational or personal projects.