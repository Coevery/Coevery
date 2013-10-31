using System.Xml.Linq;

namespace Coevery.Core.XmlRpc {
    public interface IXmlRpcHandler : IDependency {
        void SetCapabilities(XElement element);
        void Process(XmlRpcContext context);
    }
}