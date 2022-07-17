using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BoardingHouse.Models;

namespace BoardingHouse.Areas.Admin.Controllers
{
    public class RoomsController : Controller
    {
        private BOARDING_HOUSEEntities db = new BOARDING_HOUSEEntities();

        // GET: Admin/Rooms
        public ActionResult Index()
        {
            var rooms = db.Rooms.Include(r => r.Account).Include(r => r.Category).Include(r => r.District);
            return View(rooms.ToList());
        }

        // GET: Admin/Rooms/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // GET: Admin/Rooms/Create
        public ActionResult Create()
        {
            ViewBag.OwnerId = new SelectList(db.Accounts, "Id", "Email");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name");
            return View();
        }

        // POST: Admin/Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OwnerId,CategoryId,DistrictId,Title,Description,Address,Coordinates,Price,Images,CreateAt,UpdateAt,Is_Rented,Is_Delete")] Room room)
        {
            if (ModelState.IsValid)
            {
                room.Id = Guid.NewGuid();
                db.Rooms.Add(room);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.Accounts, "Id", "Email", room.OwnerId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", room.CategoryId);
            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name", room.DistrictId);
            return View(room);
        }

        // GET: Admin/Rooms/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.Accounts, "Id", "Email", room.OwnerId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", room.CategoryId);
            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name", room.DistrictId);
            return View(room);
        }

