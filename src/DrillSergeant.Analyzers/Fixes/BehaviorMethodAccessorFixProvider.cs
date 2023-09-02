using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DrillSergeant.Analyzers.Fixes;

public class BehaviorMethodAccessorFixProvider : CodeFixProvider
{
    private const string Title = "Change modifier to public";

    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(Rules.BehaviorMethodAccessorAnalyzer.DiagnosticId);

    public override FixAllProvider? GetFixAllProvider() => null;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diag = context.Diagnostics.Single();
        var span = diag.Location.SourceSpan;
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        var node = root?.FindNode(span);

        if (node is not MethodDeclarationSyntax declaration)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => ReplaceModifier(context.Document, root!, declaration),
                equivalenceKey: Title),
            diag);
    }

    private static Task<Document> ReplaceModifier(Document document, SyntaxNode root, MethodDeclarationSyntax declaration)
    {
        var nonPublicAccessModifiers = new[]
        {
            SyntaxKind.PrivateKeyword,
            SyntaxKind.ProtectedKeyword,
            SyntaxKind.InternalKeyword
        };

        var modifiers = declaration
            .Modifiers
            .Where(t => nonPublicAccessModifiers.Any(m => t.IsKind(m)) == false)
            .ToList();

        var publicToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);

        modifiers.Insert(0, publicToken);

        var newDeclaration = declaration.WithModifiers(new SyntaxTokenList(modifiers));
        var newRoot = root?.ReplaceNode(declaration, newDeclaration);

        return Task.FromResult(document.WithSyntaxRoot(newRoot!));
    }
}
