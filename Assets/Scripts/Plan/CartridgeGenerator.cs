using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using UnityEngine;

// This script manages the cartridges creation
public class CartridgeGenerator : MonoBehaviour
{
    // Manager
    [SerializeField] private PdfManager _pdfManager;

    // Textures
    [SerializeField] private Texture2D _logoBYCN;
    [SerializeField] private Texture2D _logoBBF;
    [SerializeField] private Texture2D _logoRDI;

    // Header cartridge data
    private string _mainTopHeader = "CONFIGURATEUR DE CHAMBRE D’EHPAD (CCE)";

    // Subheader cartridge data
    private string _prescriberTxt = "PRESCRIPTEUR";
    private string _prescriberTeam = "PÔLE SANTÉ";
    private string _prescriberContact = "1 av Eugène Freyssinet\r78061 GUYANCOURT\rContact : Matthieu GUET";
    private string _developmentTxt = "DÉVELOPPEMENT";
    private string _developmentTeam = "R&DI - DESIGN LAB";
    private string _developmentContact = "1 av Eugène Freyssinet\r78061 GUYANCOURT\rContact : Pierre MARTIN";

    // Plan cartridge data
    private string _mainDownHeader = "CONFIGURATION CHAMBRE D’EHPAD";
    private string _dateHeader = "DATE";
    private string  _sideHeader = "VUE";
    private string _scaleHeader = "ÉCHELLE";

    // Cover cartridge data
    private string _phaseText = "PHASE";
    private string _phaseChoice = "ESQ";
    private string _affairText = "AFFAIRE";
    private string _affairChoice = "CCE";
    private string _issuerText = "ÉMETTEUR";
    private string _issuerChoice = "DL";
        
    public void CreatePlanCartridgeTemplate()
    {
        // Create a table with 3 columns 
        Table l_table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
        l_table.SetMarginTop(PdfUtility.ConvertCmToPoints(24f)); // At footer position

        // First row
        Cell l_cell1 = new Cell(1, 3); // Merging 3 columns
        l_cell1.Add(new Paragraph(_mainDownHeader).SetFontSize(10));
        l_cell1.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell1.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell1);

