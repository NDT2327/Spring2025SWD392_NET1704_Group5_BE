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
    /// <returns>Trả về NoContent nếu thành công</returns>
    [HttpPost(APIEndPointConstant.Report.CreateReportEndpoint)]
    public async Task<IActionResult> CreateReportAsync([FromBody] ReportRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  
        }

        await _reportService.CreateReportAsync(request); 
        return NoContent(); 
    }
    /// <summary>
    /// Cập nhật báo cáo theo ID.
    /// </summary>
    /// <param name="id">ID báo cáo</param>
    /// <param name="request">Thông tin cập nhật</param>
    /// <returns>Trả về NoContent nếu thành công</returns>
    [HttpPut(APIEndPointConstant.Report.UpdateReportEndpoint)]
    public async Task<IActionResult> UpdateReport(int id, ReportRequest request)
    {
        await _reportService.UpdateReportAsync(id, request); 
        return NoContent(); 
    }
    /// <summary>
    /// Xóa báo cáo theo ID.
    /// </summary>
    /// <param name="id">ID báo cáo</param>
    /// <returns>Trả về NoContent nếu thành công, NotFound nếu không tìm thấy</returns>
    [HttpDelete(APIEndPointConstant.Report.DeleteReportEndpoint)]
    public async Task<IActionResult> DeleteReport(int id)
    {
        var result = await _reportService.DeleteReportAsync(id);
        if (!result) return NotFound();

        return NoContent();
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
}
