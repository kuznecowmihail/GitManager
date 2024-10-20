using GitManager.Extensions;
using GitManager.Helpers.Git;
using System;
using System.Linq;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace GitManager.Helpers.EntityHelpers
{
    /// <summary>
    /// Хелпер "Репозиторий пакета Git"
    /// </summary>
    internal class GitPackageRepositoryHelper : BaseHelper
    {
        /// <summary>
        /// Сущность записи
        /// </summary>
        Entity Entity { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userConnection"></param>
        /// <param name="entity"></param>
        public GitPackageRepositoryHelper(UserConnection userConnection, Entity entity) : base(userConnection)
        {
            Entity = entity;
            Argument.NotNull(Entity, nameof(Entity));
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userConnection"> Подключение пользователя </param>
        /// <param name="id"> Id записи </param>
        public GitPackageRepositoryHelper(UserConnection userConnection, Guid id) : base(userConnection)
        {
            Entity = GetEntity(userConnection, id);
            Argument.NotNull(Entity, nameof(Entity));
        }

        /// <summary>
        /// Событие после добавления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnInserting(object sender, EntityBeforeEventArgs e)
        {
            InitRepository();
        }

        /// <summary>
        /// Событие после добавления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnSaving(object sender, EntityBeforeEventArgs e)
        {
            var entity = (Entity)sender;

            var lockedById = entity.GetTypedColumnValue<Guid>("GitLockedById");

            if (lockedById != Guid.Empty)
            {
                return;
            }

            if (lockedById == Guid.Empty && entity.GetChangedColumnValues().Any(column => column.Name.Equals("GitLockedById")))
            {
                return;
            }
        }

        /// <summary>
        /// Получить сущность записи
        /// </summary>
        /// <param name="id"> Id </param>
        /// <returns> Сущность записи </returns>
        public static Entity GetEntity(UserConnection userConnection, Guid id)
        {
            var entity = userConnection.EntitySchemaManager
                .GetInstanceByName("GitPackageRepository")
                .CreateEntity(userConnection);

            entity.FetchFromDB(id, false);

            return entity;
        }

        /// <summary>
        /// Инициализация локального репозитория
        /// </summary>
        /// <exception cref="InvalidOperationException"> Доступ к репозиторию не найден </exception>
        void InitRepository()
        {

            Entity.SetColumnValue("GitLockedById", UserConnection.CurrentUser.ContactId);

            var workingDirectoryPath = Terrasoft.Core.Configuration.SysSettings.GetValue(UserConnection, "GitWorkingDirectoryPath", string.Empty);
            var remoteUrl = Entity.GetTypedColumnValue<string>("GitRemoteUrl");
            var repositoryName = Entity.GetTypedColumnValue<string>("GitName");
            var accessEntity = GetGitRepositoryAccessEntity(Entity.GetTypedColumnValue<Guid>("GitRepositoryAccessId"));

            if (accessEntity == null)
            {
                throw new InvalidOperationException(nameof(accessEntity));
            }

            var gitProvider = new GitProvider(
                remoteUrl,
                repositoryName,
                workingDirectoryPath,
                accessEntity.GetTypedColumnValue<string>("GitEmail"),
                accessEntity.GetTypedColumnValue<string>("GitUsername"),
                accessEntity.GetTypedColumnValue<string>("GitPassword"));

            Entity.SetColumnValue("GitLockedById", null);
        }

        Entity GetGitRepositoryAccessEntity(Guid id)
        {
            var entity = UserConnection.EntitySchemaManager
                .GetInstanceByName("GitRepositoryAccess")
                .CreateEntity(UserConnection);

            entity.FetchFromDB(id, false);

            return entity;
        }
    }
}
