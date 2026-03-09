using UnityEngine;

// This script coordinates the overall PDF generation process
public class PlanManager : MonoBehaviour
{
    [SerializeField] private PdfManager _pdfManager;
    [SerializeField] private ImageCaptureService _imageCaptureService;

    public void GeneratePdfFromAllSides()
    {
        _pdfManager.CreatePdfFromAllSides(_imageCaptureService.CaptureAllSides());
    }

    public void GeneratePdfFromSelectedSide()
    {
        _pdfManager.CreatePdfFromSelectedSide(_imageCaptureService.CaptureSelectedSide());
    }

    public void OpenGeneratedPDF()
    {
        if(System.IO.File.Exists(_pdfManager.GetFileName()))
        {
            Application.OpenURL(_pdfManager.GetFileName());
        }
    }
}