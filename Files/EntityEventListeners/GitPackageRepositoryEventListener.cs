using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Terrasoft.Core.Entities.Events;
using Terrasoft.Core.Entities;
using GitManager.Helpers.EntityHelpers;
using Terrasoft.Core;

namespace GitManager.EntityEventListeners
{
    /// <summary>
	/// Событиыйный слойм "Репозиторий пакета Git"
	/// </summary>
	[EntityEventListener(SchemaName = "GitPackageRepository")]
    public class GitPackageRepositoryEventListener : BaseEntityEventListener
    {

        /// <summary>
        /// После создания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnInserting(object sender, EntityBeforeEventArgs e)
        {
            base.OnInserting(sender, e);
            var entity = (Entity)sender;

            GetHelper(entity.UserConnection, entity).OnInserting(sender, e);

        }

        /// <summary>
        /// Перед сохранения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnSaving(object sender, EntityBeforeEventArgs e)
        {
            base.OnSaving(sender, e);
            var entity = (Entity)sender;

            GetHelper(entity.UserConnection, entity.PrimaryColumnValue).OnSaving(sender, e);
        }

        /// <summary>
        /// Перед обновлением
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnUpdating(object sender, EntityBeforeEventArgs e)
        {
            base.OnUpdating(sender, e);
            var entity = (Entity)sender;
        }

        /// <summary>
        /// После обновления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnUpdated(object sender, EntityAfterEventArgs e)
        {
            base.OnUpdated(sender, e);
            var entity = (Entity)sender;
            var userConnection = entity.UserConnection;
        }

        /// <summary>
        /// Перед удалением
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnDeleting(object sender, EntityBeforeEventArgs e)
        {
            base.OnDeleting(sender, e);
            var entity = (Entity)sender;
            var userConnection = entity.UserConnection;
        }

        /// <summary>
        /// После удаления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnDeleted(object sender, EntityAfterEventArgs e)
        {
            base.OnDeleted(sender, e);
            var entity = (Entity)sender;
            var userConnection = entity.UserConnection;
        }

        GitPackageRepositoryHelper GetHelper(UserConnection userConnection, Entity entity)
        {
            return new GitPackageRepositoryHelper(userConnection, entity);
        }

        GitPackageRepositoryHelper GetHelper(UserConnection userConnection, Guid id)
        {
            return new GitPackageRepositoryHelper(userConnection, id);
        }
    }
}
