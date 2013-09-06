using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Orchard.Mvc;
using Orchard.UI.Resources;
using Piedone.Combinator.Extensions;
using Piedone.Combinator.Models;
using Piedone.HelpfulLibraries.Serialization;

namespace Piedone.Combinator.Services
{
    public class CombinatorResourceManager : ICombinatorResourceManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISimpleSerializer _serializer;


        public CombinatorResourceManager(
            IHttpContextAccessor httpContextAccessor,
            ISimpleSerializer serializer)
        {
            _httpContextAccessor = httpContextAccessor;
            _serializer = serializer;
        }


        public CombinatorResource ResourceFactory(ResourceType type)
        {
            return new CombinatorResource(type, _httpContextAccessor);
        }


        [DataContract]
        public class SerializableSettings
        {
            [DataMember]
            public Uri Url { get; set; }

            [DataMember]
            public string Culture { get; set; }

            [DataMember]
            public string Condition { get; set; }

            [DataMember]
            public Dictionary<string, string> Attributes { get; set; }
        }


        public string SerializeResourceSettings(CombinatorResource resource)
        {
            var settings = resource.RequiredContext.Settings;
            if (settings == null) return "";

            return _serializer.XmlSerialize(
                new SerializableSettings()
                {
                    Url = resource.IsOriginal ? resource.IsCdnResource ? resource.AbsoluteUrl : resource.RelativeUrl : null,
                    Culture = settings.Culture,
                    Condition = settings.Condition,
                    Attributes = settings.Attributes
                });
        }

        public void DeserializeSettings(string serialization, CombinatorResource resource)
        {
            if (String.IsNullOrEmpty(serialization)) return;

            var settings = _serializer.XmlDeserialize<SerializableSettings>(serialization);

            if (settings.Url != null)
            {
                var resourceManifest = new ResourceManifest();
                resource.RequiredContext.Resource = resourceManifest.DefineResource(resource.Type.ToStringType(), settings.Url.ToString());
                resource.RequiredContext.Resource.SetUrlWithoutScheme(settings.Url);
                resource.IsOriginal = true;
            }

            if (resource.RequiredContext.Settings == null) resource.RequiredContext.Settings = new RequireSettings();
            var resourceSettings = resource.RequiredContext.Settings;
            resourceSettings.Culture = settings.Culture;
            resourceSettings.Condition = settings.Condition;
            resourceSettings.Attributes = settings.Attributes;
        }
    }
}