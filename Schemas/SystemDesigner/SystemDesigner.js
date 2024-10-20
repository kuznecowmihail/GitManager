define("SystemDesigner", [
	"ServiceHelper",
	"SystemDesignerResources"
], function (ServiceHelper, resources) {
	return {
		methods: {
			/**
			 * @inheritdoc Terrasoft.BaseSchemaViewModel#init
			 * @overridden
			 */
			init: function(callback, scope) {
				this.callParent([function() {
					this.initCanAccessGitManager();
					
					Ext.callback(callback, scope);
				}, this]);
			},
			
			/**
			 * Инициализация параметра GetCanAccessGitManager
			 */
			initCanAccessGitManager: function (callback, scope) {
				scope = scope || this;
				ServiceHelper.callService(
					"GitService",
					"GetCanAccessGitManager",
					function(response) {
						const result = response.GetCanAccessGitManagerResult;
						
						if (result && !result.success && result.errorInfo && result.errorInfo.message) {
							this.showInformationDialog(result.errorInfo.message);
						}
						const value = result && Ext.isBoolean(result.success);
						scope.set("CanAccessGitManager", Boolean(value));
						
						if (callback) {
							Ext.callback(callback, scope);
						}
					},
					{},
					this
				);
			},
			
			/**
			 * Перейти в раздел "Доступы реозиториев Git"
			 */
			onNavigateToGitRepositoryAccessClick: function () {
				if (this.get("CanAccessGitManager") == null) {
					this.initGetCanAccessConfigurationSettings(function () {
						this.navigateToGitPackageRepository();
					}, this);
				} else {
					this.navigateToGitRepositoryAccess();
				}
				
				return false;
			},
			
			/**
			 * Перейти в раздел "Репозитории пакетов Git"
			 */
			onNavigateToGitPackageRepositoryClick: function () {
				if (this.get("CanAccessGitManager") == null) {
					this.initGetCanAccessConfigurationSettings(function () {
						this.navigateToGitPackageRepository();
					}, this);
				} else {
					this.navigateToGitPackageRepository();
				}
				
				return false;
			},

			/**
			 * Перейти в раздел "Доступы реозиториев Git"
			 */
			navigateToGitRepositoryAccess: function() {
				if (this.get("CanAccessGitManager") === true) {
					this.openSection("GitRepositoryAccessSection");
				} else {
					this.showPermissionsErrorMessage("CanManageSolution");
				}
			},

			/**
			 * Перейти в раздел "Репозитории пакетов Git"
			 */
			navigateToGitPackageRepository: function() {
				if (this.get("CanAccessGitManager") === true) {
					this.openSection("GitPackageRepositorySection");
				} else {
					this.showPermissionsErrorMessage("CanManageSolution");
				}
			},

			/**
			 * overridden
			 * @returns {Object}
			 */
			getOperationRightsDecoupling: function () {
				let result = this.callParent(arguments);
				result.onNavigateToGitPackageRepositoryClick = "CanManageAdministration";

				return result;
			}
		},
		diff: [
			{
				operation: "remove",
				name: "ConfigurationTile"
			},
			{
				operation: "insert",
				name: "GitTile",
				propertyName: "items",
				parentName: "LeftContainer",
				values: {
					itemType: Terrasoft.ViewItemType.CONTAINER,
					generator: "MainMenuTileGenerator.generateMainMenuTile",
					caption: {bindTo: "Resources.Strings.GitCaption"},
					cls: ["designer-tile"],
					icon: resources.localizableImages.GitIcon,
					visible: {bindTo: "CanAccessGitManager"},
					items: []
				}
			},
			{
				operation: "insert",
				propertyName: "items",
				parentName: "GitTile",
				name: "GitRepositoryAccess",
				values: {
					itemType: this.Terrasoft.ViewItemType.LINK,
					caption: { bindTo: "Resources.Strings.GitRepositoryAccessCaption" },
					tag: "onNavigateToGitRepositoryAccessClick",
					click: { bindTo: "invokeOperation" }
				}
			},
			{
				operation: "insert",
				propertyName: "items",
				parentName: "GitTile",
				name: "GitPackageRepository",
				values: {
					itemType: this.Terrasoft.ViewItemType.LINK,
					caption: { bindTo: "Resources.Strings.GitPackageRepositoryCaption" },
					tag: "onNavigateToGitPackageRepositoryClick",
					click: { bindTo: "invokeOperation" }
				}
			},
			{
				operation: "insert",
				name: "ConfigurationTile",
				propertyName: "items",
				parentName: "LeftContainer",
				values: {
					itemType: Terrasoft.ViewItemType.CONTAINER,
					generator: "MainMenuTileGenerator.generateMainMenuTile",
					caption: {bindTo: "Resources.Strings.ConfigurationCaption"},
					cls: ["configuration", "designer-tile"],
					icon: resources.localizableImages.ConfigurationIcon,
					items: []
				}
			},
			{
				operation: "insert",
				propertyName: "items",
				parentName: "ConfigurationTile",
				name: "ConfigurationLink",
				values: {
					itemType: Terrasoft.ViewItemType.LINK,
					caption: {bindTo: "Resources.Strings.ConfigurationLinkCaption"},
					click: {bindTo: "onNavigateToConfigurationSettingsClick"}
				}
			}
		]
	};
});