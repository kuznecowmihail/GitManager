using GitManager.Extensions;
using Terrasoft.Core;

namespace GitManager.Helpers
{
    /// <summary>
    /// Базовый хелпер
    /// </summary>
    internal class BaseHelper
    {
        /// <summary>
        /// Подключение пользователя
        /// </summary>
        protected UserConnection UserConnection { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userConnection"></param>
        internal BaseHelper(UserConnection userConnection)
        {
            Argument.NotNull(userConnection, nameof(userConnection));
            UserConnection = userConnection;
        }

        /// <summary>
        /// Возвращает результат доступа к функционалу
        /// </summary>
        /// <returns> Результат доступа к функционалу </returns>
        internal bool GetCanAccessGitManager()
        {
            var operations = new string[]
            {
                "CanManageAdministration"
            };
            var result = false;

            foreach (var operation in operations)
            {
                result = result || UserConnection.DBSecurityEngine.GetCanExecuteOperation(operation);
                
                if (result)
                {
                    break;
                }
            }

            result = result && GlobalAppSettings.IsFileDesignModeEnabled;

            return result;
        }
    }
}
