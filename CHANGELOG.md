# [1.0.0-beta.17](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.16...v1.0.0-beta.17) (2026-05-21)


### Features

* allow registration of additional services in IServiceProvider ([#18](https://github.com/DIGITALLNature/DigitallTesting/issues/18)) ([4c23c38](https://github.com/DIGITALLNature/DigitallTesting/commit/4c23c38105aaf56de0bb0a74b867241154e3caac))

# [1.0.0-beta.16](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.15...v1.0.0-beta.16) (2026-05-21)


### Bug Fixes

* **WhoAmIFake:** use TryParse for env var GUIDs to handle invalid values ([3774a74](https://github.com/DIGITALLNature/DigitallTesting/commit/3774a74ee6bfad930ebd38802a22070a49f6c45e))

# [1.0.0-beta.15](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.14...v1.0.0-beta.15) (2026-05-20)


### Features

* add SpyOrganizationRequestFake ([a99caec](https://github.com/DIGITALLNature/DigitallTesting/commit/a99caec776cecf374b1f4939e9ecdf87335f725f))

# [1.0.0-beta.14](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.13...v1.0.0-beta.14) (2026-05-20)


### Bug Fixes

* resolve nullable reference type and code quality warnings ([a70cda2](https://github.com/DIGITALLNature/DigitallTesting/commit/a70cda2dfa012f62ff4ad05784cf08dc23697821))
* resolve Qodana style warnings ([910bda7](https://github.com/DIGITALLNature/DigitallTesting/commit/910bda7ae43d150d589896f0a683f1c559ed1262))
* resolve remaining mechanical Qodana warnings (Phase 4) ([251764a](https://github.com/DIGITALLNature/DigitallTesting/commit/251764aebb003edd36e14a372c515de0852697f9))
* resolve remaining Qodana warnings (Phase 3) ([aacec43](https://github.com/DIGITALLNature/DigitallTesting/commit/aacec4347c84539ddadddcab7fca0a40fb28cad6))


### Features

* migrate to TUnit + TUnit.Mocks + TUnit.Assertions ([942b90e](https://github.com/DIGITALLNature/DigitallTesting/commit/942b90e92e301061ccd5f23681788884b5b3a3be))


### Performance Improvements

* cache reflection lookups in EntityTypeResolver ([7d3ab89](https://github.com/DIGITALLNature/DigitallTesting/commit/7d3ab8976c8ee838bd8a9666a74b556df13c3e2a))

# [1.0.0-beta.13](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.12...v1.0.0-beta.13) (2026-04-29)


### Bug Fixes

* return orgservice in PluginExecutionContextBuilder when userid is null ([bcf818a](https://github.com/DIGITALLNature/DigitallTesting/commit/bcf818a5c47a42a9d7e23a4c8222669e9047ac5b))

# [1.0.0-beta.13](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.12...v1.0.0-beta.13) (2026-03-11)


### Features

* extract state from FakeOrganizationService into FakeOrganizationServiceState ([#12](https://github.com/DIGITALLNature/DigitallTesting/issues/12))

# [1.0.0-beta.12](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.11...v1.0.0-beta.12) (2026-03-11)


### Features

* add support for IOrganizationServiceAsync2 ([#11](https://github.com/DIGITALLNature/DigitallTesting/issues/11)) ([6ccdbfe](https://github.com/DIGITALLNature/DigitallTesting/commit/6ccdbfe185c1bc8da86c2382eafb85d48de47dba))

# [1.0.0-beta.11](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.10...v1.0.0-beta.11) (2026-03-11)


### Features

* support for RetrieveWithAlternateKey ([8d00ee4](https://github.com/DIGITALLNature/DigitallTesting/commit/8d00ee43a49ba3c98bbfbca7556472d6dcbaf699))

# [1.0.0-beta.10](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.9...v1.0.0-beta.10) (2026-03-11)


### Features

* add Fake for Retrieve ([b5e4335](https://github.com/DIGITALLNature/DigitallTesting/commit/b5e4335d6057845ae0d4ce17d55fddf3399a85f2))

# [1.0.0-beta.9](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.8...v1.0.0-beta.9) (2026-03-10)


### Features

* add TenantId to PluginExecutionContextBuilder ([f77f5c9](https://github.com/DIGITALLNature/DigitallTesting/commit/f77f5c9b801cddea45a161c239f07ade8adb3328))

# [1.0.0-beta.8](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.7...v1.0.0-beta.8) (2026-03-10)


### Features

* add Depth to PluginExecutionContextBuilder ([332cdf5](https://github.com/DIGITALLNature/DigitallTesting/commit/332cdf53b4ddbe01da66668407c4b222ddf217a1))

# [1.0.0-beta.7](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.6...v1.0.0-beta.7) (2026-03-10)


### Features

* for FakedDataverseBuild add method AddConfig ([da16128](https://github.com/DIGITALLNature/DigitallTesting/commit/da161282f2c35d82e55c23bdcc8d89e28b12bd2b))

# [1.0.0-beta.6](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.5...v1.0.0-beta.6) (2025-12-19)


### Features

* fake for RetrieveEntity ([72c0dd8](https://github.com/DIGITALLNature/DigitallTesting/commit/72c0dd86a508061c21bab3756bb4aeed6576409f))

# [1.0.0-beta.5](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.4...v1.0.0-beta.5) (2025-05-27)


### Bug Fixes

* add id attribute to intersect entity ([#10](https://github.com/DIGITALLNature/DigitallTesting/issues/10)) ([f477d4c](https://github.com/DIGITALLNature/DigitallTesting/commit/f477d4c26c1d4528a440e3d67ee9ff1a6b4cd4df))

# [1.0.0-beta.4](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.3...v1.0.0-beta.4) (2025-04-25)


### Features

* add builder methods for relationships & entity metadata ([#9](https://github.com/DIGITALLNature/DigitallTesting/issues/9)) ([104069b](https://github.com/DIGITALLNature/DigitallTesting/commit/104069b7ba29422ee8c80c29452b4b9eca97ff99))

# [1.0.0-beta.3](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.2...v1.0.0-beta.3) (2025-04-24)


### Bug Fixes

* wrong post entity images mock ([#8](https://github.com/DIGITALLNature/DigitallTesting/issues/8)) ([f694c4f](https://github.com/DIGITALLNature/DigitallTesting/commit/f694c4f6cdf646447d75f4109f185831607fc63f))

# [1.0.0-beta.2](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.1...v1.0.0-beta.2) (2025-04-22)


### Features

* allow TimeProvider in FakedDataverseBuilder ctors ([598d345](https://github.com/DIGITALLNature/DigitallTesting/commit/598d345271f5e336e71399fcd376657e8cd428b2))
* expose of FakeDataverse in FakeDataverseBuilder ([49d1178](https://github.com/DIGITALLNature/DigitallTesting/commit/49d117899734e24eef30dd5d36dc853e4669e17a))

# 1.0.0-beta.1 (2025-04-17)


### Features

* first release ([d4dc78c](https://github.com/DIGITALLNature/DigitallStub/commit/d4dc78c648c3f41fad2455db2e1c9a9fe9a240cd))
