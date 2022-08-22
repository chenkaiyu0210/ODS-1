using backendWeb.Models.Repositories;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace backendWeb.Areas.Report.Controllers
{
    public class ContractController : Controller
    {
        // GET: Report/ContractReport
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Download(DateTime receive_date_start, DateTime receive_date_end)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter { ParameterName = "receive_date_start", Value = receive_date_start.ToString("yyyy-MM-dd") });
            parameters.Add(new SqlParameter { ParameterName = "receive_date_end", Value = receive_date_end.ToString("yyyy-MM-dd") });

            DataTable s = new baseRepository<DataTable>(new List<string> { @"
SELECT [examine_no]
	,[receive_status]
	,[customer_name]
	,[customer_idcard_no]
	,[customer_mobile_phone]
	,CONVERT(VARCHAR, [customer_birthday], 23) customer_birthday
	,R.bus_type + ' ' + B.bus_type_name AS bus_type
	,[promotion]
	,p.promo_name
	,[staging_amount]
	,[num]
	,[num_amount]
	,[receive_staff] + ' ' + U.name AS receive_staff
	,[commission_target]
	,CONVERT(VARCHAR, [receive_date], 23) receive_date
FROM [dbo].[receiveCases] R
LEFT JOIN [dbo].[backendPromotion] P ON R.promotion = P.promo_no
LEFT JOIN [dbo].[backendBusType] B ON R.bus_type = B.bus_type
LEFT JOIN [dbo].[backendUser] U ON R.receive_staff = U.account
WHERE is_delete = 0
    AND receive_status = N'核准'
	AND CONVERT(DATE, receive_date) BETWEEN @receive_date_start
		AND @receive_date_end
ORDER BY receive_date DESC
" }, new List<List<SqlParameter>> { parameters }).DataTable();

            return File(ExportExcel(s), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + "立約報表.xlsx");
        }

        public static byte[] ExportExcel(DataTable dataTable)
        {
            byte[] result = null;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("立約報表");
                workSheet.Cells.LoadFromDataTable(dataTable, true);

                workSheet.Cells[1, 1].Value = "審件編號";
                workSheet.Cells[1, 2].Value = "案件狀態";
                workSheet.Cells[1, 3].Value = "申請人姓名";
                workSheet.Cells[1, 4].Value = "身分證字號";
                workSheet.Cells[1, 5].Value = "手機號碼";
                workSheet.Cells[1, 6].Value = "生日";
                workSheet.Cells[1, 7].Value = "業務別";
                workSheet.Cells[1, 8].Value = "促銷專案代號";
                workSheet.Cells[1, 9].Value = "促銷專案名稱";
                workSheet.Cells[1, 10].Value = "辦理分期金額";
                workSheet.Cells[1, 11].Value = "期數";
                workSheet.Cells[1, 12].Value = "月付款";
                workSheet.Cells[1, 13].Value = "進件人員";
                workSheet.Cells[1, 14].Value = "撥佣對象";
                workSheet.Cells[1, 15].Value = "進件日期";

                workSheet.Cells.AutoFitColumns();
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                result = package.GetAsByteArray();
            }
            return result;
        }
    }
}