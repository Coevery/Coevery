using Orchard.ContentManagement;

namespace Orchard.WebApi.Common
{
    public interface IDto<TContentType> where TContentType : ContentPart 
    {
        void UpdateEntity(TContentType entity);
        int ContentId { get; set; }
    }
}
