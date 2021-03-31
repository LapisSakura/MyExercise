using Exercise.Models;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Exercise.Infrastructure.Helpers
{
    public class ImportDataHelper
    {

        /// <summary>
        /// 檢查匯入的 Excel 資料.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="importZipCodes">The import zip codes.</param>
        /// <returns></returns>
        public CheckResult CheckImportData(
            string fileName,
            List<tCustomizeTopic> importTest,
            string MemberID,
            string Category
            )
        {
            var result = new CheckResult();

            var targetFile = new FileInfo(fileName);

            if (!targetFile.Exists)
            {
                result.ID = Guid.NewGuid();
                result.Success = false;
                result.ErrorCount = 0;
                result.ErrorMessage = "匯入的資料檔案不存在";
                return result;
            }

            var excelFile = new ExcelQueryFactory(fileName);

            //欄位對映
            //excelFile.AddMapping<tTest>(x => x.ID, "ID");
            excelFile.AddMapping<tCustomizeTopic>(x => x.Question, "Question");
            excelFile.AddMapping<tCustomizeTopic>(x => x.Answer, "Answer");

            //SheetName
            var excelContent = excelFile.Worksheet<tCustomizeTopic>("自訂題目");

            int errorCount = 0;
            int rowIndex = 1;
            var importErrorMessages = new List<string>();

            //檢查資料
            foreach (var row in excelContent)
            {
                var errorMessage = new StringBuilder();
                var tcp = new tCustomizeTopic();

                //test.ID = row.ID;
                tcp.Question = row.Question;
                tcp.Answer = row.Answer;
                tcp.MemberID = int.Parse(MemberID);
                tcp.Category = Category;

                //題目
                if (string.IsNullOrWhiteSpace(row.Question))
                {
                    errorMessage.Append("題目 - 不可空白. ");
                }
                tcp.Question = row.Answer;

                //答案
                if (string.IsNullOrWhiteSpace(row.Answer))
                {
                    errorMessage.Append("答案 - 不可空白. ");
                }
                tcp.Answer = row.Answer;

                //=============================================================================
                if (errorMessage.Length > 0)
                {
                    errorCount += 1;
                    importErrorMessages.Add(string.Format(
                        "第 {0} 列資料發現錯誤：{1}{2}",
                        rowIndex,
                        errorMessage,
                        "<br/>"));
                }
                importTest.Add(tcp);
                rowIndex += 1;
            }

            try
            {
                result.ID = Guid.NewGuid();
                result.Success = errorCount.Equals(0);
                result.RowCount = importTest.Count;
                result.ErrorCount = errorCount;

                string allErrorMessage = string.Empty;

                foreach (var message in importErrorMessages)
                {
                    allErrorMessage += message;
                }

                result.ErrorMessage = allErrorMessage;

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Saves the import data.
        /// </summary>
        /// <param name="importZipCodes">The import zip codes.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SaveImportData(IEnumerable<tCustomizeTopic> importTest)
        {
            try
            {
                ////先砍掉全部資料
                //using (var db = new dbTeam2_FinalEntities())
                //{
                //    foreach (var item in db.tCustomizeTopic.OrderBy(x => x.種類))
                //    {
                //        db.tCustomizeTopic.Remove(item);
                //    }
                //    db.SaveChanges();
                //}

                //再把匯入的資料給存到資料庫
                using (var db = new dbTeam2_FinalEntities())
                {
                    foreach (var item in importTest)
                    {
                        db.tCustomizeTopic.Add(item);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}