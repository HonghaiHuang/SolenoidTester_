using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using FirstFloor.ModernUI.Windows.Controls;
using ExcelReport;
/// <summary>
/// 注意！需要引用debug目录下的ExcelReportDll文件夹所有dll,还需要引用System.Drawing；
/// </summary>
namespace SolenoidTester.DataClass
{
    class ExportExcel
    {

        /// <summary>
        /// 生成路径
        /// </summary>
        //    public string fimename=Directory.GetCurrentDirectory() + "/测试报告/档案模板"+System.DateTime.Now.ToString("yyyyMMddHHmmss")+".xls";



        public bool Isexporttoexcel = false;
        /// <summary>
        /// 存储测试信息
        /// </summary>
        private string[] _TestInformation = new string[10];

        /// <summary>
        /// 生成excel
        /// </summary>
        /// <returns></returns>
        public void  exporttoexcel(string[] _dataValue, string[,] SpeedTestData, string[,] _UnitTesting_data,string _fimeName, string[] TCUtemperature, string[,] StepTest_data, string[,] StepTest_Instruction, string[,] solenoid1Data, string[,] solenoid2Data)
        {
            CreateDirectory( "D:/产品档案/自动测试/" + ((App) Application.Current)._TestInformation[2]);//判断目录是否存在，不存在则创建
            _fimeName = "D:/产品档案/自动测试/" + ((App) Application.Current)._TestInformation[2] +"/" + _fimeName +".xls";
            DeleteFile(_fimeName);//判断是否已经存在该文件，存在则删除

            
            _TestInformation =((App) Application.Current)._TestInformation;
            ParameterCollection collection = new ParameterCollection();
            collection.Load(Directory.GetCurrentDirectory() + "/Excel_File/档案模板.xml");
            List<ElementFormatter> sheet1Formatters = new List<ElementFormatter>();
            List<ElementFormatter> sheet2Formatters = new List<ElementFormatter>();//单体测试

            List<ElementFormatter> sheet3Formatters = new List<ElementFormatter>();//阶跃测试

            List<ElementFormatter> sheet4Formatters = new List<ElementFormatter>();//换挡性能测试
            for (int i=0;i< _TestInformation.Length; i++)
            {
                sheet1Formatters.Add(new CellFormatter(collection["测试报告", "Infor" + i], _TestInformation[i]));
            }
            for(int j=0;j<_dataValue.Length;j++)
            {
                sheet1Formatters.Add(new CellFormatter(collection["测试报告", "dataValue" + j], _dataValue[j]));
            }



            for(int n=0;n< SpeedTestData.Length/4;n++)
            {
                sheet1Formatters.Add(new CellFormatter(collection["测试报告", "TCUInputSpeed" + n], SpeedTestData[0,n]));
                sheet1Formatters.Add(new CellFormatter(collection["测试报告", "TCUOnputSpeed" + n], SpeedTestData[2, n]));
            }


            //单体测试
            for (int j = 0; j < _UnitTesting_data.Length/14; j++)
            {
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "EPC_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[0,j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "EPC_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[1, j])));

