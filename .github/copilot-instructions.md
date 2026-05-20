# Project Guidelines for GitHub Copilot

## Documentation Maintenance (MANDATORY)

When making changes to this codebase, **you MUST keep the documentation up to date**. This is not optional.

### Rules

1. **README.md must reflect the current state of the project.** After any change that affects the public API, architecture, configuration, project structure, or usage patterns, update the corresponding section(s) in `README.md`.

2. **What requires a README update:**
   - Adding, removing, or renaming public classes, interfaces, or methods
   - Adding or removing NuGet dependencies
   - Adding new Organization Request fakes
   - Changing configuration/environment variables
   - Modifying the builder APIs or fluent extensions
   - Adding new folders or restructuring the project
   - Changing build/test commands or CI/CD workflows
   - Adding new features or capabilities

3. **What does NOT require a README update:**
   - Internal refactoring that doesn't change the public API
   - Bug fixes that don't change behavior or usage
   - Test-only changes
   - Code style / formatting changes

4. **CHANGELOG.md is auto-generated** by semantic-release. Do NOT edit it manually.

5. **Keep documentation in English.** All documentation in this repository is written in English.

### Documentation Style

- Use concise, technical language
- Include code examples for new public APIs
- Keep the table of contents in sync with the actual sections
- Use tables for listing related items (request fakes, config vars, etc.)
- Architecture diagrams use ASCII art (no external dependencies)

---

## RTK — Token-Optimized CLI

**rtk** is a CLI proxy that filters and compresses command outputs, saving 60-90% tokens.

## Rule

Always prefix shell commands with `rtk`:

```bash
# Instead of:              Use:
git status                 rtk git status
git log -10                rtk git log -10
cargo test                 rtk cargo test
docker ps                  rtk docker ps
kubectl get pods           rtk kubectl pods
```

## Meta commands (use directly)

```bash
rtk gain              # Token savings dashboard
rtk gain --history    # Per-command savings history
rtk discover          # Find missed rtk opportunities
rtk proxy <cmd>       # Run raw (no filtering) but track usage
```
