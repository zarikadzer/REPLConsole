namespace REPL.SyntaxAnalyzer
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Host.Mef;

    public class ReplAnalyzerCS
    {

        public ReplAnalyzerCS(string code) {
            Code = code;
        }

        public string Code { get; set; }

        /// <summary>
        /// Determines whether the given text is considered a syntactically complete submission.
        /// Throws <see cref="ArgumentException"/> if the tree was not compiled as an interactive submission.
        /// </summary>
        public bool IsCompleteSubmission() {
            var tree = CSharpSyntaxTree.ParseText(Code, new CSharpParseOptions(documentationMode: DocumentationMode.Diagnose, kind: SourceCodeKind.Script));

            if (tree == null) {
                throw new ArgumentNullException(nameof(tree));
            }
            if (tree.Options.Kind != SourceCodeKind.Script) {
                throw new ArgumentException("Syntax Tree Is Not A Submission");
            }

            if (!tree.HasCompilationUnitRoot) {
                return false;
            }
            var compilation = (CompilationUnitSyntax)tree.GetRoot();

            if (!compilation.GetDiagnostics().Any()) {
                return true;
            }

            foreach (var error in compilation.EndOfFileToken.GetDiagnostics()) {
                switch ((ErrorCode)Convert.ToInt32(error.Id.Substring(2))) {
                    case ErrorCode.ERR_OpenEndedComment:
                    case ErrorCode.ERR_EndifDirectiveExpected:
                    case ErrorCode.ERR_EndRegionDirectiveExpected:
                    return false;
                }
            }

            var lastNode = compilation.ChildNodes().LastOrDefault();
            if (lastNode == null) {
                return true;
            }

            // unterminated multi-line comment:
            if (lastNode.HasTrailingTrivia && lastNode.ContainsDiagnostics && HasUnterminatedMultiLineComment(lastNode.GetTrailingTrivia())) {
                return false;
            }

            if (lastNode.IsKind(SyntaxKind.IncompleteMember)) {
                return false;
            }

            // All top-level constructs but global statement (i.e. extern alias, using directive, global attribute, and declarations)
            // should have a closing token (semicolon, closing brace or bracket) to be complete.
            if (!lastNode.IsKind(SyntaxKind.GlobalStatement)) {
                var closingToken = lastNode.GetLastToken(includeZeroWidth: true, includeSkipped: true, includeDirectives: true, includeDocumentationComments: true);
                return !closingToken.IsMissing;
            }

            var globalStatement = (GlobalStatementSyntax)lastNode;
            var token = lastNode.GetLastToken(includeZeroWidth: true, includeSkipped: true, includeDirectives: true, includeDocumentationComments: true);

            if (token.IsMissing) {
                // expression statement terminating semicolon might be missing in script code:
                if (tree.Options.Kind == SourceCodeKind.Regular ||
                    !globalStatement.Statement.IsKind(SyntaxKind.ExpressionStatement) ||
                    !token.IsKind(SyntaxKind.SemicolonToken)) {
                    return false;
                }

                token = token.GetPreviousToken();
                if (token.IsMissing) {
                    return false;
                }
            }

            foreach (var error in token.GetDiagnostics()) {
                switch ((ErrorCode)Convert.ToInt32(error.Id.Substring(2))) {
                    // unterminated character or string literal:
                    case ErrorCode.ERR_NewlineInConst:

                    // unterminated verbatim string literal:
                    case ErrorCode.ERR_UnterminatedStringLit:

                    // unexpected token following a global statement:
                    case ErrorCode.ERR_GlobalDefinitionOrStatementExpected:
                    case ErrorCode.ERR_EOFExpected:
                    return false;
                }
            }

            return true;
        }

        private static bool HasUnterminatedMultiLineComment(SyntaxTriviaList triviaList) {
            foreach (var trivia in triviaList) {
                if (trivia.ContainsDiagnostics && trivia.Kind() == SyntaxKind.MultiLineCommentTrivia) {
                    return true;
                }
            }

            return false;
        }
    }

}
