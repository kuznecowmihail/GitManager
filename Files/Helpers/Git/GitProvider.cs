using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GitManager.Extensions;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace GitManager.Helpers.Git
{
    /// <summary>
    /// Клиент Git
    /// </summary>
    public class GitProvider : IDisposable
    {
        /// <summary>
        /// Ссылка на удаленный репозиторий
        /// </summary>
        string RemoteRepositoryUtl { get; }

        /// <summary>
        /// Рабочая директория
        /// </summary>
        string WorkingDirectoryPath { get; }

        /// <summary>
        /// Название репозитория
        /// </summary>
        string RepositoryName { get; }

        /// <summary>
        /// Сущность репозитория
        /// </summary>
        Repository Repository { get; set; }

        /// <summary>
        /// Почта пользователя
        /// </summary>
        string Email { get; }

        /// <summary>Логин пользователяUser name
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Пароль или токен пользователя
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="email"> Почта пользователя </param>
        /// <param name="username"> Логин пользователя </param>
        /// <param name="password"> Пароль или токен пользователя </param>
        /// <exception cref="InvalidOperationException"> Репозиторий не проинициализровался </exception>
        public GitProvider(
            string remoteRepositoryUtl,
            string repositoryName,
            string workingDirectoryPath,
            string email,
            string username,
            string password)
        {
            Argument.NotNullOrEmpty(remoteRepositoryUtl, nameof(remoteRepositoryUtl));
            Argument.NotNullOrEmpty(repositoryName, nameof(repositoryName));
            Argument.NotNullOrEmpty(workingDirectoryPath, nameof(workingDirectoryPath));
            Argument.NotNullOrEmpty(email, nameof(email));
            Argument.NotNullOrEmpty(username, nameof(username));
            Argument.NotNullOrEmpty(password, nameof(password));

            RemoteRepositoryUtl = remoteRepositoryUtl;
            RepositoryName = repositoryName;
            WorkingDirectoryPath = workingDirectoryPath;
            Email = email;
            Username = username;
            Password = password;

            InitRepository();
        }

        /// <summary>
        /// Инициализация репозитория
        /// </summary>
        /// <exception cref="InvalidOperationException"> Рабочая папка не существует или Репозиторий не инициализироваля </exception>
        protected void InitRepository()
        {
            var repositoryPath = $"{WorkingDirectoryPath}\\{RepositoryName}";

            if (!Directory.Exists(WorkingDirectoryPath))
            {
                throw new InvalidOperationException(WorkingDirectoryPath);
            }

            if (!Directory.Exists(repositoryPath))
            {
                Directory.CreateDirectory(repositoryPath);
            }

            try
            {
                if (!Repository.IsValid(repositoryPath))
                {
                    var cloneOptions = new CloneOptions
                    {
                        CredentialsProvider = (url, user, cred) =>
                            new UsernamePasswordCredentials
                            {
                                Username = Username,
                                Password = Password // password or token
                            }
                    };
                    Repository.Clone(RemoteRepositoryUtl, repositoryPath, cloneOptions);
                }
                Repository = new Repository(repositoryPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (Repository == null)
            {
                throw new InvalidOperationException(nameof(Repository));
            }
        }

        /// <summary>
        /// Очистка сущности
        /// </summary>
        public void Dispose()
        {
            Repository?.Dispose();
        }

        /// <summary>
        /// Получить сущность ветки
        /// </summary>
        /// <param name="name"> Название ветки </param>
        /// <returns> Сущность ветки </returns>
        public Branch GetBrancgByName(string name)
        {
            return Repository.Branches.Where(t => t.FriendlyName.Equals(name)).FirstOrDefault();
        }

        /// <summary>
        /// Переключиться на ветку
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> Ветка пустая </exception>
        public Branch Checkout(Branch branch)
        {
            Argument.NotNull(branch, nameof(branch));

            return Commands.Checkout(Repository, branch);
        }

        /// <summary>
        /// Отменить изменения в текущей ветке
        /// </summary>
        public void DiscardAllFileChanges()
        {
            var status = Repository.RetrieveStatus();

            var options = new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force };

            var filePaths = status.Modified
                .Select(mods => mods.FilePath)
                .ToList();

            filePaths.ForEach(filePath => Repository.CheckoutPaths(
                Repository.Head.FriendlyName,
                new[] { filePath },
                options));
        }

        /// <summary>
        /// Актуализация ветки из удаленного репозитория
        /// </summary>
        public void Fetch()
        {
            string logMessage = "";
            var options = new FetchOptions()
            {
                CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) =>
                    new UsernamePasswordCredentials()
                    {
                        Username = Username,
                        Password = Password
                    })
            };

            foreach (var remote in Repository.Network.Remotes)
            {
                IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                Commands.Fetch(Repository, remote.Name, refSpecs, options, logMessage);
            }
        }

        /// <summary>
        /// Получить изменения в ветку из удаленного репозитория
        /// </summary>
        public void Pull()
        {
            var pullOptions = new PullOptions()
            {
                FetchOptions = new FetchOptions()
                {
                    Prune = true,
                    CredentialsProvider = (url, user, cred) =>
                        new UsernamePasswordCredentials
                        {
                            Username = Username,
                            Password = Password // password or token
                        }
                }
            };
            Commands.Pull(Repository, new Signature(Username, Email, DateTimeOffset.Now), pullOptions);
        }

        /// <summary>
        /// Создать ветку из текущей
        /// </summary>
        /// <param name="name"> Название ветки </param>
        /// <returns> Созданная ветка </returns>
        /// <exception cref="InvalidOperationException"> Ветка существует </exception>
        public Branch CreateBranch(string name)
        {
            if (Repository.Branches.Any(t => t.RemoteName.Equals(name)))
            {
                throw new InvalidOperationException("Branch already exists");
            }
            var branch = Repository.CreateBranch(name);

            return Commands.Checkout(Repository, branch);
        }

        /// <summary>
        /// Индексировать все изменения
        /// </summary>
        public void StageChanges()
        {
            var status = Repository.RetrieveStatus();
            var filePaths = status.Modified.Select(mods => mods.FilePath).ToList();

            Commands.Stage(Repository, filePaths);
        }

        /// <summary>
        /// Зафиксировать изменения
        /// </summary>
        /// <param name="message"> Название коммита </param>
        /// <returns></returns>
        public Commit CommitChanges(string message)
        {
            return Repository.Commit(
                message,
                new Signature(Username, Email, DateTimeOffset.Now),
                new Signature(Username, Email, DateTimeOffset.Now));
        }

        /// <summary>
        /// Отправить изменения в удаленный репозиторий
        /// </summary>
        /// <param name="branch"> Ветка для отправления </param>
        public void PushChanges(Branch branch)
        {
            var pushOptions = new PushOptions()
            {
                CredentialsProvider = (url, user, cred) =>
                    new UsernamePasswordCredentials
                    {
                        Username = Username,
                        Password = Password // password or token
                    }
            };

            foreach (var remote in Repository.Network.Remotes)
            {
                Repository.Branches.Update(branch, b => b.Remote = remote.Name, b => b.UpstreamBranch = branch.CanonicalName);
                Repository.Network.Push(branch, pushOptions);
            }
        }
    }
}
