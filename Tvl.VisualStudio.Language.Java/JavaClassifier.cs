﻿namespace Tvl.VisualStudio.Language.Java
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using System;
    using System.Reflection;
    using Tvl.VisualStudio.Language.Parsing;

    internal sealed class JavaClassifier : AntlrClassifierBase<JavaClassifierLexerState>
    {
        private static readonly HashSet<string> Keywords =
            new HashSet<string>()
            {
                "abstract",
                "assert",
                "boolean",
                "break",
                "byte",
                "case",
                "catch",
                "char",
                "class",
                "const",
                "continue",
                "default",
                "do",
                "double",
                "else",
                "enum",
                "extends",
                "final",
                "finally",
                "float",
                "for",
                "if",
                "goto",
                "implements",
                "import",
                "instanceof",
                "int",
                "interface",
                "long",
                "native",
                "new",
                "package",
                "private",
                "protected",
                "public",
                "return",
                "short",
                "static",
                "strictfp",
                "super",
                "switch",
                "synchronized",
                "this",
                "throw",
                "throws",
                "transient",
                "try",
                "void",
                "volatile",
                "while",
                "true",
                "false",
                "null"
            };

        private readonly ITextBuffer _textBuffer;
        private readonly IStandardClassificationService _standardClassificationService;

        public JavaClassifier(ITextBuffer textBuffer, IStandardClassificationService standardClassificationService)
            : base(textBuffer)
        {
            this._textBuffer = textBuffer;
            this._standardClassificationService = standardClassificationService;
        }

        protected override JavaClassifierLexerState GetStartState()
        {
            return JavaClassifierLexerState.Initial;
        }

        protected override ITokenSourceWithState<JavaClassifierLexerState> CreateLexer(ICharStream input, JavaClassifierLexerState state)
        {
            return new JavaClassifierLexer(input, state);
        }

        protected override bool IsMultilineToken(ITextSnapshot snapshot, ITokenSource lexer, IToken token)
        {
            JavaClassifierLexer javaLexer = lexer as JavaClassifierLexer;
            if (javaLexer != null && javaLexer.CharStream.Line >= token.Line)
                return false;

            int startLine = snapshot.GetLineNumberFromPosition(token.StartIndex);
            int stopLine = snapshot.GetLineNumberFromPosition(token.StopIndex + 1);
            return startLine != stopLine;
        }

        protected override bool TokenEndsAtEndOfLine(ITextSnapshot snapshot, ITokenSource lexer, IToken token)
        {
            JavaClassifierLexer javaLexer = lexer as JavaClassifierLexer;
            if (javaLexer != null)
            {
                int c = javaLexer.CharStream.LA(1);
                return c == '\r' || c == '\n';
            }

            ITextSnapshotLine line = snapshot.GetLineFromPosition(token.StopIndex + 1);
            return line.End <= token.StopIndex + 1 && line.EndIncludingLineBreak >= token.StopIndex + 1;
        }

        protected override IClassificationType ClassifyToken(IToken token)
        {
            switch (token.Type)
            {
#if false // operator coloring was just annoying
            case JavaColorizerLexer.EQ:
            case JavaColorizerLexer.NEQ:
            case JavaColorizerLexer.EQEQ:
            case JavaColorizerLexer.PLUS:
            case JavaColorizerLexer.PLUSEQ:
            case JavaColorizerLexer.MINUS:
            case JavaColorizerLexer.MINUSEQ:
            case JavaColorizerLexer.TIMES:
            case JavaColorizerLexer.TIMESEQ:
            case JavaColorizerLexer.DIV:
            case JavaColorizerLexer.DIVEQ:
            case JavaColorizerLexer.LT:
            case JavaColorizerLexer.GT:
            case JavaColorizerLexer.LE:
            case JavaColorizerLexer.GE:
            case JavaColorizerLexer.NOT:
            case JavaColorizerLexer.BITNOT:
            case JavaColorizerLexer.AND:
            case JavaColorizerLexer.BITAND:
            case JavaColorizerLexer.ANDEQ:
            case JavaColorizerLexer.QUES:
            case JavaColorizerLexer.OR:
            case JavaColorizerLexer.BITOR:
            case JavaColorizerLexer.OREQ:
            case JavaColorizerLexer.COLON:
            case JavaColorizerLexer.INC:
            case JavaColorizerLexer.DEC:
            case JavaColorizerLexer.XOR:
            case JavaColorizerLexer.XOREQ:
            case JavaColorizerLexer.MOD:
            case JavaColorizerLexer.MODEQ:
            case JavaColorizerLexer.LSHIFT:
            case JavaColorizerLexer.RSHIFT:
            case JavaColorizerLexer.LSHIFTEQ:
            case JavaColorizerLexer.RSHIFTEQ:
            case JavaColorizerLexer.ROR:
            case JavaColorizerLexer.ROREQ:
                return _standardClassificationService.Operator;
#endif

            case JavaColorizerLexer.CHAR_LITERAL:
                //return _standardClassificationService.CharacterLiteral;
                return _standardClassificationService.StringLiteral;

            case JavaColorizerLexer.STRING_LITERAL:
                return _standardClassificationService.StringLiteral;

            case JavaColorizerLexer.NUMBER:
                return _standardClassificationService.NumberLiteral;

            case JavaColorizerLexer.IDENTIFIER:
                if (Keywords.Contains(token.Text))
                    return _standardClassificationService.Keyword;

                return _standardClassificationService.Identifier;

            case JavaColorizerLexer.COMMENT:
            case JavaColorizerLexer.ML_COMMENT:
                return _standardClassificationService.Comment;

            default:
                return null;
            }
        }
    }
}