using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Infrastructure;

namespace Rise.Shared.Documents;

public interface IDocumentService
{
    IDocument CreatePdfDocument(DocumentDto.Index model, bool isOrder);
}
