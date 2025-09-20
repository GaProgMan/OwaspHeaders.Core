<!--
# Sync Impact Report
Version change: N/A → 1.0.0 (initial constitution)
Modified principles: None (initial version)
Added sections: All sections (initial constitution)
Removed sections: None
Templates requiring updates:
  ✅ .specify/templates/plan-template.md - updated Constitution Check section with specific OwaspHeaders.Core principles
  ✅ .specify/templates/spec-template.md - no changes needed (no constitution references)
  ✅ .specify/templates/tasks-template.md - no changes needed (no constitution references)
  ✅ No agent-specific guidance files found - none to update
Follow-up TODOs: None - all placeholders resolved and templates updated
-->

# OwaspHeaders.Core Constitution

## Core Principles

### I. Security-First Design
Security considerations MUST be the primary driver of all architectural decisions; Every feature MUST be evaluated against OWASP security guidelines and threat models; NO functionality SHALL be implemented that weakens the security posture of consuming applications; Security headers implementation MUST follow current OWASP Secure Headers Project recommendations exactly.

**Rationale**: As an OWASP-aligned security middleware, the project's core mission is enhancing web application security. Any compromise on security standards defeats the fundamental purpose and undermines user trust.

### II. Library-First Architecture
Every component MUST be designed as a standalone, reusable library; Components SHALL be independently testable without external dependencies; Clear separation of concerns required - no organizational-only abstractions; Each module MUST have a single, well-defined purpose that can be documented in one sentence.

**Rationale**: Middleware libraries require modularity for maintainability and testing. Tight coupling reduces reusability and makes security auditing more difficult.

### III. Test-Driven Development (NON-NEGOTIABLE)
Tests MUST be written before implementation for all new features and bug fixes; Red-Green-Refactor cycle strictly enforced - tests must fail before implementation begins; Code coverage MUST maintain 90%+ for security-critical paths; Integration tests required for all middleware configuration scenarios.

**Rationale**: Security middleware demands the highest confidence in correctness. TDD ensures all security behaviors are explicitly verified and prevents regression of security features.

### IV. Code Quality and Formatting Standards
ALL code MUST pass dotnet-format validation without warnings; EditorConfig rules are NON-NEGOTIABLE and enforced in CI/CD; Primary constructors SHALL NOT be used in example projects due to formatting tool limitations; Code review required for any deviation from established formatting standards.

**Rationale**: Consistent formatting reduces cognitive load during security reviews and ensures professional code quality. The primary constructor restriction prevents CI/CD pipeline failures while maintaining code clarity.

### V. Documentation and Compliance Excellence
Security implications MUST be documented for every public API; Comprehensive examples required for all configuration options; OpenSSF Best Practices compliance maintained at all times; Attestations generated for all releases to ensure supply chain security; All security headers MUST include rationale and browser compatibility notes.

**Rationale**: Security middleware users need complete understanding of implications. Poor documentation leads to misconfigurations that compromise security. Supply chain security is critical for security-focused libraries.

## Security Requirements

Security headers implementation SHALL follow OWASP Secure Headers Project specifications exactly; NO custom security headers without documented security analysis; Default configurations MUST provide maximum security while maintaining broad compatibility; Security-sensitive configuration changes require security team review; Vulnerability disclosure process must be clearly documented and followed.

## Development Workflow

ALL changes require pull request review with security focus; Breaking changes require migration documentation and version bump; Performance impact analysis required for middleware changes; Compatibility testing across supported .NET versions mandatory; Security regression testing on every commit.

## Governance

Constitution supersedes all other development practices and guidelines; Amendments require documentation of security impact, team approval, and migration plan for existing code; Complexity that violates these principles must be justified with specific security or compatibility requirements; ALL pull requests and code reviews must verify compliance with these constitutional principles.

Security violations are never acceptable and must be addressed immediately regardless of timeline pressures; Use the latest CLAUDE.md file for runtime development guidance and context-specific instructions.

**Version**: 1.0.0 | **Ratified**: 2025-09-20 | **Last Amended**: 2025-09-20