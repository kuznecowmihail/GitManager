define("GitPackageRepositoryPage", [], function() {
	return {
		entitySchemaName: "GitPackageRepository",
		attributes: {
			GitRepositoryAccess: {
				lookupListConfig: {
					filter: function () {
							const filterGroup = new this.Terrasoft.createFilterGroup();
						
							filterGroup.add("Hostname", this.Terrasoft.createColumnFilterWithParameter(
							    this.Terrasoft.ComparisonType.EQUAL,
							    "GitHostname", 
							    this.getHostnameGromRemoteUrl()));
						
							return filterGroup;
					}
				}
			}
		},
		modules: /**SCHEMA_MODULES*/{}/**SCHEMA_MODULES*/,
		details: /**SCHEMA_DETAILS*/{
			"Files": {
				"schemaName": "FileDetailV2",
				"entitySchemaName": "GitPackageRepositoryFile",
				"filter": {
					"masterColumn": "Id",
					"detailColumn": "GitPackageRepository"
				}
			}
		}/**SCHEMA_DETAILS*/,
		businessRules: /**SCHEMA_BUSINESS_RULES*/{}/**SCHEMA_BUSINESS_RULES*/,
		methods: {
			/**
			 * @inheritdoc Terrasoft.BaseSchemaViewModel#init
			 * @overridden
			 */
			init: function (callback, scope) {
				this.callParent(arguments);
				
				this.on("change:GitRemoteUrl", this.onGitRemoteUrlChanged, this);
			},
			
			/**
			 * @inheritdoc Terrasoft.BaseSchemaViewModel#destroy
			 * @overridden
			 */
			destroy: function () {
				this.callParent(arguments);
				
				this.un("change:GitRemoteUrl", this.onGitRemoteUrlChanged, this);
			},
			
			/**
			 * Событие изменения "Ссылка на удаленный репозиторий"
			 */
			onGitRemoteUrlChanged: function () {
				if (!this.$IsEntityInitialized || !this.$GitRemoteUrl) {
					return;
				}
				const scope = this;
				
				// Синхронно еще не закончился сценарий изменения поля, новое изменение инициируемой асинхронно
				Terrasoft.delay(() => {
					let newValue = this.getHostnameGromRemoteUrl();
					
					if (newValue) {
						newValue = this.$GitRemoteUrl;
					}
					
					if (this.$GitRemoteUrl === newValue) {
						return;
					}
					this.un("change:GitHostname", this.onGitRemoteUrlChanged, this);
					this.$GitRemoteUrl = newValue;
					this.on("change:GitHostname", this.onGitRemoteUrlChanged, this);
				});
			},
			
			/**
			 * Получить значение Домена по ссылке на удаленный репозиторий
			 */
			getHostnameGromRemoteUrl: function () {
				if (!this.$GitRemoteUrl) {
					return null;
				}
				let matches = this.$GitRemoteUrl.match(/^(?:https?:\/\/)?(?:www\.)?([^\/]+)/i);
				
				if (!matches || matches.length !== 2 || matches[0] === matches[1]) {
					this.showInformationDialog(this.get("Resources.Strings.IncorrectGitRemoteUrlError"));
					
					return null;
				} else {
					return matches[0];
				}
			}
		},
		dataModels: /**SCHEMA_DATA_MODELS*/{}/**SCHEMA_DATA_MODELS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"name": "GitName",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "ProfileContainer"
					},
					"bindTo": "GitName",
					"enabled": { "bindTo": "isAddMode" }
				},
				"parentName": "ProfileContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GitRemoteUrl",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "ProfileContainer"
					},
					"bindTo": "GitRemoteUrl",
					"enabled": { "bindTo": "isAddMode" }
				},
				"parentName": "ProfileContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GitRepositoryAccess",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "ProfileContainer"
					},
					"bindTo": "GitRepositoryAccess"
				},
				"parentName": "ProfileContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GitSysPackage",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 2,
						"layoutName": "ProfileContainer"
					},
					"bindTo": "GitSysPackage",
					"enabled": { "bindTo": "isAddMode" },
					"tip": {
						"content": {
							"bindTo": "Resources.Strings.GitSysPackageTip"
						}
					}
				},
				"parentName": "ProfileContainer",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "NotesAndFilesTab",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.NotesAndFilesTabCaption"
					},
					"items": [],
					"order": 0
				},
				"parentName": "Tabs",
				"propertyName": "tabs",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Files",
				"values": {
					"itemType": 2
				},
				"parentName": "NotesAndFilesTab",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NotesControlGroup",
				"values": {
					"itemType": 15,
					"caption": {
						"bindTo": "Resources.Strings.NotesGroupCaption"
					},
					"items": []
				},
				"parentName": "NotesAndFilesTab",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "Notes",
				"values": {
					"bindTo": "GitNotes",
					"dataValueType": 1,
					"contentType": 4,
					"layout": {
						"column": 0,
						"row": 0,
						"colSpan": 24
					},
					"labelConfig": {
						"visible": false
					},
					"controlConfig": {
						"imageLoaded": {
							"bindTo": "insertImagesToNotes"
						},
						"images": {
							"bindTo": "NotesImagesCollection"
						}
					}
				},
				"parentName": "NotesControlGroup",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "merge",
				"name": "ESNTab",
				"values": {
					"order": 1
				}
			}
		]/**SCHEMA_DIFF*/
	};
});