﻿using JobPortal.BAL.Abstraction;
using JobPortal.BAL.Common;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobPortal.Areas.GlobalAdmin.Controllers
{
    [Web.UserLogin]
    [Web.RoleAuthorization]
    public class ManageJobsController : Controller
    {
        // GET: GlobalAdmin/ManageJobs
        private readonly BAL.Abstraction.IBALJobs _balService;
        private readonly BAL.Abstraction.IBALRegions _bALRegions;

        private readonly BAL.Abstraction.IBALJobTimeMapping _iBALJobTimeMapping;
        private readonly BAL.Abstraction.IBALJobTimeEmpMapping _iBALJobTimeEmpMapping;
        private readonly BAL.Abstraction.IBALTrainings _bALTraining;

        public ManageJobsController(IBALRegions IBALRegions, BAL.Abstraction.IBALJobs balService, IBALJobTimeMapping iBALJobTimeMapping
            , IBALJobTimeEmpMapping iBALJobTimeEmpMapping, IBALTrainings ibALTraining)
        {
            _balService = balService;
            _bALRegions = IBALRegions;
            _iBALJobTimeMapping = iBALJobTimeMapping;
            _iBALJobTimeEmpMapping = iBALJobTimeEmpMapping;
            _bALTraining = ibALTraining;
        }


        // GET: SuperAdmin/ManageJobs
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllJobs()
        {

            if (Session["RegionId"] != null)
            {
                var academyTeams = _balService.GetAllJobsbyRegion(Convert.ToInt32(Session["RegionId"]));
                var gridModel = new DataSourceResult { Data = academyTeams, Total = academyTeams.Count };
                var jsonResult = Json(gridModel, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            else
            {
                var academyTeams = _balService.GetAllJobs();
                var gridModel = new DataSourceResult { Data = academyTeams, Total = academyTeams.Count };
                var jsonResult = Json(gridModel, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }


        }
        public JsonResult GetAllJobsWithTrainings()
        {
            if (Session["RegionId"] != null)
            {
                var academyTeams = _balService.GetJobsWithTrainingName(Convert.ToInt32(Session["RegionId"]));
                var gridModel = new DataSourceResult { Data = academyTeams, Total = academyTeams.Count };
                var jsonResult = Json(gridModel, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            else {
                return null;
            }
        }

        public ActionResult JobTimeMapping()
        {
            return View();
        }

        [HttpPost]
        public JsonResult JobTimeMapping(Core.Entity.JobTimeMappingEntity model)
        {
            var jsonResult = Json(_iBALJobTimeMapping.InsertJobTimeMapping(model), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult JobTimeAndEmpMapping()
        {
            return View();
        }


        public JsonResult GetAllJobTimeMapping()
        {
            var jsonResult = Json(_iBALJobTimeMapping.GetAllJobTimeMapping(), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetAllJobTimeEmpMapping()
        {
            var jsonResult = Json(_iBALJobTimeEmpMapping.GetAllJobTimeEmpMapping(), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpPost]
        public JsonResult JobTimeAndEmpMapping(Core.Entity.JobTimeMappingEntity model)
        {
            var jsonResult = Json(_iBALJobTimeEmpMapping.InsertJobTimeEmpMapping(model), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult InsertUpdateJobs(Core.Entity.JobEntity model)
        {
            string multipleTraining = string.Empty;

            if (model.multipleTrainings != null && model.multipleTrainings.Count() > 0)
            {
                foreach (var item in model.multipleTrainings)
                {
                    multipleTraining += item + ",";
                }
                model.MultipleTrainingsAssignedCommaSeperated = multipleTraining;
            }


            int Id = _balService.InsertUpdate(model);
            var jsonResult = Json(Id, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult DeleteJobTimeMapping(int Id)
        {
            var result = _iBALJobTimeMapping.DeleteJobTimeMapping(Id);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult DeleteJobTimeAndEmpMapping(int Id)
        {
            var result = _iBALJobTimeEmpMapping.DeleteJobTimeAndEmpMapping(Id);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult CheckIfMatchedAnyTraining(int Jobid, int Userid)
        {
            var result = _iBALJobTimeEmpMapping.CheckIfMatchedAnyTraining(Jobid, Userid);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetJobById(int jobId)
        {
            var result = _balService.GetJobById(jobId);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult DeleteJob(int Id)
        {
            var result = _balService.DeleteJob(Id);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase FileUpload)
        {
            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  

                string filename = FileUpload.FileName;
                if (!Directory.Exists(Server.MapPath("~/Doc/")))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(Server.MapPath("~/Doc/"));
                }
                string targetpath = Server.MapPath("~/Doc/");
                FileUpload.SaveAs(targetpath + filename);
                string pathToExcelFile = targetpath + filename;

                //  string sheetName = "Sheet1";

                //var excelFile = new ExcelQueryFactory(pathToExcelFile);
                //var artistAlbums = from a in excelFile.Worksheet<JobPortal.Core.Entity.JobEntity>(sheetName) select a;

                DataTable dtable = GetDataTabletFromCSVFile(pathToExcelFile);

                foreach (DataRow dr in dtable.Rows)
                {
                    JobPortal.Core.Entity.JobEntity jobEntity = new Core.Entity.JobEntity();



                    string region = Convert.ToString(dr["Region Name"]);
                    if (!string.IsNullOrEmpty(region))
                    {
                        var regiondetail = _bALRegions.GetRegionByName(region);
                        if (regiondetail != null)
                        {
                            jobEntity.RegionId = regiondetail.Id;
                        }
                    }

                    string training = Convert.ToString(dr["TrainingName"]);
                    string multipletrainings = string.Empty;
                    foreach (string item in training.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var trainingdetail = _bALTraining.GetTrainingsByName(item);
                            if (trainingdetail != null)
                            {
                                multipletrainings += trainingdetail.ID + ",";
                            }
                        }
                    }




                    jobEntity.MultipleTrainingsAssignedCommaSeperated = multipletrainings.TrimEnd(',');
                    jobEntity.JobTitle = dr["JobTitle"].ToString();
                    jobEntity.JobDescription = dr["JobDescription"].ToString();
                    jobEntity.JobNumber = Convert.ToString(dr["JobNumber"]);

                    int Status = 0;
                    switch (Convert.ToString(dr["Status"]).ToLower())
                    {
                        case "open":
                            Status = 1;
                            break;
                        case "in progress":
                            Status = 2;
                            break;
                        case "completed":
                            Status = 3;
                            break;
                        case "close":
                            Status = 4;
                            break;
                    }

                    jobEntity.Status = Status;

                    _balService.InsertUpdate(jobEntity);
                }
                //deleting excel file from folder  
                if ((System.IO.File.Exists(pathToExcelFile)))
                {
                    System.IO.File.Delete(pathToExcelFile);
                }
                return RedirectToAction("index");
            }
            else
            {
                //alert message for invalid file format  
                data.Add("<ul>");
                data.Add("<li>Only Excel file format is allowed</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }


        private static DataTable GetDataTabletFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return csvData;
        }

        public ActionResult JobsAssignedToEmp()
        {

            return View();
        }

        public JsonResult GetAllJobsAssigneToEmpByRegion()
        {
            if (Session["RegionId"] != null)
            {
                var academyTeams = _iBALJobTimeEmpMapping.GetAllJobsAssigneToEmpByRegion(Convert.ToInt32(Session["RegionId"]));
                var gridModel = new DataSourceResult { Data = academyTeams, Total = academyTeams.Count };
                var jsonResult = Json(gridModel, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            else
            {
                return null;
            }
        }
    }
}