                sheet2Formatters.Add(new CellFormatter(collection["数据表", "TCC_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[2, j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "TCC_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[3, j])));

                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C1234_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[4, j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C1234_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[5, j])));

                sheet2Formatters.Add(new CellFormatter(collection["数据表", "CB26_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[6, j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "CB26_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[7, j])));

                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C35R_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[8, j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C35R_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[9, j])));

                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C456_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[10, j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C456_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[11, j])));


            }
            //换挡电磁阀测试结果
            sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureL_H_L" ], Convert.ToDouble(_UnitTesting_data[12, 0])));
            sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureL_H_H"], Convert.ToDouble(_UnitTesting_data[12, 2])));
            if(_UnitTesting_data[12, 3]=="合格"&& _UnitTesting_data[12, 1]== "合格")
            {
                sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureL_H"], "合格"));
            }
            else
            { sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureL_H"], "不合格")); }

            sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureH_L_L" ], Convert.ToDouble(_UnitTesting_data[13, 0])));
            sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureH_L_H" ], Convert.ToDouble(_UnitTesting_data[13, 2])));
            if (_UnitTesting_data[13, 3] == "合格" && _UnitTesting_data[13, 1] == "合格")
            {
                sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureH_L"], "合格"));
            }
            else
            {
                sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureH_L"], "不合格"));
            }


            //温度测试结果
            sheet1Formatters.Add(new CellFormatter(collection["测试报告", "TCUtemperature"], TCUtemperature[0]+ "°C~" + TCUtemperature[1] + "°C") );


            //阶跃测试
            for (int t = 0; t < StepTest_data.Length / 12; t++)
            {

                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "EPC_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[0, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "EPC_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[1, t])));

                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "TCC_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[2, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "TCC_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[3, t])));

                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C1234_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[4, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C1234_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[5, t])));

                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "CB26_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[6, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "CB26_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[7, t])));

                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C35R_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[8, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C35R_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[9, t])));

                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C456_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[10, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C456_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[11, t])));






                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "EPC_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[0, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "EPC_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[1, t])));

                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "TCC_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[2, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "TCC_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[3, t])));

                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C1234_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[4, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C1234_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[5, t])));

                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "CB26_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[6, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "CB26_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[7, t])));

                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C35R_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[8, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C35R_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[9, t])));

                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C456_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[10, t])));
                sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C456_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[11, t])));


            }




            for (int t = 0; t < solenoid1Data.Length / 10; t++)
            {
                for(int k=0;k<10;k++)
                {
                    if(solenoid1Data[0, t]!="null")
                    {
                        sheet4Formatters.Add(new CellFormatter(collection["性能测试", "solenoid1Data"+k+"H_L" + t], Convert.ToDouble(solenoid1Data[k, t])));
                    }
                    else
                    {
                        sheet4Formatters.Add(new CellFormatter(collection["性能测试", "solenoid1Data" + k + "H_L" + t], ""));
                    }
                }
            }


            for (int t = 0; t < solenoid2Data.Length / 10; t++)
            {
                for (int k = 0; k < 10; k++)
                {
                    if (solenoid2Data[0, t] != "null")
                    {
                        sheet4Formatters.Add(new CellFormatter(collection["性能测试", "solenoid2Data" + k + "L_H" + t], Convert.ToDouble(solenoid2Data[k, t])));
                    }
                    else
                    {
                        sheet4Formatters.Add(new CellFormatter(collection["性能测试", "solenoid2Data" + k + "L_H" + t], ""));
                    }
                }
            }







            try
            {
                    //导出文件到本地
                    ExportHelper.ExportToLocal(Directory.GetCurrentDirectory() + "/Excel_File/档案模板.xls", _fimeName,
                        new SheetFormatterContainer("测试报告", sheet1Formatters),
                        //new SheetFormatterContainer("数据源30009", sheet2Formatters),
                        // new SheetFormatterContainer("数据源30010", sheet3Formatters),
                        // new SheetFormatterContainer("数据源30018", sheet4Formatters),
                         new SheetFormatterContainer("数据表", sheet2Formatters),
                          new SheetFormatterContainer("阶跃测试", sheet3Formatters),
                           new SheetFormatterContainer("性能测试", sheet4Formatters)
                        );
                Isexporttoexcel = true;
                }
                catch (Exception t)
                {
                Isexporttoexcel = false;
            }
        }


        public void UnitTesting(string[] _dataValue, string[,] SpeedTestData, string[,] _UnitTesting_data, string _fimeName, string[] TCUtemperature, string[,] StepTest_data, string[,] StepTest_Instruction, string[,] solenoid1Data, string[,] solenoid2Data)
        {
            CreateDirectory("D:/产品档案/自动测试/" + ((App)Application.Current)._TestInformation[2]);//判断目录是否存在，不存在则创建
            _fimeName = "D:/产品档案/自动测试/" + ((App)Application.Current)._TestInformation[2] + "/" + _fimeName + ".xls";
            DeleteFile(_fimeName);//判断是否已经存在该文件，存在则删除


            _TestInformation = ((App)Application.Current)._TestInformation;
            ParameterCollection collection = new ParameterCollection();
            collection.Load(Directory.GetCurrentDirectory() + "/Excel_File/档案模板.xml");
            List<ElementFormatter> sheet1Formatters = new List<ElementFormatter>();
            List<ElementFormatter> sheet2Formatters = new List<ElementFormatter>();//单体测试

            //List<ElementFormatter> sheet3Formatters = new List<ElementFormatter>();//阶跃测试

            //List<ElementFormatter> sheet4Formatters = new List<ElementFormatter>();//换挡性能测试
            for (int i = 0; i < _TestInformation.Length; i++)
            {
                sheet1Formatters.Add(new CellFormatter(collection["测试报告", "Infor" + i], _TestInformation[i]));
            }
            for (int j = 0; j < _dataValue.Length; j++)
            {
                sheet1Formatters.Add(new CellFormatter(collection["测试报告", "dataValue" + j], _dataValue[j]));
            }



            //for (int n = 0; n < SpeedTestData.Length / 4; n++)
            //{
            //    sheet1Formatters.Add(new CellFormatter(collection["测试报告", "TCUInputSpeed" + n], SpeedTestData[0, n]));
            //    sheet1Formatters.Add(new CellFormatter(collection["测试报告", "TCUOnputSpeed" + n], SpeedTestData[2, n]));
            //}


            //单体测试
            for (int j = 0; j < _UnitTesting_data.Length / 14; j++)
            {
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "EPC_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[0, j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "EPC_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[1, j])));

                sheet2Formatters.Add(new CellFormatter(collection["数据表", "TCC_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[2, j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "TCC_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[3, j])));

                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C1234_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[4, j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C1234_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[5, j])));

                sheet2Formatters.Add(new CellFormatter(collection["数据表", "CB26_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[6, j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "CB26_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[7, j])));

                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C35R_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[8, j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C35R_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[9, j])));

                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C456_RealPressureL_H" + j], Convert.ToDouble(_UnitTesting_data[10, j])));
                sheet2Formatters.Add(new CellFormatter(collection["数据表", "C456_RealPressureH_L" + j], Convert.ToDouble(_UnitTesting_data[11, j])));


            }
            ////换挡电磁阀测试结果
            //sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureL_H_L"], Convert.ToDouble(_UnitTesting_data[12, 0])));
            //sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureL_H_H"], Convert.ToDouble(_UnitTesting_data[12, 2])));
            //if (_UnitTesting_data[12, 3] == "合格" && _UnitTesting_data[12, 1] == "合格")
            //{
            //    sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureL_H"], "合格"));
            //}
            //else
            //{ sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureL_H"], "不合格")); }

            //sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureH_L_L"], Convert.ToDouble(_UnitTesting_data[13, 0])));
            //sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureH_L_H"], Convert.ToDouble(_UnitTesting_data[13, 2])));
            //if (_UnitTesting_data[13, 3] == "合格" && _UnitTesting_data[13, 1] == "合格")
            //{
            //    sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureH_L"], "合格"));
            //}
            //else
            //{
            //    sheet1Formatters.Add(new CellFormatter(collection["测试报告", "shift_RealPressureH_L"], "不合格"));
            //}


            ////温度测试结果
            //sheet1Formatters.Add(new CellFormatter(collection["测试报告", "TCUtemperature"], TCUtemperature[0] + "°C~" + TCUtemperature[1] + "°C"));


            ////阶跃测试
            //for (int t = 0; t < StepTest_data.Length / 12; t++)
            //{

            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "EPC_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[0, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "EPC_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[1, t])));

            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "TCC_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[2, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "TCC_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[3, t])));

            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C1234_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[4, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C1234_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[5, t])));

            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "CB26_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[6, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "CB26_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[7, t])));

            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C35R_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[8, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C35R_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[9, t])));

            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C456_StepTest_dataL_H" + t], Convert.ToDouble(StepTest_data[10, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C456_StepTest_dataH_L" + t], Convert.ToDouble(StepTest_data[11, t])));






            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "EPC_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[0, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "EPC_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[1, t])));

            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "TCC_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[2, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "TCC_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[3, t])));

            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C1234_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[4, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C1234_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[5, t])));

            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "CB26_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[6, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "CB26_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[7, t])));

            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C35R_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[8, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C35R_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[9, t])));

            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C456_StepTest_InstructionL_H" + t], Convert.ToDouble(StepTest_Instruction[10, t])));
            //    sheet3Formatters.Add(new CellFormatter(collection["阶跃测试", "C456_StepTest_InstructionH_L" + t], Convert.ToDouble(StepTest_Instruction[11, t])));


            //}




            //for (int t = 0; t < solenoid1Data.Length / 10; t++)
            //{
            //    for (int k = 0; k < 10; k++)
            //    {
            //        if (solenoid1Data[0, t] != "null")
            //        {
            //            sheet4Formatters.Add(new CellFormatter(collection["性能测试", "solenoid1Data" + k + "H_L" + t], Convert.ToDouble(solenoid1Data[k, t])));
            //        }
            //        else
            //        {
            //            sheet4Formatters.Add(new CellFormatter(collection["性能测试", "solenoid1Data" + k + "H_L" + t], ""));
            //        }
            //    }
            //}


            //for (int t = 0; t < solenoid2Data.Length / 10; t++)
            //{
            //    for (int k = 0; k < 10; k++)
            //    {
            //        if (solenoid2Data[0, t] != "null")
            //        {
            //            sheet4Formatters.Add(new CellFormatter(collection["性能测试", "solenoid2Data" + k + "L_H" + t], Convert.ToDouble(solenoid2Data[k, t])));
            //        }
            //        else
            //        {
            //            sheet4Formatters.Add(new CellFormatter(collection["性能测试", "solenoid2Data" + k + "L_H" + t], ""));
            //        }
            //    }
            //}







            try
            {
                //导出文件到本地
                ExportHelper.ExportToLocal(Directory.GetCurrentDirectory() + "/Excel_File/档案模板.xls", _fimeName,
                    new SheetFormatterContainer("测试报告", sheet1Formatters),
                     //new SheetFormatterContainer("数据源30009", sheet2Formatters),
                     // new SheetFormatterContainer("数据源30010", sheet3Formatters),
                     // new SheetFormatterContainer("数据源30018", sheet4Formatters),
                     new SheetFormatterContainer("数据表", sheet2Formatters)
                    );
                Isexporttoexcel = true;
            }
            catch (Exception t)
            {
                Isexporttoexcel = false;
            }
        }

        /// <summary>
        /// 压力开关测试报告
        /// </summary>
        /// <param name="_dataValue"></param>
        /// <param name="_fimeName"></param>
        public void exporttoexcelSwipress(string[] _dataValue, string _fimeName)
        {
            CreateDirectory("D:/产品档案/压力开关测试/" + ((App)Application.Current)._TestInformation[2]);//判断目录是否存在，不存在则创建
            _fimeName = "D:/产品档案/压力开关测试/" + ((App)Application.Current)._TestInformation[2] + "/" + _fimeName + ".xls";
            DeleteFile(_fimeName);//判断是否已经存在该文件，存在则删除
            ParameterCollection collection = new ParameterCollection();
            collection.Load(Directory.GetCurrentDirectory() + "/Excel_File/压力开关档案模板.xml");
            List<ElementFormatter> sheet1Formatters = new List<ElementFormatter>();
            for (int i = 0; i < _TestInformation.Length; i++)
            {
                sheet1Formatters.Add(new CellFormatter(collection["测试报告", "Infor" + i], _TestInformation[i]));
            }

            for (int j = 0; j < _dataValue.Length; j++)
            {
                sheet1Formatters.Add(new CellFormatter(collection["测试报告", "dataValue" + j], _dataValue[j]));
            }
            try
            {
                //导出文件到本地
                ExportHelper.ExportToLocal(Directory.GetCurrentDirectory() + "/Excel_File/压力开关档案模板.xls", _fimeName,
                    new SheetFormatterContainer("测试报告", sheet1Formatters)
                    );
                Isexporttoexcel = true;
            }
            catch (Exception t)
            {
                Isexporttoexcel = false;
            }


        }
        #region 检测指定目录是否存在  
        /// <summary>  
        /// 检测指定目录是否存在  
        /// </summary>  
        /// <param name="directoryPath">目录的绝对路径</param>          
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        #endregion
        #region 创建一个目录  
        /// <summary>  
        /// 创建一个目录  
        /// </summary>  
        /// <param name="directoryPath">目录的绝对路径</param>  
        public static void CreateDirectory(string directoryPath)
        {
            //如果目录不存在则创建该目录  
            if (!IsExistDirectory(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
        #endregion

        #region 检测指定文件是否存在  
        /// <summary>  
        /// 检测指定文件是否存在,如果存在则返回true。  
        /// </summary>  
        /// <param name="filePath">文件的绝对路径</param>          
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
        #endregion

        #region 删除指定文件  
        /// <summary>  
        /// 删除指定文件  
        /// </summary>  
        /// <param name="filePath">文件的绝对路径</param>  
        public static void DeleteFile(string filePath)
        {
            if (IsExistFile(filePath))
            {
                File.Delete(filePath);
            }
        }
        #endregion












        //调用方法用到
        /*
                ExportExcel excel = new ExportExcel();
        private void button_Click(object sender, RoutedEventArgs e)
        {
            SalaryInfo aa = new SalaryInfo();
            //using Microsoft.Win32;
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = @"Excel 2003文件|*.xls|Excel 2007文件|*.xlsx";
            if (saveFileDlg.ShowDialog() == true)
            {
                excel.fimename = @saveFileDlg.FileName;
                excel.exporttoexcel();
            }
        }

        */
    }
}
