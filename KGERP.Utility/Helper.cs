using System;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Utility
{
    public static class Helper
    {
        public static List<SelectModel> GetYears()
        {
            List<SelectModel> yearSelectModels = new List<SelectModel>();
            int year = DateTime.Today.Year;
            for (int i = 1950; i <= year; i++)
            {
                SelectModel yearSelectModel = new SelectModel
                {
                    Text = i,
                    Value = i
                };
                yearSelectModels.Add(yearSelectModel);
            }
            return yearSelectModels.OrderByDescending(x => x.Text).ToList();
        }

        public static List<SelectModel> GetMonths()
        {

            List<SelectModel> monthSelectModels = new List<SelectModel>() {
                new SelectModel{Text="Jan",Value=1},
                new SelectModel{Text="Feb",Value=2},
                new SelectModel{Text="Mar",Value=3},
                new SelectModel{Text="Apr",Value=4},
                new SelectModel{Text="May",Value=5},
                new SelectModel{Text="Jun",Value=6},
                new SelectModel{Text="Jul",Value=7},
                new SelectModel{Text="Aug",Value=8},
                new SelectModel{Text="Sep",Value=9},
                new SelectModel{Text="Oct",Value=10},
                new SelectModel{Text="Nov",Value=11},
                new SelectModel{Text="Dec",Value=12}
            };

            return monthSelectModels;
        }
        public static List<SelectModel> GetHours()
        {

            List<SelectModel> hours = new List<SelectModel>
            {

                new SelectModel { Text="01",Value=1},
                new SelectModel { Text="02",Value=2},
                new SelectModel { Text="03",Value=3},
                new SelectModel { Text="04",Value=4},
                new SelectModel { Text="05",Value=5},
                new SelectModel { Text="06",Value=6},
                new SelectModel { Text="07",Value=7},
                new SelectModel { Text="08",Value=8},
                new SelectModel { Text="09",Value=9},
                new SelectModel { Text="10",Value=10},
                new SelectModel { Text="11",Value=11},
                new SelectModel { Text="12",Value=12},
            };
            return hours;
        }

        public static List<SelectModel> GetMinutes()
        {

            List<SelectModel> minutes = new List<SelectModel>();
            SelectModel minute = new SelectModel();

            for (int i = 0; i < 59; i++)
            {
                if (i < 10)
                {
                    minute = new SelectModel
                    {
                        Text = "0" + i.ToString(),
                        Value = "0" + i.ToString()
                    };
                }
                else
                {
                    minute = new SelectModel
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    };
                }
                minutes.Add(minute);
            }
            return minutes;
        }

        public static List<SelectModel> GetFormet()
        {
            List<SelectModel> formats = new List<SelectModel>
            {
                new SelectModel{Text="AM",Value="AM"},
                new SelectModel{Text="PM",Value="AM"},

            };
            return formats;
        }
    }
}
