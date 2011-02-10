﻿namespace Tvl.VisualStudio.Language.Antlr3
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Intellisense;
    using Tvl.VisualStudio.Language.Parsing;

    [Export(typeof(IIntellisenseControllerProvider))]
    [ContentType(AntlrConstants.AntlrContentType)]
    [Order]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [Name("AntlrIntellisenseController")]
    internal class AntlrIntellisenseControllerProvider : IntellisenseControllerProvider
    {
        [Import]
        public IGlyphService GlyphService
        {
            get;
            private set;
        }

        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory
        {
            get;
            private set;
        }

        [Import]
        private IBackgroundParserFactoryService BackgroundParserFactoryService
        {
            get;
            set;
        }

        protected override IntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            AntlrIntellisenseController controller = new AntlrIntellisenseController(textView, this, (AntlrBackgroundParser)BackgroundParserFactoryService.GetBackgroundParser(textView.TextBuffer));
            textView.Properties[typeof(AntlrIntellisenseController)] = controller;
            return controller;
        }
    }
}