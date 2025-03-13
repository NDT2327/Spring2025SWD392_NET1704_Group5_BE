using CCSystem.API.Constants;
using CCSystem.BLL.DTOs.Report;
using CCSystem.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using static CCSystem.BLL.Constants.MessageConstant;

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
            return Ok(new { message = ReportMessage.ReportCreated });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ReportMessage.CreateFailed, error = ex.Message });
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

        var existingReport = await _reportService.GetReportByIdAsync(id);
        if (existingReport == null)
        {
            return NotFound(new { message = ReportMessage.ReportNotFound });
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
            return BadRequest(new { message = ReportMessage.UpdateFailed });
        }

        return Ok(new { message = ReportMessage.UpdatedSuccessfully });
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
            return BadRequest(new { message = ReportMessage.DeleteFailed });
        }

        return Ok(new { message = ReportMessage.DeletedSuccessfully });
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
    /// <summary>
    /// Lấy danh sách báo cáo theo HousekeeperId.
    /// </summary>
    /// <param name="id">ID của Housekeeper</param>
    /// <returns>Danh sách báo cáo nếu có, hoặc lỗi 404 nếu không tìm thấy</returns>
    [HttpGet(APIEndPointConstant.Report.GetReportByHousekeeperIdEndpoint)]
    public async Task<ActionResult<IEnumerable<ReportResponse>>> GetReportsByHousekeeperId([FromRoute] int id)
    {
        var reports = await _reportService.GetReportsByHousekeeperIdAsync(id);
        if (reports == null || !reports.Any())
        {
            return NotFound(new { message = ReportMessage.ReportNotFound });
        }
        return Ok(reports);
    }
    /// <summary>
    /// Lấy danh sách báo cáo theo AssignId.
    /// </summary>
    /// <param name="id">ID của Assign</param>
    /// <returns>Danh sách báo cáo nếu có, hoặc lỗi 404 nếu không tìm thấy</returns>
    [HttpGet(APIEndPointConstant.Report.GetReportByAssignIdEndpoint)]
    public async Task<IActionResult> GetByAssignId(int id)
    {
        var reports = await _reportService.GetByAssignIdAsync(id);
        return reports.Count > 0 ? Ok(reports) : NotFound(new { message = string.Format(ReportMessage.ReportNotFound, id) });
    }
}