        // Second row
        Cell l_cell2 = new Cell(1, 1);
        l_cell2.Add(new Paragraph(_dateHeader).SetFontSize(10));
        l_cell2.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell2.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell2);

        Cell l_cell3 = new Cell(1, 1);
        l_cell3.Add(new Paragraph(_sideHeader).SetFontSize(10));
        l_cell3.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell3.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell3);

        Cell l_cell4 = new Cell(1, 1);
        l_cell4.Add(new Paragraph(_scaleHeader).SetFontSize(10));
        l_cell4.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell4.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell4);

        // Third row
        Cell l_cell5 = new Cell(1, 1);
        l_cell5.Add(new Paragraph(_pdfManager.GetDate()).SetFontSize(10).SetBold());
        l_cell5.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell5.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell5);

        Cell l_cell6 = new Cell(1, 1);
        l_cell6.Add(new Paragraph(_pdfManager.GetSideName().ToUpper()).SetFontSize(10).SetBold());
        l_cell6.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell6.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell6);

        Cell l_cell7 = new Cell(1, 1);
        l_cell7.Add(new Paragraph(_pdfManager.GetScale()).SetFontSize(10).SetBold());
        l_cell7.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell7.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell7);

        _pdfManager.AddElementToDocument(l_table);
    }

    public void CreateHeaderCartridgeTemplate()
    {
        // Create a table with 2 columns 
        Table l_table = new Table(UnitValue.CreatePercentArray(new float[] { 25f, 75f })).UseAllAvailableWidth();

        byte[] l_logoBYCNBytes = _logoBYCN.EncodeToPNG();
        ImageData l_imageData = ImageDataFactory.Create(l_logoBYCNBytes);
        Image l_img = new Image(l_imageData).ScaleToFit(100, 50);
        l_img.SetHorizontalAlignment(HorizontalAlignment.CENTER);
        Cell l_cell2 = new Cell();
        l_table.AddCell(l_cell2);
        l_cell2.Add(l_img);
        l_cell2.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell2.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        

        Cell l_cell1 = new Cell().Add(new Paragraph(_mainTopHeader).SetFontSize(10).SetBold());
        l_cell1.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell1.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_cell1.SetVerticalAlignment(VerticalAlignment.MIDDLE);
        l_table.AddCell(l_cell1);

        _pdfManager.AddElementToDocument(l_table);
    }

    public void CreateSubheaderCartridgeTemplate()
    {
        // Create a table with 4 columns 
        Table l_table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth();
        l_table.SetMarginTop(PdfUtility.ConvertCmToPoints(0.2f)); // At footer position

        // First row
        Cell l_cell1 = new Cell(1, 1);
        l_cell1.Add(new Paragraph(_prescriberTxt).SetFontSize(10));
        l_cell1.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell1.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_cell1.SetVerticalAlignment(VerticalAlignment.MIDDLE);
        l_table.AddCell(l_cell1);

        byte[] l_prescriberBytes = _logoBBF.EncodeToPNG();
        ImageData l_imagePrescriberData = ImageDataFactory.Create(l_prescriberBytes);
        Image l_img = new Image(l_imagePrescriberData).ScaleToFit(100, 50); 
        l_img.SetHorizontalAlignment(HorizontalAlignment.CENTER);
        Cell l_cell2 = new Cell().Add(l_img);
        l_cell2.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell2.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell2);

        
        Cell l_cell3 = new Cell(1, 1);
        l_cell3.Add(new Paragraph(_prescriberTeam).SetFontSize(10));
        l_cell3.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell3.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_cell3.SetVerticalAlignment(VerticalAlignment.MIDDLE);
        l_table.AddCell(l_cell3);
        

        Cell l_cell4 = new Cell(1, 1);
        l_cell4.Add(new Paragraph(_prescriberContact).SetFontSize(10));
        l_cell4.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell4.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell4);

        // Second row
        Cell l_cell5 = new Cell(1, 1);
        l_cell5.Add(new Paragraph(_developmentTxt).SetFontSize(10));
        l_cell5.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell5.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_cell5.SetVerticalAlignment(VerticalAlignment.MIDDLE);
        l_table.AddCell(l_cell5);

        byte[] l_devBytes = _logoRDI.EncodeToPNG();
        ImageData l_imageDevData = ImageDataFactory.Create(l_devBytes);
        Image l_img2 = new Image(l_imageDevData).ScaleToFit(100, 50);
        l_img2.SetHorizontalAlignment(HorizontalAlignment.CENTER);
        Cell l_cell6 = new Cell().Add(l_img2);
        l_cell6.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell6.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell6);

        Cell l_cell7 = new Cell(1, 1);
        l_cell7.Add(new Paragraph(_developmentTeam).SetFontSize(10));
        l_cell7.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell7.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_cell7.SetVerticalAlignment(VerticalAlignment.MIDDLE);
        l_table.AddCell(l_cell7);

        Cell l_cell8 = new Cell(1, 1);
        l_cell8.Add(new Paragraph(_developmentContact).SetFontSize(10));
        l_cell8.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell8.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell8);

        _pdfManager.AddElementToDocument(l_table);
    }

    public void CreateFooterCartridgeTemplate()
    {
        // Create a table with 2 columns 
        Table l_table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
        l_table.SetMarginTop(PdfUtility.ConvertCmToPoints(17f)); // At footer position

        // First row
        Cell l_cell1 = new Cell(1, 2); 
        l_cell1.Add(new Paragraph(_mainDownHeader).SetFontSize(10));
        l_cell1.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell1.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell1);
        
        // Second row
        Cell l_cell2 = new Cell(1, 1);
        l_cell2.Add(new Paragraph(_phaseText).SetFontSize(10));
        l_cell2.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell2.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell2);

        Cell l_cell3 = new Cell(1, 1);
        l_cell3.Add(new Paragraph(_phaseChoice).SetFontSize(10).SetBold());
        l_cell3.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell3.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell3);

        // Third row
        Cell l_cell4 = new Cell(1, 1);
        l_cell4.Add(new Paragraph(_affairText).SetFontSize(10));
        l_cell4.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell4.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell4);

        Cell l_cell5 = new Cell(1, 1);
        l_cell5.Add(new Paragraph(_affairChoice).SetFontSize(10).SetBold());
        l_cell5.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell5.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell5);

        // 4th row
        Cell l_cell6 = new Cell(1, 1);
        l_cell6.Add(new Paragraph(_issuerText).SetFontSize(10));
        l_cell6.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell6.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell6);

        Cell l_cell7 = new Cell(1, 1);
        l_cell7.Add(new Paragraph(_issuerChoice).SetFontSize(10).SetBold());
        l_cell7.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        l_cell7.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        l_table.AddCell(l_cell7);

        _pdfManager.AddElementToDocument(l_table);
    }
}