using System;
using System.Globalization;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using UnityEngine;

// This script manages the creation and manipulation of the PDF document
public class PdfManager : MonoBehaviour
{
    // Managers
    [SerializeField] private CartridgeGenerator _cartridgeGenerator;
    [SerializeField] private ImageCaptureService _imageCaptureService;

    [SerializeField] private Texture2D _logoBYCNBuildingForLife;
    [SerializeField] private Texture2D _coverImage;

    [SerializeField] UnityEngine.UI.Button _openPDFButton;

    // PDF document
    private string _path = string.Empty;
    private string _fileName = string.Empty;
    private PageSize _pageSize;
    private PdfWriter _writer;
    private PdfDocument _pdf;
    private Document _document;

    // Dynamics cartridge data
    private string _dateText = string.Empty;
    private string  _sideText = string.Empty;
    private string _scaleText = string.Empty;
    private CameraManager.Side[] _sides;

    // Scaling
    [SerializeField] private float _imageSizeInCm = 19f;
    [SerializeField] private PdfUtility.ScaleInCm _scaleInCm = PdfUtility.ScaleInCm._1_50;

    private void Start() 
    {
        InitializeData();
    }

    private void InitializeData()
    {
        _path = Application.streamingAssetsPath + "/PlanPDF";
        _sides = (CameraManager.Side[])Enum.GetValues(typeof(CameraManager.Side));
        _scaleText += PdfUtility.ScaleInCmToText(_scaleInCm);
        GenerateDate();
        GenerateFileName();
    }

    public void InitializePdfDocument()
    {
        _pageSize = PageSize.A4;
        _writer = new PdfWriter(_fileName);
        _pdf = new PdfDocument(_writer);  
        _document = new Document(_pdf, _pageSize);  
    }

    public void CreatePdfFromAllSides(Texture2D[] p_capturedTextures)
    {
        bool l_result = false;
  
        try
        {
            InitializePdfDocument();

            CreateCoverPage();
            AddAllViewsCaptureToPdf(p_capturedTextures);
            CreateFinalPage();

            _document.Close();
            Debug.Log($"PDF created at : {_fileName}");
            l_result = true;
        }
        catch (System.Exception e)
        {
            Debug.Log($"PDF creation failed : {e.Message}");
        }

        if(_openPDFButton != null)
        {
            _openPDFButton.interactable = l_result;
        }
    }

    public void CreatePdfFromSelectedSide(Texture2D p_capturedTexture)
    {
         try
        {
            InitializePdfDocument();    
            AddImageCapturedToPdf(p_capturedTexture.EncodeToPNG(), _imageSizeInCm);

            _document.Close();
        
            Debug.Log($"CaptureCameraViews - PDF created at : {_fileName}");
        }
        catch (Exception e)
        {
            Debug.Log($"CaptureCameraViews - PDF creation failed : {e.Message}");
        }    
    }


    private void CreateCoverPage() 
    {
        _coverImage = _imageCaptureService.CaptureCoverImage();
        AddImageCapturedToPdf(_coverImage.EncodeToPNG(), _imageSizeInCm);

        _cartridgeGenerator.CreateHeaderCartridgeTemplate();
        _cartridgeGenerator.CreateSubheaderCartridgeTemplate();
        _cartridgeGenerator.CreateFooterCartridgeTemplate();
    }

    public void CreateFinalPage()
    {
        AddNewPage();
        AddImageCapturedToPdf(_logoBYCNBuildingForLife.EncodeToPNG(), _imageSizeInCm);
    }

    // Add images to PDF
    private void AddImageCapturedToPdf(byte[] p_imageBytes, float p_imageSizeInCm, float p_xOffset = 0, float p_yOffset = 0)
    {
        ImageData l_imageData = ImageDataFactory.Create(p_imageBytes);
        Image l_pdfImage = new Image(l_imageData);
        l_pdfImage.ScaleToFit(PdfUtility.ConvertCmToPoints(p_imageSizeInCm), PdfUtility.ConvertCmToPoints(p_imageSizeInCm));

        float l_xPosition = (_pdf.GetDefaultPageSize().GetWidth() - l_pdfImage.GetImageScaledWidth()) / 2 + p_xOffset;
        float l_yPosition = (_pdf.GetDefaultPageSize().GetHeight() - l_pdfImage.GetImageScaledHeight()) / 2 + p_yOffset;

        l_pdfImage.SetFixedPosition(l_xPosition, l_yPosition);
        _document.Add(l_pdfImage);
    }

    private void AddAllViewsCaptureToPdf(Texture2D[] p_capturedTextures)
    {
        for (int i = 0; i < p_capturedTextures.Length; i++)
        {
            AddNewPage();
            
            _sideText = CameraManager.SideToText(_sides[i]);
            AddImageCapturedToPdf(p_capturedTextures[i].EncodeToPNG(), _imageSizeInCm);

            _cartridgeGenerator.CreatePlanCartridgeTemplate();
        }
    }

    // Getters
    public string GetDate()
    {
        return _dateText;
    }

    public string GetFileName()
    {
        return _fileName;
    }

    public string GetSideName()
    {
        return _sideText;
    }

    public string GetScale()
    {
        return _scaleText;
    }

    // Utils
    private void GenerateFileName()
    {
        _fileName = $"{_path}/BYCN_CCE_plans_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.pdf";
        _fileName.Replace("/", "//").Replace(" ", "/ ");
    }

    private void GenerateDate()
    {
        CultureInfo l_frCultureInfo = new CultureInfo("fr-FR");
        DateTime l_date = DateTime.Now;
        string l_dateFrenchFormat = l_date.ToString("d", l_frCultureInfo);
        _dateText = l_dateFrenchFormat;
    }

    private void AddNewPage()
    {
        _document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
    }

    public void AddElementToDocument(IBlockElement p_element)
    {
        _document.Add(p_element);
    }

}