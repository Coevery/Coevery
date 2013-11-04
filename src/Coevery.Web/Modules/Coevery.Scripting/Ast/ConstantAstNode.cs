using Coevery.Scripting.Compiler;

namespace Coevery.Scripting.Ast {
    public class ConstantAstNode : AstNode, IAstNodeWithToken {
        private readonly Token _token;

        public ConstantAstNode(Token token) {
            _token = token;
        }

        public Token Token {
            get { return _token; }
        }

        public object Value { get { return _token.Value; } }

        public override object Accept(AstVisitor visitor) {
            return visitor.VisitConstant(this);
        }
    }
}