using CCSystem.API.Constants;
using CCSystem.BLL.DTOs.Report;
using CCSystem.BLL.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    /// <summary>
    /// Constructor để inject service quản lý báo cáo.
    /// </summary>
    /// <param name="reportService">Service báo cáo</param>
    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }
    /// <summary>
    /// Tạo một báo cáo mới.
    /// </summary>
    /// <param name="request">Thông tin báo cáo</param>
    [HttpPost(APIEndPointConstant.Report.CreateReportEndpoint)]
    public async Task<IActionResult> CreateReportAsync([FromBody] ReportRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _reportService.CreateReportAsync(request);
            return Ok(new { message = "Report created successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the report", error = ex.Message });
        }
    }
    /// <summary>
    /// Cập nhật báo cáo theo ID.
    /// </summary>
    /// <param name="id">ID báo cáo</param>
    /// <param name="request">Thông tin cập nhật</param>
    [HttpPut(APIEndPointConstant.Report.UpdateReportEndpoint)]
    public async Task<IActionResult> UpdateReport(int id, ReportRequest request)
    {

        // 🔹 Kiểm tra xem báo cáo có tồn tại không
        var existingReport = await _reportService.GetReportByIdAsync(id);
        if (existingReport == null)
        {
            return NotFound(new { message = "Report not found" });
        }

        await _reportService.UpdateReportAsync(id, request);

        var updatedReport = await _reportService.GetReportByIdAsync(id);

        if (updatedReport == null ||
            updatedReport.HousekeeperId != request.HousekeeperId ||
            updatedReport.AssignId != request.AssignId ||
            updatedReport.WorkDate != request.WorkDate ||
            updatedReport.StartTime != request.StartTime ||
            updatedReport.EndTime != request.EndTime ||
            updatedReport.TotalHours != request.TotalHours ||
            updatedReport.TaskStatus != request.TaskStatus)
        {
            return BadRequest(new { message = "Failed to update report" });
        }

        return Ok(new { message = "Report updated successfully" });
    }
    /// <summary>
    /// Xóa báo cáo theo ID.
    /// </summary>
    /// <param name="id">ID báo cáo</param>
    [HttpDelete(APIEndPointConstant.Report.DeleteReportEndpoint)]
    public async Task<IActionResult> DeleteReport(int id)
    {
        var result = await _reportService.DeleteReportAsync(id);

        if (!result)
        {
            return BadRequest(new { message = "Failed to delete report" });
        }

        return Ok(new { message = "Report deleted successfully" });
    }
    /// <summary>
    /// Lấy thông tin báo cáo theo ID.
    /// </summary>
    /// <param name="id">ID báo cáo</param>
    /// <returns>Thông tin báo cáo nếu tồn tại</returns>
    [HttpGet(APIEndPointConstant.Report.GetReportByIdEndpoint)]
    public async Task<IActionResult> GetReportById(int id)
    {
        var result = await _reportService.GetReportByIdAsync(id);
        if (result == null) return NotFound();

        return Ok(result);
    }
    /// <summary>
    /// Lấy danh sách tất cả báo cáo.
    /// </summary>
    /// <returns>Danh sách báo cáo</returns>
    [HttpGet(APIEndPointConstant.Report.GetAllReportsEndpoint)]
    public async Task<IActionResult> GetAllReports()
    {
        var result = await _reportService.GetAllReportsAsync();
        return Ok(result);
    }
    [HttpGet(APIEndPointConstant.Report.GetReportByHousekeeperIdEndpoint)]
    public async Task<ActionResult<IEnumerable<ReportResponse>>> GetReportsByHousekeeperId([FromRoute] int id)
    {
        var reports = await _reportService.GetReportsByHousekeeperIdAsync(id);
        if (reports == null || !reports.Any())
        {
            return NotFound("No reports found for this housekeeper.");
        }
        return Ok(reports);
    }
}

