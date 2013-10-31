using Coevery.Scripting.Compiler;

namespace Coevery.Scripting.Ast {
    public interface IAstNodeWithToken {
        Token Token { get; }
    }
}