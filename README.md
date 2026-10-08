# DotNetTestingWorkshop

This repository is part of a test automation lecture. It starts with basic code, then adds functionality and introduces tests and concepts.

Each phase is a tag. To see a phase, check it out – for example `git switch --detach phase-1` – or compare two phases with `git diff phase-1 phase-2`.

## phase-1

- Basic ASP.NET Core project
- Bank account management functionality

## phase-2

- Basic unit testing (and a discovered bug!)

## phase-3

- Fix the bug found in phase 2
- Test multiple values using `[Theory]` with `[InlineData]` and `[MemberData]`
- A test that finds a missing requirement (what happens on overflow?)
- An assertion that tells you nothing

## phase-4

- Replace xUnit's `Assert` with Awesome Assertions (readable assertions, readable failure messages)

## phase-5

- Unit tests for `AccountService`
- Shared setup: from repeated Arrange, through constructor + `IDisposable`, to a `CreateSut()` helper
