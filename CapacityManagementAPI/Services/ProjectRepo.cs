using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapacityManagementAPI.Models;
namespace CapacityManagementAPI.Services
{
    public class ProjectRepo
    {
        public double getPoints(double allocation, double weight, string role)
        {
            allocation = allocation / 100;
            var hours = .5;
            var pts = 0.0;
            if (allocation == 0)
            {
                hours = .5;
            }
            else
            {
                hours = 6 * (allocation / 1);
            }
            if (role.Equals("BA"))
            {
                if (weight == .25)
                {
                    pts = hours / 63;
                }
                else if (weight == .5)
                {
                    pts = hours / 28;
                }
                else if (weight == .75)
                {
                    pts = hours / 13;
                }
                else
                {
                    pts = hours / 6;
                }
            }
            else if (role.Equals("QA"))
            {
                if (weight == .25)
                {
                    pts = hours / 63;
                }
                else if (weight == .5)
                {
                    pts = hours / 38;
                }
                else if (weight == .75)
                {
                    pts = hours / 19;
                }
                else
                {
                    pts = hours / 11;
                }
            }
            else 
            {
                if (weight == .25)
                {
                    pts = hours / 63;
                }
                else if (weight == .5)
                {
                    pts = hours / 33;
                }
                else if (weight == .75)
                {
                    pts = hours / 17;
                }
                else
                {
                    pts = hours / 9;
                }
            }
            return pts;
        }

        public DateTime GetEndDate(DateTime projectStartDate, string role, double remPts, ICollection<Allocation> allocations)
        {
            IDictionary<int, int> monLen = new Dictionary<int, int>()
            {
                {1, 30},
                {2, 27},
                {3, 30},
                {4, 29},
                {5, 30},
                {6, 29},
                {7, 30},
                {8, 30},
                {9, 29},
                {10, 30},
                {11, 29},
                {12, 30}
            };
            var date = DateTime.Today;
            if (DateTime.Compare(projectStartDate, date) > 0)
            {
                date = projectStartDate;
            }
            while (remPts > 0)
            {
                var noAl = true;
                //check if day is a workday
                if (!date.DayOfWeek.Equals(DayOfWeek.Saturday) && !date.DayOfWeek.Equals(DayOfWeek.Sunday))
                {
                    //look at each allocation with respect to the current day
                    foreach (Allocation allocation in allocations)
                    {
                        var startDate = (DateTime)allocation.StartDate;
                        var endDate = (DateTime)allocation.EndDate;
                        endDate = endDate.AddDays(monLen[endDate.Month]);
                        //see if that allocation is the current day and correct role
                        if (DateTime.Compare(date, startDate) >= 0
                            && DateTime.Compare(date, endDate) <= 0
                            && allocation.Role.Equals(role))
                        {
                            //there are people allocated
                            noAl = false;
                            remPts -= getPoints((double)allocation.Allocation1, (double)allocation.WorkWeight, allocation.Role);
                        }
                    }
                    //check if there were no allocation with the correct role this current day
                    if (noAl)
                    {
                        remPts -= (.5 / 65);
                    }

                }
                date = date.AddDays(1);
            }
            return date;
        }
    }
}