        // POST: Admin/Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerId,CategoryId,DistrictId,Title,Description,Address,Coordinates,Price,Images,CreateAt,UpdateAt,Is_Rented,Is_Delete")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Accounts, "Id", "Email", room.OwnerId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", room.CategoryId);
            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name", room.DistrictId);
            return View(room);
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase[] postedFile)
        {
            DataSet ds = new DataSet();

            string path = Server.MapPath("~/UploadedFiles/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            for (int j = 0; j < postedFile.Length; j++)
            {

                if (postedFile[j] != null)
                {
                    try
                    {
                        //HttpPostedFileBase postedFile = postedFile[j];
                        string filePath = string.Empty;
                        filePath = path + Path.GetFileName(postedFile[j].FileName);
                        string fileExtension = Path.GetExtension(postedFile[j].FileName);

                        //Validate uploaded file and return error.
                        if (fileExtension != ".xls" && fileExtension != ".xlsx")
                        {
                            ViewBag.Message = "Please select the excel file with .xls or .xlsx extension";
                            return View();
                        }


                        //Save file to folder
                        //var filePath = folderPath + Path.GetFileName(postedFile.FileName);
                        postedFile[j].SaveAs(filePath);

                        //Get file extension

                        string excelConString = "";

                        //Get connection string using extension 
                        switch (fileExtension)
                        {
                            //If uploaded file is Excel 1997-2007.
                            case ".xls":
                                excelConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                                break;
                            //If uploaded file is Excel 2007 and above
                            case ".xlsx":
                                excelConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                                break;
                        }

                        //Read data from first sheet of excel into datatable
                        DataTable dt = new DataTable();
                        excelConString = string.Format(excelConString, filePath);

                        using (OleDbConnection excelOledbConnection = new OleDbConnection(excelConString))
                        {
                            using (OleDbCommand excelDbCommand = new OleDbCommand())
                            {
                                using (OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter())
                                {
                                    excelDbCommand.Connection = excelOledbConnection;

                                    excelOledbConnection.Open();
                                    //Get schema from excel sheet
                                    DataTable excelSchema = GetSchemaFromExcel(excelOledbConnection);
                                    //Get sheet name
                                    string sheetName = excelSchema.Rows[0]["TABLE_NAME"].ToString();
                                    excelOledbConnection.Close();

                                    //Read Data from First Sheet.
                                    excelOledbConnection.Open();
                                    excelDbCommand.CommandText = "SELECT * From [" + sheetName + "]";
                                    excelDataAdapter.SelectCommand = excelDbCommand;
                                    //Fill datatable from adapter
                                    excelDataAdapter.Fill(dt);
                                    excelOledbConnection.Close();
                                }
                            }
                        }

                        GetSubmittedBusRouteDataFromExcelRow(dt);
                        //Insert records to Submitted Route table.
                        //using (var context = new DemoContext())
                        //{
                        //    var a = GetSubmittedBusRouteDataFromExcelRow(dt);
                        //    context.STARS_SubmittedRouteData.Add(a);
                        //    context.SaveChanges();
                        //}
                        ViewBag.Message = "Data Imported Successfully.";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = ex.Message;
                    }
                }

                else

                {
                    ViewBag.Message = "Please select the file first to upload.";
                }
            }
            var rooms = db.Rooms.Include(r => r.Account).Include(r => r.Category).Include(r => r.District);
            return View(rooms.ToList());
        }

        private static DataTable GetSchemaFromExcel(OleDbConnection excelOledbConnection)
        {
            return excelOledbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        }

        //private STARS_SubmittedRouteData GetSubmittedBusRouteDataFromExcelRow(DataTable dt)
        private void GetSubmittedBusRouteDataFromExcelRow(DataTable dt)
        {
            try
            {
                int i = 0;
                DateTime dateTime = DateTime.Now;
                //Loop through datatable and add employee data to employee table. 
                foreach (DataRow row in dt.Rows)
                {
                    i++;
                    Account account = new Account
                    {
                        Id = Guid.NewGuid(),
                        RoleId = Guid.Parse("24169e57-4e33-48c3-aae4-5ae19be6c87d"),
                        Email = row.ItemArray[4].ToString(),
                        Password = GeneratePassword(8),
                        FullName = row.ItemArray[3].ToString(),
                        Address = row.ItemArray[2].ToString(),
                        CreateAt = dateTime,
                        Is_Active = true,
                    };
                    db.Accounts.Add(account);
                    db.SaveChanges();
                    // int Accountid = db.Accounts


                    Room room = new Room
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = account.Id,
                        CategoryId = Guid.Parse(GenerateCate()),
                        DistrictId = Guid.Parse(GenerateDis()),
                        Title = "Nhà Trọ Giá Rẻ " + account.FullName,
                        Description = row.ItemArray[0].ToString() + " Liên Hệ : 0"+ row.ItemArray[1].ToString(),
                        Address = row.ItemArray[0].ToString(),
                        Price = float.Parse(GeneratePrice()),
                        Images = i+".jpg",
                        
                        CreateAt = DateTime.Now,
                        Is_Rented = false,

                    };
                    db.Rooms.Add(room);
                }

                db.SaveChanges();


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        // GET: Admin/Rooms/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Admin/Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Room room = db.Rooms.Find(id);
            db.Rooms.Remove(room);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public static string GeneratePassword(int passLength)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz@#$&ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, passLength)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
        public static string GenerateDis()
        {
            Random random = new Random();
            var names = new List<string> { "32b43bbb-b4f1-45c2-a085-29266f55f4d0", "347a5147-e357-4267-892f-31daa7da57b4", "0fa0b09b-4c17-4220-88cd-351a4fd7c0ee", "0131fcf9-5021-4e2d-b48a-6578da6ab544", "4b14ff6a-7309-41a9-a087-6fa3434c00ef", "c1afd606-73f8-421f-ba2e-85406bc113b9", "c7753b1f-9434-4b5f-9969-b5c139c65e75", "b224562a-3c0c-45a0-8683-c93db0e867ea", "50edf6c1-55f2-4d62-b6b6-ed8d89442022" };
            int index = random.Next(names.Count);
            var name = names[index];
            return name;
        }
        public static string GenerateCate()
        {
            Random random = new Random();
            var names = new List<string> { "69c4765a-9ed9-4263-93f2-91b1423066f8", "0b4a7065-c29d-47da-aa5b-e136061ca5ff", "b34b1d3e-3446-4a37-b2d0-ec017110bfc5" };
            int index = random.Next(names.Count);
            var name = names[index];
            return name;
        }
        public static string GeneratePrice()
        {
            Random random = new Random();
            var names = new List<string> { "500000", "600000", "700000", "800000", "900000", "1000000", "1600000", "2000000", "2600000" };
            int index = random.Next(names.Count);
            var name = names[index];
            return name;
        }
    }
}
