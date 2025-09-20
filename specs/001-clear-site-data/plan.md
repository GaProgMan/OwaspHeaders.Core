
# Implementation Plan: Clear-Site-Data Header Support

**Branch**: `001-clear-site-data` | **Date**: 2025-09-20 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/Users/jamie/Code/OwaspHeaders.Core/specs/001-clear-site-data/spec.md`

## Execution Flow (/plan command scope)
```
1. Load feature spec from Input path
   → If not found: ERROR "No feature spec at {path}"
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detect Project Type from context (web=frontend+backend, mobile=app+api)
   → Set Structure Decision based on project type
3. Fill the Constitution Check section based on the content of the constitution document.
4. Evaluate Constitution Check section below
   → If violations exist: Document in Complexity Tracking
   → If no justification possible: ERROR "Simplify approach first"
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → If NEEDS CLARIFICATION remain: ERROR "Resolve unknowns"
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file (e.g., `CLAUDE.md` for Claude Code, `.github/copilot-instructions.md` for GitHub Copilot, `GEMINI.md` for Gemini CLI, `QWEN.md` for Qwen Code or `AGENTS.md` for opencode).
7. Re-evaluate Constitution Check section
   → If new violations: Refactor design, return to Phase 1
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command
```

**IMPORTANT**: The /plan command STOPS at step 7. Phases 2-4 are executed by other commands:
- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary
Implement Clear-Site-Data header support for OwaspHeaders.Core middleware to enable developers to configure automated browser cache/cookie clearing on specific paths (primarily logout endpoints), reducing session hijacking risks by ensuring complete logout security through OWASP-recommended header directives.

## Technical Context
**Language/Version**: C# .NET (multiple target frameworks per existing project structure)
**Primary Dependencies**: ASP.NET Core middleware pipeline, Microsoft.AspNetCore.Http
**Storage**: N/A (header configuration only)
**Testing**: xUnit (following existing test patterns in OwaspHeaders.Core.Tests)
**Target Platform**: Cross-platform .NET (Windows, Linux, macOS) via ASP.NET Core
**Project Type**: single - .NET library project extending existing middleware
**Performance Goals**: Minimal overhead (<1ms header processing), thread-safe configuration
**Constraints**: Zero breaking changes to existing API, follows existing middleware patterns
**Scale/Scope**: Enterprise-ready middleware component integrated with existing OwaspHeaders.Core

## Constitution Check
*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Security-First Design**:
- [x] Feature enhances or maintains security posture - no weakening of security
- [x] OWASP security guidelines considered and followed
- [x] Security headers implementation aligns with OWASP Secure Headers Project

**Library-First Architecture**:
- [x] Components are standalone and independently testable
- [x] Clear separation of concerns with single-purpose modules
- [x] No organizational-only abstractions

**Test-Driven Development**:
- [x] Test strategy defined before implementation planning
- [x] 90%+ coverage planned for security-critical paths
- [x] Integration tests planned for middleware configuration scenarios

**Code Quality Standards**:
- [x] Plan includes dotnet-format validation
- [x] EditorConfig compliance ensured
- [x] Primary constructor restrictions noted for example projects

**Documentation Requirements**:
- [x] Security implications documented for public APIs
- [x] Examples planned for all configuration options
- [x] OpenSSF compliance considerations included

## Project Structure

### Documentation (this feature)
```
specs/[###-feature]/
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output (/plan command)
├── data-model.md        # Phase 1 output (/plan command)
├── quickstart.md        # Phase 1 output (/plan command)
├── contracts/           # Phase 1 output (/plan command)
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (repository root)
```
# Option 1: Single project (DEFAULT)
src/
├── models/
├── services/
├── cli/
└── lib/

tests/
├── contract/
├── integration/
└── unit/

# Option 2: Web application (when "frontend" + "backend" detected)
backend/
├── src/
│   ├── models/
│   ├── services/
│   └── api/
└── tests/

frontend/
├── src/
│   ├── components/
│   ├── pages/
│   └── services/
└── tests/

# Option 3: Mobile + API (when "iOS/Android" detected)
api/
└── [same as backend above]

ios/ or android/
└── [platform-specific structure]
```

**Structure Decision**: Option 1 (Single project) - Extending existing OwaspHeaders.Core library structure

## Phase 0: Outline & Research
1. **Extract unknowns from Technical Context** above:
   - For each NEEDS CLARIFICATION → research task
   - For each dependency → best practices task
   - For each integration → patterns task

2. **Generate and dispatch research agents**:
   ```
   For each unknown in Technical Context:
     Task: "Research {unknown} for {feature context}"
   For each technology choice:
     Task: "Find best practices for {tech} in {domain}"
   ```

3. **Consolidate findings** in `research.md` using format:
   - Decision: [what was chosen]
   - Rationale: [why chosen]
   - Alternatives considered: [what else evaluated]

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts
*Prerequisites: research.md complete*

1. **Extract entities from feature spec** → `data-model.md`:
   - Entity name, fields, relationships
   - Validation rules from requirements
   - State transitions if applicable

2. **Generate API contracts** from functional requirements:
   - For each user action → endpoint
   - Use standard REST/GraphQL patterns
   - Output OpenAPI/GraphQL schema to `/contracts/`

3. **Generate contract tests** from contracts:
   - One test file per endpoint
   - Assert request/response schemas
   - Tests must fail (no implementation yet)

4. **Extract test scenarios** from user stories:
   - Each story → integration test scenario
   - Quickstart test = story validation steps

5. **Update agent file incrementally** (O(1) operation):
   - Run `.specify/scripts/bash/update-agent-context.sh claude` for your AI assistant
   - If exists: Add only NEW tech from current plan
   - Preserve manual additions between markers
   - Update recent changes (keep last 3)
   - Keep under 150 lines for token efficiency
   - Output to repository root

**Output**: data-model.md, /contracts/*, failing tests, quickstart.md, agent-specific file

## Phase 2: Task Planning Approach
*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task Generation Strategy**:
- Load `.specify/templates/tasks-template.md` as base
- Generate tasks from Phase 1 design docs (contracts, data model, quickstart)
- ClearSiteDataOptions enum → enum creation task [P]
- ClearSiteDataConfiguration model → model creation task [P]
- ClearSiteDataPathConfiguration model → model creation task [P]
- Builder extension methods → builder implementation task [P]
- Middleware integration → middleware modification task
- Constants addition → constants update task [P]
- Each contract → comprehensive test task covering all scenarios
- Quickstart scenarios → integration test validation

**TDD Ordering Strategy**:
1. **Phase 2A - Test Infrastructure** [Tests before implementation]
   - Enum validation tests [P]
   - Configuration model tests [P]
   - Builder method tests [P]
   - Middleware integration tests [P]
   - Performance benchmark tests [P]

2. **Phase 2B - Core Implementation** [Make tests pass]
   - ClearSiteDataOptions enum implementation [P]
   - ClearSiteDataConfiguration model [P]
   - ClearSiteDataPathConfiguration model [P]
   - Constants update [P]
   - Builder extension methods implementation

3. **Phase 2C - Integration Implementation** [Sequential dependencies]
   - SecureHeadersMiddlewareConfiguration extension
   - SecureHeadersMiddleware modification
   - Path resolution logic integration

4. **Phase 2D - Validation & Documentation** [Final verification]
   - Integration test execution and validation
   - Performance validation against <1ms target
   - Quickstart guide validation
   - XML documentation completion

**Parallel Execution Markers**:
- [P] = Can be implemented in parallel (independent files)
- Sequential tasks clearly marked with dependencies
- Test tasks always precede their implementation counterparts

**Constitutional Compliance Tasks**:
- dotnet-format validation task
- EditorConfig compliance verification
- Security regression testing
- 90%+ test coverage validation for security-critical paths

**Estimated Output**: 28-32 numbered, ordered tasks in tasks.md

**Key Dependencies Identified**:
- Enum must exist before Configuration models
- Configuration models before Builder methods
- Builder methods before Middleware integration
- All components before Integration tests

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation
*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking
*Fill ONLY if Constitution Check has violations that must be justified*

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |


## Progress Tracking
*This checklist is updated during execution flow*

**Phase Status**:
- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:
- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved
- [x] Complexity deviations documented

---
*Based on Constitution v1.0.0 - See `.specify/memory/constitution.md`*
