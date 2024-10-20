using System;
using System.ServiceModel.Activation;
using System.ServiceModel;
using Terrasoft.Web.Common.ServiceRouting;
using System.ServiceModel.Web;
using Terrasoft.Configuration;
using Terrasoft.Core.Factories;
using GitManager.Helpers;
using Common.Logging;

namespace GitManager.Services
{
    [ServiceContract(Name = "GitService")]
    [DefaultServiceRoute]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class BaseService : Terrasoft.Web.Common.BaseService
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public BaseService()
        {
            _helperLazy = new Lazy<BaseHelper>(() => new BaseHelper(UserConnection));
        }

        /// <summary>
        /// Хелпер.
        /// </summary>
        BaseHelper Helper => _helperLazy.Value;

        /// <summary>
        /// Ленивый хелпер
        /// </summary>
        readonly Lazy<BaseHelper> _helperLazy;

        /// <summary>
        /// Логгер.
        /// </summary>
        readonly ILog Logger = LogManager.GetLogger(nameof(BaseService));


        /// <summary>
        /// Возвращает результат доступа к функционалу
        /// </summary>
        /// <returns> Результат доступа к функционалу </returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "GetCanAccessGitManager",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        public ConfigurationServiceResponse GetCanAccessGitManager()
        {
            var response = new ConfigurationServiceResponse();
            try
            {
                response.Success = Helper.GetCanAccessGitManager();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                response.Success = false;
                response.ErrorInfo = response.SetErrorInfo(ex);
            }

            return response;
        }
    }
}