using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Exceptions;
using Orchard.FileSystems.Media;
using Orchard.Services;
using Piedone.Combinator.EventHandlers;
using Piedone.Combinator.Extensions;
using Piedone.Combinator.Models;

namespace Piedone.Combinator.Services
{
    [OrchardFeature("Piedone.Combinator")]
    public class CacheFileService : ICacheFileService
    {
        private readonly IStorageProvider _storageProvider;
        private readonly IRepository<CombinedFileRecord> _fileRepository;
        private readonly ICombinatorResourceManager _combinatorResourceManager;
        private readonly IClock _clock;
        private readonly ICombinatorEventHandler _combinatorEventHandler;

        #region In-memory caching fields
        private readonly ICacheManager _cacheManager;
        private readonly ICombinatorEventMonitor _combinatorEventMonitor;
        private const string CachePrefix = "Piedone.Combinator.";
        #endregion

        #region Static file caching fields
        private const string _rootPath = "Combinator/";
        private const string _stylesPath = _rootPath + "Styles/";
        private const string _scriptsPath = _rootPath + "Scripts/";
        #endregion

        public CacheFileService(
            IStorageProvider storageProvider,
            IRepository<CombinedFileRecord> fileRepository,
            ICombinatorResourceManager combinatorResourceManager,
            IClock clock,
            ICombinatorEventHandler combinatorEventHandler,
            ICacheManager cacheManager,
            ICombinatorEventMonitor combinatorEventMonitor)
        {
            _storageProvider = storageProvider;
            _fileRepository = fileRepository;
            _combinatorResourceManager = combinatorResourceManager;
            _clock = clock;
            _combinatorEventHandler = combinatorEventHandler;

            _cacheManager = cacheManager;
            _combinatorEventMonitor = combinatorEventMonitor;
        }

        public void Save(int hashCode, CombinatorResource resource)
        {
            var scliceCount = _fileRepository.Count(file => file.HashCode == hashCode);

            var fileRecord = new CombinedFileRecord()
            {
                HashCode = hashCode,
                Slice = ++scliceCount,
                Type = resource.Type,
                LastUpdatedUtc = _clock.UtcNow,
                Settings = _combinatorResourceManager.SerializeResourceSettings(resource)
            };

            if (!String.IsNullOrEmpty(resource.Content))
            {
                var path = MakePath(fileRecord);

                using (var stream = _storageProvider.CreateFile(path).OpenWrite())
                {
                    var bytes = Encoding.UTF8.GetBytes(resource.Content);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            _fileRepository.Create(fileRecord);
        }

        public IList<CombinatorResource> GetCombinedResources(int hashCode)
        {
            return _cacheManager.Get(MakeCacheKey("GetCombinedResources." + hashCode.ToString()), ctx =>
            {
                _combinatorEventMonitor.MonitorCacheEmptied(ctx);

                var files = GetRecords(hashCode);
                var fileCount = files.Count;

                var resources = new List<CombinatorResource>(fileCount);

                foreach (var file in files)
                {
                    var resource = _combinatorResourceManager.ResourceFactory(file.Type);
                    resource.FillRequiredContext(
                        "CombinedResource" + file.Id.ToString(),
                        _storageProvider.GetPublicUrl(MakePath(file)));
                    _combinatorResourceManager.DeserializeSettings(file.Settings, resource);
                    resource.LastUpdatedUtc = file.LastUpdatedUtc ?? _clock.UtcNow;
                    resources.Add(resource);
                }

                return resources;
            });
        }

        public bool Exists(int hashCode)
        {
            return _cacheManager.Get(MakeCacheKey("Exists." + hashCode.ToString()), ctx =>
            {
                _combinatorEventMonitor.MonitorCacheEmptied(ctx);
                // Maybe also check if the file exists?
                return _fileRepository.Count(file => file.HashCode == hashCode) != 0;
            });
        }

        public int GetCount()
        {
            return _fileRepository.Table.Count();
        }

        //public void Delete(int hashCode)
        //{
        //    DeleteFiles(GetRecords(hashCode));

        //    TriggerCacheChangedSignal(hashCode);
        //}

        public void Empty()
        {
            var files = _fileRepository.Table.ToList();
            DeleteFiles(files);

            // These will throw an exception if a folder doesn't exist. Since currently there is no method
            // in IStorageProvider to check the existence of a file/folder (see: http://orchard.codeplex.com/discussions/275146)
            // this is the only way to deal with it.
            // We don't check if there were any files in a DB here, we try to delete even if there weren't. This adds robustness: with emptying the cache
            // everything can be reset, even if the user or a deploy process manipulated the DB or the file system.
            try
            {
                _storageProvider.DeleteFolder(_scriptsPath);
                Thread.Sleep(300); // This is to ensure we don't get an "access denied" when deleting the root folder
            }
            catch (Exception ex)
            {
                if (ex.IsFatal()) throw;
            }

            try
            {
                _storageProvider.DeleteFolder(_stylesPath);
                Thread.Sleep(300);
            }
            catch (Exception ex)
            {
                if (ex.IsFatal()) throw;
            }

            try
            {
                _storageProvider.DeleteFolder(_rootPath);
            }
            catch (Exception ex)
            {
                if (ex.IsFatal()) throw;
            }

            _combinatorEventHandler.CacheEmptied();
        }

        private List<CombinedFileRecord> GetRecords(int hashCode)
        {
            return _fileRepository.Fetch(file => file.HashCode == hashCode).ToList();
        }

        private void DeleteFiles(List<CombinedFileRecord> files)
        {
            foreach (var file in files)
            {
                _fileRepository.Delete(file);
                // Try-catch for the case that someone deleted the file.
                // Currently there is no way to check the existence of a file.
                try
                {
                    _storageProvider.DeleteFile(MakePath(file));
                }
                catch (Exception ex)
                {
                    if (ex.IsFatal()) throw;
                }
            }
        }

        private static string MakePath(CombinedFileRecord file)
        {
            // Maybe others will come, therefore the architecture
            string extension = "";
            string folderPath = "";
            if (file.Type == ResourceType.JavaScript)
            {
                folderPath = _scriptsPath;
                extension = "js";
            }
            else if (file.Type == ResourceType.Style)
            {
                folderPath = _stylesPath;
                extension = "css";
            }

            return folderPath + file.GetFileName() + "." + extension;
        }

        private static string MakeCacheKey(string name)
        {
            return CachePrefix + name;
        }
    }
}