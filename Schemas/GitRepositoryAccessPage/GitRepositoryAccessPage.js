define("GitRepositoryAccessPage", [], function() {
	return {
		entitySchemaName: "GitRepositoryAccess",
		attributes: {},
		modules: /**SCHEMA_MODULES*/{}/**SCHEMA_MODULES*/,
		details: /**SCHEMA_DETAILS*/{
			"Files": {
				"schemaName": "FileDetailV2",
				"entitySchemaName": "GitRepositoryAccessFile",
				"filter": {
					"masterColumn": "Id",
					"detailColumn": "GitRepositoryAccess"
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
				
				this.on("change:GitHostname", this.onGitHostnameChanged, this);
			},
			
			/**
			 * @inheritdoc Terrasoft.BaseSchemaViewModel#destroy
			 * @overridden
			 */
			destroy: function () {
				this.callParent(arguments);
				
				this.un("change:GitHostname", this.onGitHostnameChanged, this);
			},
			
			/**
			 * Событие изменения "Домен"
			 */
			onGitHostnameChanged: function () {
				if (!this.$IsEntityInitialized || !this.$GitHostname) {
					return;
				}
				const scope = this;
				
				// Синхронно еще не закончился сценарий изменения поля, новое изменение инициируемой асинхронно
				Terrasoft.delay(() => {
					let newValue = null;
					let matches = this.$GitHostname.match(/^(?:https?:\/\/)?(?:www\.)?([^\/]+)/i);
					
					if (!matches || matches.length !== 2 || matches[0] === matches[1]) {
						this.showInformationDialog(this.get("Resources.Strings.IncorrectGitHostnameError"));
					} else {
						newValue = matches[0];
					}
					
					if (this.$GitHostname === newValue) {
						return;
					}
					this.un("change:GitHostname", this.onGitHostnameChanged, this);
					this.$GitHostname = newValue;
					this.on("change:GitHostname", this.onGitHostnameChanged, this);
				});
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
						"layoutName": "Header"
					},
					"bindTo": "GitName"
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "GitUsername",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 2,
						"layoutName": "Header"
					},
					"bindTo": "GitUsername",
					"enabled": true
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "GitPassword",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 3,
						"layoutName": "Header"
					},
					"bindTo": "GitPassword",
					"tip": {
						"content": {
							"bindTo": "Resources.Strings.GitPasswordTip"
						}
					},
					"enabled": true,
					"controlConfig": {
						"protect": true
					}
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "GitHostname",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "Header"
					},
					"bindTo": "GitHostname",
					"enabled": true
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "insert",
				"name": "GitEmail",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 12,
						"row": 2,
						"layoutName": "Header"
					},
					"bindTo": "GitEmail",
					"enabled": true
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 4
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
