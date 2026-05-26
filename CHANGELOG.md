# [1.1.0-beta.7](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.1.0-beta.6...v1.1.0-beta.7) (2026-05-26)


### Bug Fixes

* **bulk-delete:** copy RecurrencePattern and StartDateTime to asyncoperation ([97d08d0](https://github.com/DIGITALLNature/DigitallTesting/commit/97d08d0fbbc71042a9111b67e4ef7ec61675d6bd))
* **bulk-delete:** omit recurrencepattern and recurrencestarttime when not set ([665a695](https://github.com/DIGITALLNature/DigitallTesting/commit/665a6957895b978636af3159e4fb7064af9d69b2))
* **query:** fix 4 critical query engine bugs ([9bcbdc2](https://github.com/DIGITALLNature/DigitallTesting/commit/9bcbdc2c08eae3452a32a28bdbcf110d8510be70))
* remove 14 redundant using directives ([66f139b](https://github.com/DIGITALLNature/DigitallTesting/commit/66f139ba136c5790ab626483a546eb1b98d8d6e6))
* remove 5 unused local variables ([9ef79de](https://github.com/DIGITALLNature/DigitallTesting/commit/9ef79def34d0e0063e2a673cea8f079f9e2d037c))
* remove redundant System.Reflection qualifier ([c11bd75](https://github.com/DIGITALLNature/DigitallTesting/commit/c11bd75c0819ba424c580f622ac5b796c5903cfd))
* replace obsolete HasCount() with Count().IsEqualTo() ([24d9719](https://github.com/DIGITALLNature/DigitallTesting/commit/24d971977f4189a131015debf78f0a9a5d0c8ee9))
* resolve ambiguous XML doc comment reference ([f3a3928](https://github.com/DIGITALLNature/DigitallTesting/commit/f3a392894253c3bee0676826696c5ebd6c2cb65b))
* use Count property instead of Count() method ([1577b52](https://github.com/DIGITALLNature/DigitallTesting/commit/1577b52b7d5ba3bf21fc74ba0749f443bfd79b42))
* validate relationship metadata matches parent entity in DeepInsertProcessor ([fdafece](https://github.com/DIGITALLNature/DigitallTesting/commit/fdafeceb6746ccdb6f26b39bbbdbfd2d2d90d3a3))


### Features

* **query:** implement CompareColumns and MatchFirstRowUsingCrossApply ([7376e02](https://github.com/DIGITALLNature/DigitallTesting/commit/7376e02199f43a8ba21f9c399218bd6e8e387fdf))
* **query:** support FilterExpression.AnyAllFilterLinkEntity and JoinOperator.All/NotAll ([628fd01](https://github.com/DIGITALLNature/DigitallTesting/commit/628fd0162ed13b20bde105d4965272e1c1510c32))
* support deep insert (RelatedEntities) in CreateFake and UpsertFake ([07ef938](https://github.com/DIGITALLNature/DigitallTesting/commit/07ef938cfd67a5678bc0db1bb02d722ff47cb708))

# [1.1.0-beta.6](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.1.0-beta.5...v1.1.0-beta.6) (2026-05-22)


### Bug Fixes

* **bulk-delete:** populate name, ownerid, and operationtype on asyncoperation entity ([3843fba](https://github.com/DIGITALLNature/DigitallTesting/commit/3843fbaae960f1d4cf72f188b83ef22eab9f0718))
* **query:** validate actual values in aggregate FetchXml guards ([ef96352](https://github.com/DIGITALLNature/DigitallTesting/commit/ef96352c84579d3eca8ae18c74062a75e8cb2b18))
* replace generic exceptions with Dataverse-style FaultException ([eadc251](https://github.com/DIGITALLNature/DigitallTesting/commit/eadc251923baf06a1206e5641f16f653ad2a55ad))
* **retrieve:** translate QueryByAttribute Attributes/Values and Orders in RetrieveFake ([2b1ad37](https://github.com/DIGITALLNature/DigitallTesting/commit/2b1ad37f47e888c9579262f3e1521a7f8375cbdb))


### Features

* add RelatedEntitiesQuery support to RetrieveFake ([ce49f8a](https://github.com/DIGITALLNature/DigitallTesting/commit/ce49f8adbef7172662987498e2cca2fa000ebb79))
* **query:** support EXISTS-style semi-joins via JoinOperator.Any/NotAny/Exists ([79fe312](https://github.com/DIGITALLNature/DigitallTesting/commit/79fe3126ff54174ba5c4eeb1e7399e4c3a17bae2))
* route Create/Update/Delete/Retrieve/Associate/Disassociate through Execute pipeline ([a9346b1](https://github.com/DIGITALLNature/DigitallTesting/commit/a9346b15ea077a795e2c08d46a2423c1ee8bb8e8))

# [1.1.0-beta.5](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.1.0-beta.4...v1.1.0-beta.5) (2026-05-22)


### Bug Fixes

* merge attributes on Update instead of replacing the entire entity ([68dab94](https://github.com/DIGITALLNature/DigitallTesting/commit/68dab943354b841e068fd374b0c6682a20ca9153))
* **query:** pass Guid directly for EqualUserId/EqualBusinessId conditions ([ccc49fa](https://github.com/DIGITALLNature/DigitallTesting/commit/ccc49fa75f135953c06652fa24c237a66348f259))
* **query:** use fiscalYear from condition as year for InFiscalYear date range ([dcbd230](https://github.com/DIGITALLNature/DigitallTesting/commit/dcbd2308193c77a1e6deeb1f88ab4cba47fd9651))
* **query:** use TimeOnly.MaxValue for end-of-day upper bound in date range operators ([93f273f](https://github.com/DIGITALLNature/DigitallTesting/commit/93f273f2fe44b6c41ebdd7d83369424b38edb910))
* skip ownerid default when proxy type has no ownerid attribute ([6118674](https://github.com/DIGITALLNature/DigitallTesting/commit/611867400290b2ae6b0e00f479f4d6c22744fc7a))
* validate maxRetrieveCount argument in WithMaxRetrieveCount ([fdd5417](https://github.com/DIGITALLNature/DigitallTesting/commit/fdd5417d589418c102d89ce27d23d101be15973b))


### Features

* set audit fields and RowVersion on Create and Update ([13212f4](https://github.com/DIGITALLNature/DigitallTesting/commit/13212f43e04bb55cb65bcbcf55a5197e6c288917))
* use Options class for explicit configuration ([a7d9497](https://github.com/DIGITALLNature/DigitallTesting/commit/a7d949776b7e76ca8fcec3486e108826aec217c5))

# [1.1.0-beta.4](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.1.0-beta.3...v1.1.0-beta.4) (2026-05-22)


### Bug Fixes

* deep-clone OptionSetValue instances inside OptionSetValueCollection ([8620cca](https://github.com/DIGITALLNature/DigitallTesting/commit/8620cca484e63f69462c69dbacd530b771ec2162))
* ensure CloneEntity returns base Entity runtime type ([78d9383](https://github.com/DIGITALLNature/DigitallTesting/commit/78d9383e7d7513742d04fc8d7d66f612d17ecc8b))
* fall back to EntityLogicalNameAttribute in CreateQuery<T> when type is not in resolver cache ([d2b7350](https://github.com/DIGITALLNature/DigitallTesting/commit/d2b73503cd792efe460e3c5d05bfe4279982dadc))
* **query:** clone outer entity per row in LeftOuter join ([26ba3a0](https://github.com/DIGITALLNature/DigitallTesting/commit/26ba3a0604d391c363c5ccd8419c5291100f7590))
* replace Activator.CreateInstance with plain Entity in ProjectAttributes ([1567b6b](https://github.com/DIGITALLNature/DigitallTesting/commit/1567b6bae4c325756632bce265471bfc2d97b839))


### Performance Improvements

* cache EntityLogicalNameAttribute lookup in CreateQuery<T> ([11c7fb6](https://github.com/DIGITALLNature/DigitallTesting/commit/11c7fb618bbd995ced0132148a532e94a0342646))
* cache MethodInfo lookups in ConditionParser as static readonly fields ([863b8a2](https://github.com/DIGITALLNature/DigitallTesting/commit/863b8a235e1393b4c929a9b251c0a13fb5399b99))
* compile proxy converter delegates via expression trees ([14cd064](https://github.com/DIGITALLNature/DigitallTesting/commit/14cd064ea2653ec3a2b09024f58f8407174142d6))
* use pattern matching in XrmOrderByAttributeComparer ([f078747](https://github.com/DIGITALLNature/DigitallTesting/commit/f07874744a67ad8b7b42806c89d0a87a093cfbdb))

# [1.1.0-beta.3](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.1.0-beta.2...v1.1.0-beta.3) (2026-05-22)


### Bug Fixes

* **query:** guard against null Attributes array in PatchDateFormat ([fdefc11](https://github.com/DIGITALLNature/DigitallTesting/commit/fdefc11fcc7903fcaf926b7523fe9335d44f6312))
* **query:** handle null otherEntity in JoinAttributes for left outer joins ([1a58675](https://github.com/DIGITALLNature/DigitallTesting/commit/1a58675019e9be299fafd2589a1213690728677a))
* **query:** materialize DateTime attributes before modifying collection in PatchDateFormat ([9b9aa2d](https://github.com/DIGITALLNature/DigitallTesting/commit/9b9aa2d98f13cb71e0001226bdf4ff0ee8ccd6ea))

# [1.1.0-beta.2](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.1.0-beta.1...v1.1.0-beta.2) (2026-05-22)


### Bug Fixes

* **query:** match primary id attribute against Entity.Id in RetrieveMultiple ([a66b55f](https://github.com/DIGITALLNature/DigitallTesting/commit/a66b55f8fbb1ada6b174a0d62b784f7dc8315348))

# [1.1.0-beta.1](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0...v1.1.0-beta.1) (2026-05-22)


### Bug Fixes

* **query:** treat fetchxml bool condition values without metadata as bool ([fe18d0c](https://github.com/DIGITALLNature/DigitallTesting/commit/fe18d0c814e5bc6e27639f78e48903f0a5cb55e2))


### Features

* add FetchXmlToQueryExpression organization request fake ([de7cd6f](https://github.com/DIGITALLNature/DigitallTesting/commit/de7cd6f2e9b88b0dc071df8f3b35613f590a87ae))
* add QueryExpressionToFetchXml organization request fake ([a18c9b3](https://github.com/DIGITALLNature/DigitallTesting/commit/a18c9b3d20d056a2583aed63e7f93f95839036da))
* add RetrieveAllEntities organization request fake ([f826094](https://github.com/DIGITALLNature/DigitallTesting/commit/f82609456c9652636204e25ac9a9df3accb85260))

# 1.0.0 (2026-05-21)


* feat!: remove deprecated state proxy properties from FakeOrganizationService ([cdfef89](https://github.com/DIGITALLNature/DigitallTesting/commit/cdfef892ab5161b2f295bad826a3c7acdac3f0f9))


### Bug Fixes

* add id attribute to intersect entity ([#10](https://github.com/DIGITALLNature/DigitallTesting/issues/10)) ([f477d4c](https://github.com/DIGITALLNature/DigitallTesting/commit/f477d4c26c1d4528a440e3d67ee9ff1a6b4cd4df))
* **builder:** make UserId settable agnostically without mutating env variable ([a01cbd6](https://github.com/DIGITALLNature/DigitallTesting/commit/a01cbd65ef5c9f76d1a44464ff6239706bd078d0))
* correct typo getNonBasibuteValueExpr -> getNonBasicValueExpr in ConditionParser ([038354c](https://github.com/DIGITALLNature/DigitallTesting/commit/038354cc2c5748186a8adc6555056964cdeae28c))
* remove redundant code ([81f5d4e](https://github.com/DIGITALLNature/DigitallTesting/commit/81f5d4e60101b3e83d61a3f3de27cf4ac963dbc5))
* resolve nullable reference type and code quality warnings ([a70cda2](https://github.com/DIGITALLNature/DigitallTesting/commit/a70cda2dfa012f62ff4ad05784cf08dc23697821))
* resolve Qodana quality and performance findings ([4ebe327](https://github.com/DIGITALLNature/DigitallTesting/commit/4ebe327e074d9cd9dccb456a767c871180dc3820))
* resolve Qodana style warnings ([910bda7](https://github.com/DIGITALLNature/DigitallTesting/commit/910bda7ae43d150d589896f0a683f1c559ed1262))
* resolve remaining mechanical Qodana warnings (Phase 4) ([251764a](https://github.com/DIGITALLNature/DigitallTesting/commit/251764aebb003edd36e14a372c515de0852697f9))
* resolve remaining Qodana warnings (Phase 3) ([aacec43](https://github.com/DIGITALLNature/DigitallTesting/commit/aacec4347c84539ddadddcab7fca0a40fb28cad6))
* return orgservice in PluginExecutionContextBuilder when userid is null ([bcf818a](https://github.com/DIGITALLNature/DigitallTesting/commit/bcf818a5c47a42a9d7e23a4c8222669e9047ac5b))
* **WhoAmIFake:** use TryParse for env var GUIDs to handle invalid values ([3774a74](https://github.com/DIGITALLNature/DigitallTesting/commit/3774a74ee6bfad930ebd38802a22070a49f6c45e))
* wrong post entity images mock ([#8](https://github.com/DIGITALLNature/DigitallTesting/issues/8)) ([f694c4f](https://github.com/DIGITALLNature/DigitallTesting/commit/f694c4f6cdf646447d75f4109f185831607fc63f))


### Features

* add builder methods for relationships & entity metadata ([#9](https://github.com/DIGITALLNature/DigitallTesting/issues/9)) ([104069b](https://github.com/DIGITALLNature/DigitallTesting/commit/104069b7ba29422ee8c80c29452b4b9eca97ff99))
* add Depth to PluginExecutionContextBuilder ([332cdf5](https://github.com/DIGITALLNature/DigitallTesting/commit/332cdf53b4ddbe01da66668407c4b222ddf217a1))
* add Fake for Retrieve ([b5e4335](https://github.com/DIGITALLNature/DigitallTesting/commit/b5e4335d6057845ae0d4ce17d55fddf3399a85f2))
* add SpyOrganizationRequestFake ([a99caec](https://github.com/DIGITALLNature/DigitallTesting/commit/a99caec776cecf374b1f4939e9ecdf87335f725f))
* add support for IOrganizationServiceAsync2 ([#11](https://github.com/DIGITALLNature/DigitallTesting/issues/11)) ([6ccdbfe](https://github.com/DIGITALLNature/DigitallTesting/commit/6ccdbfe185c1bc8da86c2382eafb85d48de47dba))
* add TenantId to PluginExecutionContextBuilder ([f77f5c9](https://github.com/DIGITALLNature/DigitallTesting/commit/f77f5c9b801cddea45a161c239f07ade8adb3328))
* add WithTenantId, WithDepth and WithTracingService builder extensions ([426fc1a](https://github.com/DIGITALLNature/DigitallTesting/commit/426fc1a90172330cb62d9dac1ac8a2134a25f186))
* allow registration of additional services in IServiceProvider ([#18](https://github.com/DIGITALLNature/DigitallTesting/issues/18)) ([4c23c38](https://github.com/DIGITALLNature/DigitallTesting/commit/4c23c38105aaf56de0bb0a74b867241154e3caac))
* allow TimeProvider in FakedDataverseBuilder ctors ([598d345](https://github.com/DIGITALLNature/DigitallTesting/commit/598d345271f5e336e71399fcd376657e8cd428b2))
* expose of FakeDataverse in FakeDataverseBuilder ([49d1178](https://github.com/DIGITALLNature/DigitallTesting/commit/49d117899734e24eef30dd5d36dc853e4669e17a))
* fake for RetrieveEntity ([72c0dd8](https://github.com/DIGITALLNature/DigitallTesting/commit/72c0dd86a508061c21bab3756bb4aeed6576409f))
* first release ([d4dc78c](https://github.com/DIGITALLNature/DigitallTesting/commit/d4dc78c648c3f41fad2455db2e1c9a9fe9a240cd))
* for FakedDataverseBuild add method AddConfig ([da16128](https://github.com/DIGITALLNature/DigitallTesting/commit/da161282f2c35d82e55c23bdcc8d89e28b12bd2b))
* migrate to TUnit + TUnit.Mocks + TUnit.Assertions ([942b90e](https://github.com/DIGITALLNature/DigitallTesting/commit/942b90e92e301061ccd5f23681788884b5b3a3be))
* set default state on create ([#19](https://github.com/DIGITALLNature/DigitallTesting/issues/19)) ([30f7daa](https://github.com/DIGITALLNature/DigitallTesting/commit/30f7daa797ee4ab273b34bad75d11c567844b86b)), closes [#18](https://github.com/DIGITALLNature/DigitallTesting/issues/18)
* support for RetrieveWithAlternateKey ([8d00ee4](https://github.com/DIGITALLNature/DigitallTesting/commit/8d00ee43a49ba3c98bbfbca7556472d6dcbaf699))


### Performance Improvements

* cache reflection lookups in EntityTypeResolver ([7d3ab89](https://github.com/DIGITALLNature/DigitallTesting/commit/7d3ab8976c8ee838bd8a9666a74b556df13c3e2a))


### BREAKING CHANGES

* use State.ModelAssemblies, State.EntityMetadata and
State.Relationships instead of the removed proxy properties.

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>

# [1.0.0-beta.20](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.19...v1.0.0-beta.20) (2026-05-21)


### Bug Fixes

* remove redundant code ([81f5d4e](https://github.com/DIGITALLNature/DigitallTesting/commit/81f5d4e60101b3e83d61a3f3de27cf4ac963dbc5))

# [1.0.0-beta.19](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.18...v1.0.0-beta.19) (2026-05-21)


### Bug Fixes

* **builder:** make UserId settable agnostically without mutating env variable ([a01cbd6](https://github.com/DIGITALLNature/DigitallTesting/commit/a01cbd65ef5c9f76d1a44464ff6239706bd078d0))

# [1.0.0-beta.18](https://github.com/DIGITALLNature/DigitallTesting/compare/v1.0.0-beta.17...v1.0.0-beta.18) (2026-05-21)


* feat!: remove deprecated state proxy properties from FakeOrganizationService ([cdfef89](https://github.com/DIGITALLNature/DigitallTesting/commit/cdfef892ab5161b2f295bad826a3c7acdac3f0f9))


### Bug Fixes

* correct typo getNonBasibuteValueExpr -> getNonBasicValueExpr in ConditionParser ([038354c](https://github.com/DIGITALLNature/DigitallTesting/commit/038354cc2c5748186a8adc6555056964cdeae28c))
* resolve Qodana quality and performance findings ([4ebe327](https://github.com/DIGITALLNature/DigitallTesting/commit/4ebe327e074d9cd9dccb456a767c871180dc3820))


### Features

* add WithTenantId, WithDepth and WithTracingService builder extensions ([426fc1a](https://github.com/DIGITALLNature/DigitallTesting/commit/426fc1a90172330cb62d9dac1ac8a2134a25f186))
* set default state on create ([#19](https://github.com/DIGITALLNature/DigitallTesting/issues/19)) ([30f7daa](https://github.com/DIGITALLNature/DigitallTesting/commit/30f7daa797ee4ab273b34bad75d11c567844b86b)), closes [#18](https://github.com/DIGITALLNature/DigitallTesting/issues/18)


### BREAKING CHANGES

* use State.ModelAssemblies, State.EntityMetadata and
State.Relationships instead of the removed proxy properties.

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>

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
