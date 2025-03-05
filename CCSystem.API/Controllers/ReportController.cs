using CCSystem.API.Constants;
using CCSystem.BLL.DTOs.Report;
using CCSystem.BLL.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost(APIEndPointConstant.Report.CreateReportEndpoint)]
    public async Task<IActionResult> CreateReportAsync([FromBody] ReportRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  // Trả về lỗi nếu ModelState không hợp lệ
        }

        var reportResponse = await _reportService.CreateReportAsync(request);
        return Ok(reportResponse);
    }
    [HttpPut(APIEndPointConstant.Report.UpdateReportEndpoint)]
    public async Task<IActionResult> UpdateReport(int id, ReportRequest request)
    {
        var result = await _reportService.UpdateReportAsync(id, request);
        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpDelete(APIEndPointConstant.Report.DeleteReportEndpoint)]
    public async Task<IActionResult> DeleteReport(int id)
    {
        var result = await _reportService.DeleteReportAsync(id);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpGet(APIEndPointConstant.Report.GetReportByIdEndpoint)]
    public async Task<IActionResult> GetReportById(int id)
    {
        var result = await _reportService.GetReportByIdAsync(id);
        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpGet(APIEndPointConstant.Report.GetAllReportsEndpoint)]
    public async Task<IActionResult> GetAllReports()
    {
        var result = await _reportService.GetAllReportsAsync();
        return Ok(result);
    }
}
