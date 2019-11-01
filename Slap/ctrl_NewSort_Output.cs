﻿using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Slap
{
    public partial class ctrl_NewSort_Output : UserControl
    {
        // variables for parcels
        private string[] ParcelData, RouteData;
        private Parcel[] parcelArray;
        private List<Parcel> filteredParcelList;
        private List<Parcel> sortedParcelList;

        // variables for routeGroups and routes
        private List<List<string>> routesList;
        private List<RouteGroup> routeGroupList;
        private List<RouteGroup> sortedRouteGroupList;
        
        public ctrl_NewSort_Output()
        {
            InitializeComponent();
            Reset();
        }

        public void addData(string[] parcelData, string[] routeData)
        {
            ParcelData = parcelData;
            RouteData = routeData;
        }

        // reset
        private void Reset()
        {
            pb_DL_ParcelList.Image = Properties.Resources.filePurple;
            pb_DL_RouteList.Image = Properties.Resources.filePurple;
            pb_DL_FloorPlan.Image = Properties.Resources.filePurple;
            pb_DL_SortPlan.Image = Properties.Resources.filePurple;
        }

        // New Sort and Clear buttons
        private void btn_Process_MouseDown(object sender, MouseEventArgs e)
        {
            btn_Process.Enabled = false;
            ReadRoutes();
            //displayArray();
            FilterParcels();
            btn_Process.Enabled = true;
        }

        private void btn_Back_MouseDown(object sender, MouseEventArgs e)
        {
            Hide();
        }
        
        // On Click functions for Individual File Download
        private void pb_DL_ParcelList_MouseDown(object sender, MouseEventArgs e)
        {
            pb_DL_ParcelList.Image = Properties.Resources.fileLightPurple;
        }

        private void pb_DL_RouteList_MouseDown(object sender, MouseEventArgs e)
        {
            pb_DL_RouteList.Image = Properties.Resources.fileLightPurple;

        }

        private void pb_DL_FloorPlan_MouseDown(object sender, MouseEventArgs e)
        {
            pb_DL_FloorPlan.Image = Properties.Resources.fileLightPurple;

        }

        private void pb_DL_SortPlan_MouseDown(object sender, MouseEventArgs e)
        {
            pb_DL_SortPlan.Image = Properties.Resources.fileLightPurple;

        }

        private void pb_DL_ParcelList_MouseUp(object sender, MouseEventArgs e)
        {

            pb_DL_ParcelList.Image = Properties.Resources.filePurple;
        }

        private void pb_DL_RouteList_MouseUp(object sender, MouseEventArgs e)
        {

            pb_DL_RouteList.Image = Properties.Resources.filePurple;
        }

        private void pb_DL_FloorPlan_MouseUp(object sender, MouseEventArgs e)
        {
            pb_DL_FloorPlan.Image = Properties.Resources.filePurple;
        }

        private void pb_DL_SortPlan_MouseUp(object sender, MouseEventArgs e)
        {
            pb_DL_SortPlan.Image = Properties.Resources.filePurple;
        }

        // Sorting Method
        public bool ReadParcels()
        {
            if (ParcelData != null)
            {
                // extract all data from the file
                string txtData = "";
                foreach (string line in ParcelData)
                {
                    try
                    {
                        txtData = File.ReadAllText(line);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }

                string[] txtDataLines = txtData.Split('\n');

                // to remove occurence of empty last line
                if (txtDataLines[txtDataLines.Length - 1] == "")
                {
                    string[] temp = new string[txtDataLines.Length - 1];
                    Array.Copy(txtDataLines, 0, temp, 0, txtDataLines.Length - 1);
                    txtDataLines = temp;
                }

                // first line to create header
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                
                string[] headerLabels = CSVParser.Split(txtDataLines[0]);

                for (int i = 0; i < headerLabels.Length; i++)
                {
                    // remove carriage return
                    headerLabels[i] = Regex.Replace(headerLabels[i], "[^a-zA-Z0-9 +–=_.,!\"\'/$]", "");
                }

                parcelArray = new Parcel[txtDataLines.Length - 1];

                DataTable dataTable = new DataTable();

                foreach (string headerWord in headerLabels)
                {
                    dataTable.Columns.Add(new DataColumn(headerWord));
                }

                // second line onwards to process data
                // to be able to read data encapsulated by ""
                for (int row = 1; row < txtDataLines.Length; row++)
                {
                    string[] dataWords = CSVParser.Split(txtDataLines[row]);

                    // clean up the fields (remove " and leading spaces)
                    for (int i = 0; i < dataWords.Length; i++)
                    {
                        // remove carriage return
                        dataWords[i] = Regex.Replace(dataWords[i], "[^a-zA-Z0-9 +–=_.,!\"\'/$]", "");

                        dataWords[i] = dataWords[i].TrimStart(' ', '"');
                        dataWords[i] = dataWords[i].TrimEnd('"');
                    }

                    // read the column data of each row
                    DataRow dataRow = dataTable.NewRow();
                    int col = 0;

                    string AWB = "", ConsigneeCompany = "", ConsigneeAddress = "", ConsigneePostal = "";
                    string SelectCd = "", DestLocCd = "", CourierRoute = "";
                    int PieceQty = 0;
                    double KiloWgt = 0.0;

                    foreach (string headerWord in headerLabels)
                    {
                        try
                        {
                            if (dataWords[col] == null || dataWords[col] == "")
                            {
                                dataRow[headerWord] = "";
                                col++;
                            }
                            else
                            {
                                dataRow[headerWord] = dataWords[col];

                                switch (headerWord)
                                {
                                    case "AWB":
                                        AWB = dataWords[col];
                                        break;
                                    case "ConsigneeCompany":
                                        ConsigneeCompany = dataWords[col];
                                        break;
                                    case "ConsigneeAddr1":
                                        ConsigneeAddress = dataWords[col];
                                        break;
                                    case "ConsigneePostal":
                                        ConsigneePostal = dataWords[col];
                                        break;
                                    case "SelectCd":
                                        SelectCd = dataWords[col];
                                        break;
                                    case "DestLocCd":
                                        DestLocCd = dataWords[col];
                                        break;
                                    case "CourierRoute":
                                        CourierRoute = dataWords[col];
                                        break;
                                    case "PieceQty":
                                        PieceQty = Convert.ToInt32(dataWords[col]);
                                        break;
                                    case "KiloWgt":
                                        KiloWgt = Convert.ToDouble(dataWords[col]);
                                        break;
                                }

                                col++;
                            }

                            // Add the Parcel to the Parcel Array
                            Parcel parcel = new Parcel(
                                AWB, ConsigneeCompany, ConsigneeAddress, ConsigneePostal,
                                SelectCd, DestLocCd, CourierRoute, PieceQty, KiloWgt);
                            
                            parcelArray[row - 1] = parcel;
                        }
                        catch (Exception e)
                        {
                            dataRow[headerWord] = null;
                        }
                    }

                    // add data row into data table
                    dataTable.Rows.Add(dataRow);
                }

                // load data table into data grid view
                dgv_FileData.DataSource = dataTable;
            }

            return true;
        }

        private void ReadRoutes()
        {
            // given values
            //routeList.Add(new List<string> { "835", "837", "839" });
            //routeList.Add(new List<string> { "850", "857", "859" });
            //routeList.Add(new List<string> { "823", "833" });
            //routeList.Add(new List<string> { "815" });
            //routeList.Add(new List<string> { "860", "863", "865" });
            //routeList.Add(new List<string> { "872", "875", "877" });
            //routeList.Add(new List<string> { "880", "883", "885", "890", "893", "895" });
            //routeList.Add(new List<string> { "XKLA" });
            //routeList.Add(new List<string> { "KULAAA" });
            //routeList.Add(new List<string> { "KULBK+" });
            //routeList.Add(new List<string> { "RAWANG" });

            routesList = new List<List<string>>();

            routesList.Add(new List<string> { "810", "811", "812", "813", "814", "815", "816", "817", "818", "819" });
            routesList.Add(new List<string> { "820", "821", "822", "823", "824" });
            routesList.Add(new List<string> { "835", "836", "837", "838", "839" });
            routesList.Add(new List<string> { "850", "851", "852", "853", "854", "855", "856", "857", "858", "859" });
            routesList.Add(new List<string> { "860", "861", "862", "863", "864", "865", "866", "867", "868", "869" });
            routesList.Add(new List<string> { "870", "871", "872", "873", "874", "875", "876", "877", "878", "879" });
            routesList.Add(new List<string> {
                "880", "881", "882", "883", "884", "885", "886", "887", "888", "889",
                "890", "891", "892", "893", "894", "895", "896", "897", "898", "899" });

            routesList.Add(new List<string> { "XKLA" });
            routesList.Add(new List<string> { "KULAAA" });
            routesList.Add(new List<string> { "KULBK+" });
            routesList.Add(new List<string> { "RAWANG" });

            // add one RouteGroup for parcels that are on HOLD
            routeGroupList = new List<RouteGroup>();

            int routeGroupID = 0;

            routeGroupList.Add(new RouteGroup(routeGroupID++, new List<string>()));
            foreach (List<string> routes in routesList)
            {
                routeGroupList.Add(new RouteGroup(routeGroupID++, routes));
            }
        }

        private void DisplayArray()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("AWB"));
            dt.Columns.Add(new DataColumn("ConsigneeCompany"));
            dt.Columns.Add(new DataColumn("ConsigneeAddress"));
            dt.Columns.Add(new DataColumn("ConsigneePostal"));
            dt.Columns.Add(new DataColumn("SelectCd"));
            dt.Columns.Add(new DataColumn("Cleared"));
            dt.Columns.Add(new DataColumn("DestLocCd"));
            dt.Columns.Add(new DataColumn("CourierRoute"));
            dt.Columns.Add(new DataColumn("PieceQty"));
            dt.Columns.Add(new DataColumn("KiloWgt"));

            for (int i = 0; i < parcelArray.Length; i++)
            {
                DataRow dr = dt.NewRow();
                dr["AWB"] = parcelArray[i].AWB;
                dr["ConsigneeCompany"] = parcelArray[i].ConsigneeCompany;
                dr["ConsigneeAddress"] = parcelArray[i].ConsigneeAddress;
                dr["ConsigneePostal"] = parcelArray[i].ConsigneePostal;
                dr["SelectCd"] = parcelArray[i].SelectCd;
                dr["Cleared"] = parcelArray[i].ClearedStatus;
                dr["DestLocCd"] = parcelArray[i].DestLocCd;
                dr["CourierRoute"] = parcelArray[i].CourierRoute;
                dr["PieceQty"] = parcelArray[i].PieceQty;
                dr["KiloWgt"] = parcelArray[i].KiloWgt;

                dt.Rows.Add(dr);
            }

            dgv_FileData.DataSource = dt;

            FilterParcels();
        }

        private void FilterParcels()
        {
            filteredParcelList = new List<Parcel>();
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("AWB"));
            dt.Columns.Add(new DataColumn("ConsigneeCompany"));
            dt.Columns.Add(new DataColumn("ConsigneeAddress"));
            dt.Columns.Add(new DataColumn("ConsigneePostal"));
            dt.Columns.Add(new DataColumn("SelectCd"));
            dt.Columns.Add(new DataColumn("Cleared"));
            dt.Columns.Add(new DataColumn("DestLocCd"));
            dt.Columns.Add(new DataColumn("CourierRoute"));
            dt.Columns.Add(new DataColumn("PieceQty"));
            dt.Columns.Add(new DataColumn("KiloWgt"));

            for (int i = 0; i < parcelArray.Length; i++)
            {
                bool isBulk = false;

                // check for Bulk requirements
                // check for Quantity and KiloWeight
                if (parcelArray[i].PieceQty >= 50 ||
                    (parcelArray[i].PieceQty == 1 && parcelArray[i].KiloWgt >= 34) ||
                    (parcelArray[i].PieceQty > 1 && parcelArray[i].KiloWgt >= 225))
                {
                    isBulk = true;
                }

                // check for multiple shipper to single Consignee Address
                for (int j = 0; j < parcelArray.Length; j++)
                {
                    if (parcelArray[i].ConsigneeAddress == parcelArray[j].ConsigneeAddress &&
                        parcelArray[i].AWB != parcelArray[j].AWB)
                    {
                        isBulk = true;
                    }
                }

                if (isBulk)
                {
                    DataRow dr = dt.NewRow();

                    dr["AWB"] = parcelArray[i].AWB;
                    dr["ConsigneeCompany"] = parcelArray[i].ConsigneeCompany;
                    dr["ConsigneeAddress"] = parcelArray[i].ConsigneeAddress;
                    dr["ConsigneePostal"] = parcelArray[i].ConsigneePostal;
                    dr["SelectCd"] = parcelArray[i].SelectCd;
                    dr["Cleared"] = parcelArray[i].ClearedStatus;
                    dr["DestLocCd"] = parcelArray[i].DestLocCd;
                    dr["CourierRoute"] = parcelArray[i].CourierRoute;
                    dr["PieceQty"] = parcelArray[i].PieceQty;
                    dr["KiloWgt"] = parcelArray[i].KiloWgt;

                    dt.Rows.Add(dr);

                    filteredParcelList.Add(parcelArray[i]);
                }
            }

            dgv_FileData.DataSource = dt;

            SortParcels();
        }

        private void SortParcels()
        {
            sortedParcelList = new List<Parcel>();

            Dictionary<string, int> routeGroupRoutesDict = new Dictionary<string, int>();

            // iterate throuch each RouteGroup
            // collect all routes into one dictionary
            foreach(RouteGroup routeGroup in routeGroupList)
            {
                foreach(string courierRoute in routeGroup.CourierRoutes)
                {
                    routeGroupRoutesDict.Add(courierRoute, routeGroup.RouteGroupID);
                }
            }

            // iterate through each parcel to be sorted into routeGroups
            // assign parcels to routegroups
            foreach(Parcel parcel in filteredParcelList)
            {
                if (routeGroupRoutesDict.ContainsKey(parcel.CourierRoute))
                {
                    if (parcel.ClearedStatus)
                    {
                        //parcel.RouteGroup = routeGroupRoutesDict[parcel.CourierRoute];
                        routeGroupList[routeGroupRoutesDict[parcel.CourierRoute]].ParcelList.Add(parcel);
                    }
                    else
                    {
                        //parcel.RouteGroup = 0;
                        routeGroupList[0].ParcelList.Add(parcel);
                    }
                    sortedParcelList.Add(parcel);
                }
            }

            // iterate through each RouteGroup
            // assign lanes based on estimated volume
            sortedRouteGroupList = new List<RouteGroup>();

            char lane = 'A';
            foreach(RouteGroup routeGroup in routeGroupList)
            {
                foreach(Parcel parcel in routeGroup.ParcelList)
                {
                    routeGroup.LaneEstimateVolumeCur += parcel.EstimatedVol;
                }
                Console.WriteLine(routeGroup.LaneEstimateVolumeCur);

                int numOfLanes = (int)Math.Ceiling(routeGroup.LaneEstimateVolumeCur / routeGroup.LaneEstimateVolumeMax);
                string lanes = "";

                if(routeGroup.RouteGroupID == 0)
                {
                    lanes = "HOLD";
                    sortedRouteGroupList.Add(routeGroup);
                }
                else
                {
                    for(int i = 0; i < numOfLanes; i++)
                    {
                        lanes += lane;
                        lane++;
                    }

                    if(!lanes.Equals(""))
                    {
                        sortedRouteGroupList.Add(routeGroup);
                    }
                }

                foreach(Parcel parcel in routeGroup.ParcelList)
                {
                    parcel.Lanes = lanes;
                }

                // print console for debugging purposes
                Console.WriteLine("RouteGroup: " + routeGroup.RouteGroupID);
                string routesConsole = "";
                foreach(string route in routeGroup.CourierRoutes)
                {
                    routesConsole += route + " ";
                }
                Console.WriteLine("Routes: " + routesConsole);
                double kiloWgtConsole = 0.0;
                foreach(Parcel parcel in routeGroup.ParcelList)
                {
                    kiloWgtConsole += parcel.KiloWgt;
                }
                Console.WriteLine("KiloWgt: " + kiloWgtConsole);
                Console.WriteLine("Lanes: " + lanes);
                Console.WriteLine();
            }

            DisplayParcels();
        }

        private void DisplayParcels()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("AWB"));
            dt.Columns.Add(new DataColumn("ConsigneeCompany"));
            dt.Columns.Add(new DataColumn("ConsigneeAddress"));
            dt.Columns.Add(new DataColumn("ConsigneePostal"));
            dt.Columns.Add(new DataColumn("SelectCd"));
            dt.Columns.Add(new DataColumn("Cleared"));
            dt.Columns.Add(new DataColumn("CourierRoute"));
            dt.Columns.Add(new DataColumn("Lanes"));
            dt.Columns.Add(new DataColumn("PieceQty"));
            dt.Columns.Add(new DataColumn("KiloWgt"));

            for (int i = 0; i < sortedParcelList.Count; i++)
            {
                DataRow dr = dt.NewRow();

                dr["AWB"] = sortedParcelList[i].AWB;
                dr["ConsigneeCompany"] = sortedParcelList[i].ConsigneeCompany;
                dr["ConsigneeAddress"] = sortedParcelList[i].ConsigneeAddress;
                dr["ConsigneePostal"] = sortedParcelList[i].ConsigneePostal;
                dr["SelectCd"] = sortedParcelList[i].SelectCd;
                dr["Cleared"] = sortedParcelList[i].ClearedStatus;
                dr["CourierRoute"] = sortedParcelList[i].CourierRoute;
                dr["Lanes"] = sortedParcelList[i].Lanes;
                dr["PieceQty"] = sortedParcelList[i].PieceQty;
                dr["KiloWgt"] = sortedParcelList[i].KiloWgt;

                dt.Rows.Add(dr);
            }

            dgv_FileData.DataSource = dt;
        }

        private static void GeneratePDF_FloorPlan()
        {
            // get current date time
            DateTime dateTime = DateTime.Now;
            String dateTimeStr = dateTime.ToString("dddd, dd MMMM yyyy - HH:mm");
            String dateTimeNum = dateTime.ToString("yyyyMMdd_HHmmss");
            String fileName = "FloorPlan_" + dateTimeNum + ".pdf";

            // handle file and folder locations
            string startPath = Application.StartupPath;
            string folderPath = Path.GetFullPath(Path.Combine(startPath, @"..\..\"));
            folderPath = System.IO.Path.Combine(folderPath, "FloorPlan");
            fileName = System.IO.Path.Combine(folderPath, fileName);

            System.IO.Directory.CreateDirectory(folderPath);

            // create PDF file
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            // create table
            PdfPTable table = new PdfPTable(10 + 1);

            // create table title
            PdfPCell titleCell = new PdfPCell(new Phrase("FLOOR PLAN - " + dateTimeStr));
            titleCell.Rowspan = 4 * 7;
            titleCell.Rotation = 270;
            titleCell.HorizontalAlignment = 1;

            // sorting algorithm
            const int gridRows = 10;
            const int gridCols = 4 * 7;
            int numOfTrucks = 5;
            int[] truckLane = new int[numOfTrucks];
            truckLane[0] = 1;
            truckLane[1] = 3;
            truckLane[2] = 2;
            truckLane[3] = 4;
            truckLane[4] = 2;

            BaseColor[] colors = new BaseColor[15];
            colors[0] = new iTextSharp.text.BaseColor(128, 128, 128);
            colors[1] = new iTextSharp.text.BaseColor(255, 0, 0);
            colors[2] = new iTextSharp.text.BaseColor(255, 128, 0);
            colors[3] = new iTextSharp.text.BaseColor(255, 255, 0);
            colors[4] = new iTextSharp.text.BaseColor(128, 255, 0);
            colors[5] = new iTextSharp.text.BaseColor(0, 255, 0);
            colors[6] = new iTextSharp.text.BaseColor(0, 255, 128);
            colors[7] = new iTextSharp.text.BaseColor(0, 255, 255);
            colors[8] = new iTextSharp.text.BaseColor(0, 128, 255);
            colors[9] = new iTextSharp.text.BaseColor(0, 0, 255);
            colors[10] = new iTextSharp.text.BaseColor(128, 0, 255);
            colors[11] = new iTextSharp.text.BaseColor(255, 0, 255);
            colors[12] = new iTextSharp.text.BaseColor(255, 0, 128);
            colors[13] = new iTextSharp.text.BaseColor(0, 0, 0);
            colors[14] = new iTextSharp.text.BaseColor(255, 255, 255);

            for (int i = 0; i < gridRows; i++)
            {
                for (int j = 0; j < gridCols; j++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(" "));
                    cell.Colspan = 1;
                    cell.MinimumHeight = 40.0f;

                    int laneIndex = 0;
                    for (int truckNum = 0; truckNum < numOfTrucks; truckNum++)
                    {
                        laneIndex += truckLane[truckNum];
                        if (i < laneIndex)
                        {
                            cell.BackgroundColor = colors[truckNum];
                            break;
                        }
                    }

                    table.AddCell(cell);

                    if (i == 0 && j == 8)
                    {
                        table.AddCell(titleCell);
                    }
                }
            }

            doc.Add(table);

            doc.Close();
            writer.Close();
        }
    }
}
