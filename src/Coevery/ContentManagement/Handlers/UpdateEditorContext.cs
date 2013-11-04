using Coevery.DisplayManagement;
using Coevery.DisplayManagement.Descriptors;

namespace Coevery.ContentManagement.Handlers {
    public class UpdateEditorContext : BuildEditorContext {

        public UpdateEditorContext(IShape model, IContent content, IUpdateModel updater, string groupInfoId, IShapeFactory shapeFactory, ShapeTable shapeTable)
            : base(model, content, groupInfoId, shapeFactory) {
            
            ShapeTable = shapeTable;
            Updater = updater;
        }

        public IUpdateModel Updater { get; private set; }
        public ShapeTable ShapeTable { get; private set; }
    }
}