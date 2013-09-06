using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Serialization
{
    /// <summary>
    /// This class provides simple methods for serializing a collection of numerical ids (like the ones Orchard content items have).
    /// </summary>
    [OrchardFeature("Piedone.HelpfulLibraries.Serialization")]
    public class IdSerializer
    {
        private IEnumerable<int> _ids;
        /// <summary>
        /// Stored ids
        /// </summary>
        public IEnumerable<int> Ids
        {
            get
            {
                if (_ids == null)
                {
                    _ids = DeserializeIds(_idsDefinition);
                }

                return _ids;
            }
            set
            {
                _ids = value;
                _idsDefinition = SerializeIds(_ids);
            }
        }


        private string _idsDefinition;

        /// <summary>
        /// JSON array of the ids
        /// </summary>
        public string IdsDefinition
        {
            get { return _idsDefinition; }
            set
            {
                _idsDefinition = value;
                _ids = null;
            }
        }


        /// <summary>
        /// Constructs a new IdSerializer
        /// </summary>
        /// <param name="idsDefinition">JSON array of the ids</param>
        public IdSerializer(string idsDefinition = "")
        {
            IdsDefinition = idsDefinition;
        }


        /// <summary>
        /// Serializes ids to a JSON array
        /// </summary>
        public static string SerializeIds(IEnumerable<int> ids)
        {
            if (ids == null) ids = Enumerable.Empty<int>();
            return new JavaScriptSerializer().Serialize(ids);
        }

        /// <summary>
        /// Deserializes ids from a JSON array
        /// </summary>
        public static IEnumerable<int> DeserializeIds(string idsDefinition)
        {
            if (String.IsNullOrEmpty(idsDefinition))
            {
                return Enumerable.Empty<int>();
            }

            return new JavaScriptSerializer().Deserialize<IEnumerable<int>>(idsDefinition);
        }
    }
}
